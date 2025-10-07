using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class WMOGroupRenderer : MonoBehaviour
{
    public WMOGroup WMOGroup { get; private set; }

    public async Task Initialize(Transform parent, WMO root, WMOGroup wmoGroup)
    {
        WMOGroup = wmoGroup;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Material> materials = new List<Material>();

        foreach (var vert in wmoGroup.MOVT.Vertices)
        {
            vertices.Add(new Vector3(vert.y, vert.z, -vert.x));
        }
        foreach (var norm in wmoGroup.MONR.Normals)
        {
            normals.Add(new Vector3(norm.y, norm.z, -norm.x));
        }
        uvs.AddRange(wmoGroup.MOTV.UVs);

        Vector3 center = Vector3.zero;
        foreach (var v in vertices)
        {
            center += v;
        }
        center /= vertices.Count;

        for (int vertIndex = 0; vertIndex < vertices.Count; vertIndex++)
        {
            vertices[vertIndex] -= center;
        }

        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        mesh.subMeshCount = wmoGroup.MOBA.Batches.Length;

        for (int batchIndex = 0; batchIndex < wmoGroup.MOBA.Batches.Length; batchIndex++)
        {
            var batch = wmoGroup.MOBA.Batches[batchIndex];
            mesh.SetTriangles(wmoGroup.MOVI.Indices.Skip((int)batch.StartIndex).Take(batch.Count).Reverse().ToArray(), batchIndex);

            var material = root.MOMT.Materials[batch.MaterialID];
            var textureName = root.MOTX.TextureNameList[material.Texture1];
            if (!DbcEditor.Instance.materialCache.TryGetValue(textureName.GetHashCode(), out var unityMaterial))
            {
                var blpFile = await DbcEditor.Instance.LoadFile(textureName);
                unityMaterial = new Material(DbcEditor.Instance.Material);
                using (MemoryStream memoryStream = new MemoryStream(blpFile.Data))
                {
                    try
                    {
                        var blp = new BlpFile(memoryStream);
                        unityMaterial.mainTexture = blp.GetTexture2d(0);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to load BLP {textureName}: {ex}");
                    }
                }

                DbcEditor.Instance.materialCache.Add(textureName.GetHashCode(), unityMaterial);
            }

            materials.Add(unityMaterial);
        }

        transform.name = $"{wmoGroup.GroupName}";
        var filter = transform.GetComponent<MeshFilter>();
        var renderer = transform.GetComponent<MeshRenderer>();
        renderer.receiveShadows = false;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        filter.sharedMesh = mesh;
        renderer.materials = materials.ToArray();

        transform.SetParent(parent);
        transform.position = center;

        //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = new Vector3(wmoGroup.MOBA.BoundingBox.center.x, wmoGroup.MOBA.BoundingBox.center.z, wmoGroup.MOBA.BoundingBox.center.y);
        //cube.transform.localScale = new Vector3(wmoGroup.MOBA.BoundingBox.extents.x, wmoGroup.MOBA.BoundingBox.extents.z, wmoGroup.MOBA.BoundingBox.extents.y) * 2;
        //cube.transform.SetParent(obj.transform);
    }
}
