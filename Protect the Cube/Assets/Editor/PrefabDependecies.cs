using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class PrefabDependencies
{
    private const string OutputPath = "Assets/Output/prefab_dependencies.txt";

    public static void ScanPrefabDependencies()
    {
        // Ensure the Output directory exists
        string directoryPath = Path.GetDirectoryName(OutputPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using (StreamWriter writer = new StreamWriter(OutputPath, false))
        {
            string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab");

            foreach (string path in prefabPaths)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(path);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                writer.WriteLine($"Prefab: {prefab.name}");

                // Use HashSet to keep track of unique MonoBehaviour types for each prefab
                HashSet<string> uniqueScripts = new HashSet<string>();

                // Get all MonoBehaviour components only
                MonoBehaviour[] scripts = prefab.GetComponentsInChildren<MonoBehaviour>(true);

                foreach (var script in scripts)
                {
                    string scriptName = script.GetType().Name;
                    if (uniqueScripts.Add(scriptName)) // Add returns false if the item already exists
                    {
                        writer.WriteLine($"    - {scriptName}");
                    }
                }

                writer.WriteLine(); // Add an empty line between each prefab
            }
        }
    }
}