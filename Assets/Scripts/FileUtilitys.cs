using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class FileUtilitys
{
    #if UNITY_EDITOR
    [MenuItem("File/Gather Files")]
    #endif
    static void GenerateFileReport()
    {
        List<string> files = new List<string>();
        List<string> directories = new List<string>();

        GetDirectories(Application.dataPath, directories);

        for (int i = 0; i < directories.Count; i++)
        {
            string[] filesFromDirectory = Directory.GetFiles(directories[i]);
            for (int j = 0; j < filesFromDirectory.Length; j++)
            {
                if (!filesFromDirectory[j].Contains(".meta") &&
                  !filesFromDirectory[j].Contains(".unity") &&
                 !filesFromDirectory[j].Contains(".prefab") &&
                  !filesFromDirectory[j].Contains(".asset") &&
                  !filesFromDirectory[j].Contains(".controller") &&
                  !filesFromDirectory[j].Contains(".cs"))
                {
                    files.Add(Path.GetFileName(filesFromDirectory[j]));
                }
            }
        }

        File.WriteAllLines("FileNames.txt", files);
    }
    static void GetDirectories(string path, List<string> directories)
    {
        string[] dirs = Directory.GetDirectories(path);
        directories.AddRange(dirs);
        if (HasDirectories(dirs))
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                 GetDirectories(dirs[i], directories);
            }
        }
    }
    static bool HasDirectories(string[] directory)
    {
        return directory.Length > 0;
    }
}
