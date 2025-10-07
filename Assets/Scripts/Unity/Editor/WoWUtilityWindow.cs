using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class WoWUtilityWindow : EditorWindow
{
    [MenuItem("WoW Utilities/General Utilities")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WoWUtilityWindow));
    }

    private string gameFolder = "E:\\Games\\Wow335Clean";
    private string filePath = "";

    private MPQManager mpqManager;
    private MPQFile currentFile;
    private Vector2 fileScroll;

    private Dictionary<string, object> loadedChunks = new Dictionary<string, object>();
    private Dictionary<string, Type> chunkTypes = new Dictionary<string, Type>();
    private List<string> searchedFiles = new List<string>();
    private int searchPage = 0;
    private int selectedSearchIndex = 0;

    private readonly List<string> requiredWMOGroupChunks = new List<string>()
    {
        "MVER",
        "MOGP"
    };

    private readonly List<string> requiredMOPGChunks = new List<string>()
    {
        "MOPY",
        "MOVI",
        "MOVT",
        "MONR",
        "MOTV",
        "MOBA"
    };

    private readonly List<string> optionalMOPGChunks = new List<string>()
    {
        "MOLR",
        "MODR",
        "MOBN",
        "MOBR",
        "MOCV",
        "MLIQ",
        "MORI",
        "MOTV",
        "MOCV"
    };

    private void OnGUI()
    {
        if (mpqManager == null)
        {
            gameFolder = EditorGUILayout.TextField(gameFolder);
            if (GUILayout.Button("Open MPQs"))
            {
                mpqManager = new MPQManager(gameFolder);
            }
            return;
        }
        if (GUILayout.Button("Close MPQs"))
        {
            UnloadFile();

            mpqManager.Dispose();
            mpqManager = null;
        }

        if (currentFile == null)
        {
            filePath = EditorGUILayout.TextField(filePath);
            if (GUILayout.Button("Open file"))
            {
                selectedSearchIndex = -1;

                var file = mpqManager.LoadFile(filePath);
                if (file == null)
                {
                    Debug.Log($"Failed to open {filePath}");
                }
                else
                {
                    currentFile = file;
                }
            }
            if (GUILayout.Button("Search"))
            {
                searchedFiles = mpqManager.FindFiles(filePath);
                searchPage = 0;
            }
            if(GUILayout.Button("Compute hashes"))
            {
                int count = 0;
                Dictionary<string, int> hashes = new Dictionary<string, int>();
                foreach(var entry in mpqManager.FileNames)
                {
                    if (EditorUtility.DisplayCancelableProgressBar("Computing hashes...", $"Current file: {count}/{mpqManager.FileNames.Count}", count / (float)mpqManager.FileNames.Count))
                    {
                        break;
                    }
                    count++;

                    try
                    {
                        var file = mpqManager.LoadFile(entry);
                        var hash = Utils.Sha256Hex(file.Data);
                        if (hashes.ContainsKey(hash))
                        {
                            hashes[hash]++;
                        }
                        else
                        {
                            hashes.Add(hash, 1);
                        }
                        file.Dispose();
                    }
                    catch(Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                }
                EditorUtility.DisplayProgressBar("Saving hashes", "Please wait", 1);
                StringBuilder sb = new StringBuilder();
                foreach(var item in hashes)
                {
                    sb.AppendLine($"{item.Key} {item.Value}");
                }
                File.WriteAllText("hashes.txt", sb.ToString());

                EditorUtility.ClearProgressBar();
            }

            if (searchedFiles.Count > 0)
            {
                float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                int maxVisibleEntries = Mathf.FloorToInt(position.height / lineHeight) - 8;

                EditorGUILayout.LabelField($"Found files: {searchedFiles.Count}", $"Page: {searchPage}/{searchedFiles.Count / maxVisibleEntries}");
                if (GUILayout.Button("Search for errors"))
                {
                    List<string> errors = new List<string>();
                    for (int i = 0; i < searchedFiles.Count; i++)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Checking for errors...", $"Current file: {i}/{searchedFiles.Count}", i / (float)searchedFiles.Count))
                        {
                            errors.Clear();
                            break;
                        }
                        var entry = searchedFiles[i];
                        try
                        {
                            if(entry.EndsWith("_lod1.wmo", StringComparison.InvariantCultureIgnoreCase) || entry.EndsWith("_lod2.wmo", StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }

                            var ext = Path.GetExtension(entry);
                            if (ext == ".wmo")
                            {
                                string nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(entry);

                                bool matches = nameWithoutExt.Length >= 4
                                               && nameWithoutExt[^4] == '_'
                                               && char.IsDigit(nameWithoutExt[^3])
                                               && char.IsDigit(nameWithoutExt[^2])
                                               && char.IsDigit(nameWithoutExt[^1]);

                                var file = mpqManager.LoadFile(entry);
                                if (matches)
                                {
                                    var group = new WMOGroup(file);
                                }
                                else
                                {
                                    var wmo = new WMO(file);
                                }
                            }
                        }
                        catch
                        {
                            errors.Add(entry);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                    if (errors.Count > 0)
                    {
                        searchedFiles = errors;
                    }
                }
                if(GUILayout.Button("AutoFix"))
                {
                    for (int i = 0; i < searchedFiles.Count; i++)
                    {
                        if (EditorUtility.DisplayCancelableProgressBar("Fixing files...", $"Current file: {i}/{searchedFiles.Count}", i / (float)searchedFiles.Count))
                        {
                            break;
                        }
                        var entry = searchedFiles[i];
                        try
                        {
                            var ext = Path.GetExtension(entry);
                            if (ext == ".wmo")
                            {
                                var file = mpqManager.LoadFile(entry);
                                if (file == null)
                                {
                                    Debug.Log($"Failed to open {entry}");
                                }
                                else
                                {
                                    currentFile = file;
                                }
                                AutoFixWMO();
                            }
                        }
                        catch(Exception ex)
                        {
                            Debug.LogError($"Failed to fix {entry}: {ex}");
                        }
                    }
                    EditorUtility.ClearProgressBar();
                }
                fileScroll = EditorGUILayout.BeginScrollView(fileScroll);
                for (int i = 0; i < maxVisibleEntries; i++)
                {
                    if (searchPage * maxVisibleEntries + i >= searchedFiles.Count) continue;

                    var fileName = searchedFiles[searchPage * maxVisibleEntries + i];
                    if (GUILayout.Button($"Open {fileName}"))
                    {
                        selectedSearchIndex = searchPage * maxVisibleEntries + i;

                        var file = mpqManager.LoadFile(fileName);
                        if (file == null)
                        {
                            Debug.Log($"Failed to open {fileName}");
                        }
                        else
                        {
                            currentFile = file;
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Previous Page"))
                {
                    searchPage--;
                    if (searchPage < 0)
                    {
                        searchPage = searchedFiles.Count / maxVisibleEntries;
                    }
                }
                if (GUILayout.Button("Next Page"))
                {
                    searchPage++;
                    if (searchPage * maxVisibleEntries > searchedFiles.Count)
                    {
                        searchPage = 0;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label($"Current File: {currentFile.FilePath} ({currentFile.Data.Length} bytes)");
            if (GUILayout.Button("Close"))
            {
                UnloadFile();
                return;
            }
            if (selectedSearchIndex != -1)
            {
                Event e = Event.current;

                EditorGUILayout.LabelField("Current file", $"{selectedSearchIndex}/{searchedFiles.Count}");

                EditorGUILayout.BeginHorizontal();
                if (selectedSearchIndex > 0)
                {
                    if (GUILayout.Button("Previous Item") || (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftArrow))
                    {
                        selectedSearchIndex--;

                        UnloadFile();
                        var fileName = searchedFiles[selectedSearchIndex];
                        var file = mpqManager.LoadFile(fileName);
                        if (file == null)
                        {
                            Debug.Log($"Failed to open {fileName}");
                        }
                        else
                        {
                            currentFile = file;
                        }
                        Repaint();
                    }
                }
                if (selectedSearchIndex < searchedFiles.Count - 1)
                {
                    if (GUILayout.Button("Next Item") || (e.type == EventType.KeyDown && e.keyCode == KeyCode.RightArrow))
                    {
                        selectedSearchIndex++;
                        UnloadFile();
                        var fileName = searchedFiles[selectedSearchIndex];
                        var file = mpqManager.LoadFile(fileName);
                        if (file == null)
                        {
                            Debug.Log($"Failed to open {fileName}");
                        }
                        else
                        {
                            currentFile = file;
                        }
                        Repaint();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(20);

            fileScroll = EditorGUILayout.BeginScrollView(fileScroll);
            var ext = Path.GetExtension(currentFile.FilePath);
            switch (ext)
            {
                case ".wmo": HandleWMO(); break;
                default: GUILayout.Label($"Not handling '{ext}' yet"); break;
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void AutoFixWMO()
    {
        Span<byte> data = currentFile.Data;

        int mogpSizePos = 0;

        int offset = 0;
        while (offset + 8 <= data.Length)
        {
            var chunk = new IffChunk(data.Slice(offset, 8), (uint)offset);
            offset += 8;
            int size = (int)chunk.Size;

            if (mogpSizePos != 0)
            {
                var oldSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(mogpSizePos, 4));
                BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(mogpSizePos, 4), (uint)(oldSize + size + 8));

                currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
            }

            if (offset + chunk.Size > data.Length)
            {
                size = data.Length - offset;
                BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(offset - 4, 4), (uint)size);

                currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
            }

            if (chunk.Token == "MOGP")
            {
                mogpSizePos = offset - 4;

                var availableSize = size;
                size = HandleMOGPFix(data.Slice((int)offset), size);
                if (size > availableSize)
                {
                    BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(offset - 4, 4), (uint)size);

                    currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
                }
            }
            offset += size;
        }
    }

    private int HandleMOGPFix(Span<byte> data, int chunkSize)
    {
        int offset = 68;
        while (offset + 8 <= data.Length)
        {
            var chunk = new IffChunk(data.Slice(offset, 8), (uint)offset);
            offset += 8;
            int size = (int)chunk.Size;

            if (offset + chunk.Size > data.Length)
            {
                size = data.Length - offset;
                BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(offset - 4, 4), (uint)size);

                currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
            }

            offset += size;
        }
        return offset;
    }

    private void HandleWMO()
    {
        bool isWMOGroup = false;
        var ext = Path.GetExtension(currentFile.FilePath);
        if (ext == ".wmo")
        {
            string nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(currentFile.FilePath);

            isWMOGroup = nameWithoutExt.Length >= 4
                           && nameWithoutExt[^4] == '_'
                           && char.IsDigit(nameWithoutExt[^3])
                           && char.IsDigit(nameWithoutExt[^2])
                           && char.IsDigit(nameWithoutExt[^1]);
        }

        try
        {
            if (isWMOGroup)
            {
                var group = new WMOGroup(currentFile);
            }
            else
            {
                var wmo = new WMO(currentFile);
            }
        }
        catch (Exception ex)
        {
            EditorGUILayout.HelpBox($"Exception: {ex.Message}", MessageType.Error);
        }

        Span<byte> data = currentFile.Data;

        int requiredChunkIDX = 0;
        int mogpSizePos = 0;

        int offset = 0;
        while (offset + 8 <= data.Length)
        {
            var chunk = new IffChunk(data.Slice(offset, 8), (uint)offset);
            offset += 8;
            int size = (int)chunk.Size;

            if (isWMOGroup)
            {
                if (requiredChunkIDX >= requiredWMOGroupChunks.Count)
                {
                    EditorGUILayout.HelpBox($"{chunk.Token} is outside of required chunk range!", MessageType.Error);
                    if (mogpSizePos != 0 && GUILayout.Button("Add to MOGP"))
                    {
                        var oldSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(mogpSizePos, 4));
                        BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(mogpSizePos, 4), (uint)(oldSize + size + 8));

                        currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
                    }
                }
                else
                {
                    if (requiredWMOGroupChunks[requiredChunkIDX] != chunk.Token)
                    {
                        EditorGUILayout.HelpBox($"{chunk.Token} should have been {requiredWMOGroupChunks[requiredChunkIDX]}!", MessageType.Error);
                    }
                    requiredChunkIDX++;
                }
            }

            if (offset + chunk.Size > data.Length)
            {
                EditorGUILayout.HelpBox($"{chunk.Token} is {(offset + chunk.Size) - data.Length} bytes bigger than the data itself!", MessageType.Error);
                size = data.Length - offset;

                if (GUILayout.Button("Fix"))
                {
                    BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(offset - 4, 4), (uint)size);

                    currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
                }
            }

            size = RenderChunk(data, offset, chunk, size);

            if (chunk.Token == "MOGP")
            {
                mogpSizePos = offset - 4;

                EditorGUI.indentLevel++;
                var availableSize = size;
                size = HandleMOGP(data, offset, size);
                if (size > availableSize)
                {
                    EditorGUILayout.HelpBox($"Has read {size - availableSize} bytes more than available", MessageType.Error);

                    if (GUILayout.Button("Fix"))
                    {
                        BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(offset - 4, 4), (uint)size);

                        currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
                    }
                }
                EditorGUI.indentLevel--;
            }
            offset += size;
        }
        EditorGUILayout.LabelField($"Final offset: {offset}/{data.Length}");
    }

    private int HandleMOGP(Span<byte> data, int baseOffset, int chunkSize)
    {
        int requiredChunkIDX = 0;
        int optionalChunkIDX = 0;

        int offset = baseOffset + 68;
        while (offset + 8 <= baseOffset + chunkSize)
        {
            var chunk = new IffChunk(data.Slice(offset, 8), (uint)offset);
            offset += 8;
            int size = (int)chunk.Size;

            if (requiredChunkIDX >= requiredMOPGChunks.Count)
            {
                while (optionalChunkIDX < optionalMOPGChunks.Count)
                {
                    if (optionalMOPGChunks[optionalChunkIDX] == chunk.Token)
                    {
                        break;
                    }
                    optionalChunkIDX++;
                }

                if (optionalChunkIDX >= optionalMOPGChunks.Count)
                {
                    EditorGUILayout.HelpBox($"{chunk.Token} is outside of required chunk range!", MessageType.Error);
                }
            }
            else
            {
                if (requiredMOPGChunks[requiredChunkIDX] != chunk.Token)
                {
                    EditorGUILayout.HelpBox($"{chunk.Token} should have been {requiredMOPGChunks[requiredChunkIDX]}!", MessageType.Error);
                }
                requiredChunkIDX++;
            }

            if (offset + chunk.Size > data.Length)
            {
                EditorGUILayout.HelpBox($"{chunk.Token} is {(offset + chunk.Size) - data.Length} bytes bigger than the data itself!", MessageType.Error);
                size = data.Length - offset;

                if (GUILayout.Button("Fix"))
                {
                    BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(offset - 4, 4), (uint)size);

                    currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
                }
            }

            size = RenderChunk(data, offset, chunk, size);

            offset += size;
        }
        EditorGUILayout.LabelField($"Total size: {offset} bytes");
        return offset - baseOffset;
    }

    private int RenderChunk(Span<byte> data, int offset, IffChunk chunk, int size)
    {
        EditorGUILayout.LabelField($"{chunk.Token}", $"{chunk.Size} bytes (+8 byte chunk = {chunk.Size + 8})");

        EditorGUI.indentLevel++;
        if (loadedChunks.TryGetValue(chunk.Token, out var loadedChunk))
        {
            RenderProperties(loadedChunk, chunk.Token);
        }
        else
        {
            if (chunkTypes.TryGetValue(chunk.Token, out var type))
            {
                var instance = (IChunk)Activator.CreateInstance(type);
                instance.Read(data.Slice(offset, size));
                RenderProperties(instance, chunk.Token);
                loadedChunks.Add(chunk.Token, instance);
            }
            else
            {
                EditorGUILayout.LabelField("Uknown type");
                if(GUILayout.Button("Fix"))
                {
                    var firstPart = data.Slice(0, offset - 8);
                    var secondPart = data.Slice(offset + size);

                    byte[] newData = new byte[firstPart.Length + secondPart.Length];
                    firstPart.CopyTo(newData);
                    secondPart.CopyTo(newData.AsSpan(firstPart.Length));

                    currentFile.Data = newData;

                    currentFile.Save(Path.Combine(gameFolder, "Data", "patch-f.mpq"));
                }
            }
        }
        EditorGUI.indentLevel--;
        return size;
    }

    private void RenderProperties(object obj, string name)
    {
        if (obj == null)
        {
            EditorGUILayout.LabelField($"Null");
            return;
        }
        foreach (var prop in obj.GetType().GetProperties())
        {
            if (IsSimple(prop))
            {
                EditorGUILayout.LabelField(prop.Name, $"{prop.GetValue(obj)?.ToString() ?? "None"}");
            }
            else if (prop.PropertyType.IsArray)
            {
                object value = prop.GetValue(obj);


                if (value is Array array)
                {
                    EditorGUILayout.LabelField(prop.Name, $"Array with {array.Length} {prop.PropertyType.GetElementType()} elements");
                    //for (int i = 0; i < array.Length; i++)
                    //{
                    //    EditorGUILayout.LabelField($"{i}:");
                    //    EditorGUI.indentLevel++;
                    //    //RenderProperties(array.GetValue(i), $"{name}/{prop.Name}_{i}");
                    //    EditorGUI.indentLevel--;
                    //}
                }
            }
            else
            {
                EditorGUILayout.LabelField(prop.Name, $"Unhandled type: {prop.PropertyType}");
            }
        }
    }

    static bool IsSimple(PropertyInfo prop)
    {
        var type = prop.PropertyType;

        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(Color)
            || type == typeof(Bounds)
            || type == typeof(Plane)
            || type == typeof(Vector2)
            || type == typeof(Vector3)
            || type == typeof(Quaternion);
    }

    private void Awake()
    {
        var interfaceType = typeof(IChunk);

        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes());
        var implementingTypes = allTypes
            .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        foreach (var t in implementingTypes)
        {
            chunkTypes.Add(t.Name, t);
        }
    }

    private void OnEnable()
    {
        if (chunkTypes.Count == 0)
        {
            var interfaceType = typeof(IChunk);

            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes());
            var implementingTypes = allTypes
                .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var t in implementingTypes)
            {
                chunkTypes.Add(t.Name, t);
            }
        }
    }

    private void OnDestroy()
    {
        UnloadFile();

        searchedFiles.Clear();

        mpqManager?.Dispose();
        mpqManager = null;
    }

    private void UnloadFile()
    {
        currentFile?.Dispose();
        currentFile = null;

        loadedChunks.Clear();
    }
}
