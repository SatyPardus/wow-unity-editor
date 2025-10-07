using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MPQManager : IDisposable
{
    // Common archives
    private static readonly string[] ArchiveNameTemplates = new[]
    {
        "common.MPQ",
        "common-2.MPQ",
        "lichking.MPQ",
        "expansion.MPQ",
        //"base.MPQ", // commented in original
        "patch.MPQ",
        "patch-{number}.MPQ",
        "patch-{character}.MPQ",
        "alternate.MPQ"
    };

    // Locale-specific archives
    private static readonly string[] LocaleArchiveNameTemplates = new[]
    {
        "{locale}/lichking-speech-{locale}.MPQ",
        "{locale}/expansion-speech-{locale}.MPQ",

        "{locale}/lichking-locale-{locale}.MPQ",
        "{locale}/expansion-locale-{locale}.MPQ",

        "{locale}/speech-{locale}.MPQ",
        "{locale}/locale-{locale}.MPQ",

        "{locale}/patch-{locale}.MPQ",
        "{locale}/patch-{locale}-{number}.MPQ",
        "{locale}/patch-{locale}-{character}.MPQ",

        "development.MPQ"
    };

    private List<MPQ> loadedMPQs = new List<MPQ>();
    private string gameFolder;

    public HashSet<string> FileNames = new HashSet<string>();

    public MPQManager(string gameFolder)
    {
        this.gameFolder = gameFolder;

        if(!Directory.Exists(gameFolder) || !Directory.Exists(Path.Combine(gameFolder, "Data")) || !Directory.Exists(Path.Combine(gameFolder, "Data", "enUS")))
        {
            throw new Exception($"Invalid directory. Should be a WoW installation with enUS locale");
        }

        LoadArchives(Path.Combine(gameFolder, "Data"), "enUS");

        foreach(var mpq in loadedMPQs)
        {
            try
            {
                var listfile = mpq.LoadFile("(listfile)");
                if (listfile != null)
                {
                    Span<byte> data = listfile.Data;
                    string text = Encoding.UTF8.GetString(data);

                    // Split into lines
                    string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                    foreach (var line in lines)
                    {
                        if (!FileNames.Contains(line))
                        {
                            FileNames.Add(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load listfile: {ex}");
            }
        }

        loadedMPQs.Reverse();
        Debug.Log($"Loaded {loadedMPQs.Count} MPQs with {FileNames.Count} files");
    }

    public List<string> FindFiles(string part)
    {
        List<string> files = new List<string>();

        foreach(var file in FileNames)
        {
            if(file.Contains(part, StringComparison.InvariantCultureIgnoreCase))
            {
                files.Add(file);
            }
        }
        return files;
    }

    public MPQFile LoadFile(string filePath)
    {
        if(File.Exists(Path.Combine(gameFolder, "Data", "patch-F.mpq", filePath)))
        {
            return new MPQFile(File.ReadAllBytes(Path.Combine(gameFolder, "Data", "patch-F.mpq", filePath)), filePath);
        }
        foreach(var mpq in loadedMPQs)
        {
            var file = mpq.LoadFile(filePath);
            if(file != null)
            {
                return file;
            }
        }
        Debug.LogError($"Could not find {filePath}");
        return null;
    }

    private void LoadArchives(string basePath, string locale)
    {
        foreach (var template in ArchiveNameTemplates)
        {
            foreach (var expanded in ExpandTemplate(template, locale))
            {
                TryLoad(basePath, expanded);
            }
        }

        foreach (var template in LocaleArchiveNameTemplates)
        {
            foreach (var expanded in ExpandTemplate(template, locale))
            {
                TryLoad(basePath, expanded);
            }
        }
    }

    private IEnumerable<string> ExpandTemplate(string template, string locale)
    {
        if (template.Contains("{number}"))
        {
            for (int i = 0; i <= 9; i++)
                yield return template.Replace("{number}", i.ToString())
                                     .Replace("{locale}", locale);
        }
        else if (template.Contains("{character}"))
        {
            for (char c = 'A'; c <= 'Z'; c++)
                yield return template.Replace("{character}", c.ToString())
                                     .Replace("{locale}", locale);
        }
        else if (template.Contains("{locale}"))
        {
            yield return template.Replace("{locale}", locale);
        }
        else
        {
            yield return template;
        }
    }

    private void TryLoad(string basePath, string archiveName)
    {
        string fullPath = Path.Combine(basePath, archiveName);
        if (File.Exists(fullPath))
        {
            // Debug.Log($"[+] Loading: {archiveName}");
            var mpq = new MPQ(fullPath);
            loadedMPQs.Add(mpq);
        }
        else
        {
            //Debug.Log($"[-] Missing: {archiveName}");
        }
    }

    public void Dispose()
    {
        foreach(var mpq in loadedMPQs)
        {
            mpq.Dispose();
        }
        loadedMPQs.Clear();
    }
}
