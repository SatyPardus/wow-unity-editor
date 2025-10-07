using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DbcEditor : MonoBehaviour
{
    public static DbcEditor Instance { get; private set; }

    public MPQManager MPQManager { get; private set; }


    public string GameFolder = "E:\\Games\\Wow335Clean";
    public string MapName = "Azeroth";
    public Material Material;

    public Dictionary<int, Material> materialCache = new Dictionary<int, Material>();
    private HashSet<uint> loadedObjs = new HashSet<uint>();

    private async void Awake()
    {
        Instance = this;

        var sw = System.Diagnostics.Stopwatch.StartNew();
        MPQManager = new MPQManager(GameFolder);
        Debug.Log($"Loaded all MPQs in {sw.ElapsedMilliseconds} ms");
        sw.Restart();

        var file = await LoadFile($"World\\Maps\\{MapName}\\{MapName}.wdt");
        Debug.Log($"Got file with length: {file.Data.Length} in {sw.ElapsedMilliseconds} ms");

        var wdt = new WDT(file);
        //file.Dispose();
        int size = 5;
        Vector2Int pos = new Vector2Int(31, 49);
        for (int x = pos.x - (size / 2); x <= pos.x + (size / 2); x++)
        {
            for (int y = pos.y - (size / 2); y <= pos.y + (size / 2); y++)
            {
                LoadADT(wdt, x, y);
            }
        }

        //var wmoFilePath = "World\\wmo\\Azeroth\\Buildings\\Stormwind\\Stormwind.wmo";
        //LoadWMO(wmoFilePath);
    }

    private async Task<GameObject> LoadWMO(string wmoFilePath)
    {
        try
        {
            var wmoFile = await LoadFile(wmoFilePath);
            var wmo = new WMO(wmoFile);

            var wmoObj = new GameObject(wmo.Name);

            for (int i = 0; i < wmo.MOHD.GroupCount; i++)
            {
                var wmoGroupFilePath = Path.Combine(wmo.Directory, $"{wmo.Name}_{i.ToString("D3")}.wmo");
                try
                {
                    var wmoGroupFile = await LoadFile(wmoGroupFilePath);
                    var wmoGroup = new WMOGroup(wmoGroupFile);

                    var wmoGroupObj = new GameObject();
                    var wmoGroupRenderer = wmoGroupObj.AddComponent<WMOGroupRenderer>();
                    await wmoGroupRenderer.Initialize(wmoObj.transform, wmo, wmoGroup);
                }
                catch(Exception ex)
                {
                    Debug.LogError($"Failed to load WMOGroup {wmoGroupFilePath}: {ex}");
                }
            }

            return wmoObj;
        }
        catch(Exception ex)
        {
            Debug.LogError($"Failed to load WMO {wmoFilePath}: {ex}");
            return null;
        }
    }

    private async void LoadADT(WDT wdt, int x, int y)
    {
        var adt = await Task.Run(async () =>
        {
            var adtFile = await LoadFile($"world\\maps\\{wdt.Name}\\{wdt.Name}_{x}_{y}.adt");
            var adt = new ADT(adtFile);
            //adtFile.Dispose();

            return adt;
        });

        var adtObj = new GameObject($"{wdt.Name}_{x}_{y}");

        foreach(var objDef in adt.MODF.MapObjects)
        {
            if (loadedObjs.Contains(objDef.UniqueID)) continue;
            loadedObjs.Add(objDef.UniqueID);

            var filePath = adt.MWMO.Filenames[adt.MWID.Offsets[objDef.NameID]];
            var wmoObj = await LoadWMO(filePath);
            if (wmoObj == null) continue;

            wmoObj.transform.localRotation = Quaternion.Euler(-objDef.Rotation.x, -objDef.Rotation.y + 90, -objDef.Rotation.z);
            wmoObj.transform.position = new Vector3(Constants.MaxXY - objDef.Position.z, objDef.Position.y, Constants.MaxXY - objDef.Position.x);
        }

        foreach (var chunk in adt.MCNK)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();

            var idx = 0;
            for (var i = 0; i < 17; i++)
            {
                var rowCount = (i % 2 != 0) ? 8 : 9;
                for (var j = 0; j < rowCount; j++)
                {
                    var v = new Vector3(
                        chunk.Position.x - (i * Constants.UnitSize * 0.5f),
                        chunk.MCVT.Heights[idx] + chunk.Position.z,
                        chunk.Position.y - (j * Constants.UnitSize)
                    );
                    if (i % 2 != 0) v.z -= 0.5f * Constants.UnitSize;

                    vertices.Add(v);

                    var n = chunk.MCNR.Normals[idx];
                    normals.Add(new Vector3(n.x, n.z, n.y));

                    var uv = new Vector2(
                        (i / 16) * Constants.ChunkSize,
                        i % 2 == 0 ? (j / 8) * Constants.ChunkSize : (Constants.ChunkSize / 8) * 7 * (j / 7) + Constants.ChunkSize / 16
                    );
                    uv /= Constants.UnitSize;
                    uvs.Add(uv);

                    idx++;
                }
            }

            Vector3 center = Vector3.zero;
            foreach (var v in vertices)
            {
                center += v;
            }
            center /= vertices.Count;

            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] -= center;
            }

            for (var j = 9; j < 8 * 8 + 9 * 8; j++)
            {
                //if (!chunk.HasHole(unitidx % 8, Math.floor(unitidx / 8))) {
                indices.AddRange(new int[] {
                    j - 9, j, j + 8,
                    j - 8, j, j - 9,
                    j + 9, j, j - 8,
                    j + 8, j, j + 9
                });
                //}
                if ((j + 1) % (9 + 8) == 0) j += 9;
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetNormals(normals);
            mesh.SetTriangles(indices, 0);

            var obj = new GameObject();
            obj.name = $"Chunk_{chunk.IndexX}_{chunk.IndexY}";
            var filter = obj.AddComponent<MeshFilter>();
            var renderer = obj.AddComponent<MeshRenderer>();

            filter.sharedMesh = mesh;
            renderer.material = Material;

            obj.transform.SetParent(adtObj.transform);
            obj.transform.position = center;
        }
    }

    private void OnApplicationQuit()
    {
        MPQManager.Dispose();
        MPQManager = null;
    }

    private Dictionary<string, Task<MPQFile>> loadingTasks = new Dictionary<string, Task<MPQFile>>();
    private Dictionary<string, MPQFile> cache = new Dictionary<string, MPQFile>();

    /// <summary>
    /// Loads the specified file.
    /// Priority is the DbcEditor Project path, then MPQs itself.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public async Task<MPQFile> LoadFile(string filePath)
    {
        if (cache.TryGetValue(filePath.ToLower(), out var file))
        {
            return file;
        }

        if(loadingTasks.TryGetValue(filePath.ToLower(), out var task))
        {
            return await task;
        }

        task = Task.Run(() =>
        {
            var res = MPQManager.LoadFile(filePath);
            cache.Add(filePath.ToLower(), res);
            loadingTasks.Remove(filePath.ToLower());
            return res;
        });
        loadingTasks.Add(filePath.ToLower(), task);

        return await task;
    }
}
