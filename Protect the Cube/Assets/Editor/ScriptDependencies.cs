using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class ScriptDependencies
{
    private const string OutputPath = "Assets/Output/script_dependencies.txt";

    public static void AnalyzeAllScripts()
    {
        // Ensure the Output directory exists
        string directoryPath = Path.GetDirectoryName(OutputPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using (StreamWriter writer = new StreamWriter(OutputPath, false))
        {
            // Get all script files only from the "Scripts" folder
            string scriptsFolderPath = Path.Combine(Application.dataPath, "Scripts");
            string[] scriptPaths = Directory.GetFiles(scriptsFolderPath, "*.cs", SearchOption.AllDirectories);

            // Get valid class names from script file names
            HashSet<string> validClassNames = GetValidClassNames(scriptPaths);

            foreach (var path in scriptPaths)
            {
                string scriptName = Path.GetFileNameWithoutExtension(path);
                writer.WriteLine($"Script: {scriptName}");

                // Read the script contents
                string scriptContents = File.ReadAllText(path);
                List<string> dependencies = ExtractDependencies(scriptContents, validClassNames);

                // Remove the script's own name from the dependencies list if present
                dependencies.Remove(scriptName);

                // Extract GameObject/Prefab references
                List<string> gameObjectReferences = ExtractGameObjectReferences(scriptContents);

                // Output dependencies
                if (dependencies.Count > 0)
                {
                    writer.WriteLine($"    - Dependencies for {scriptName}:");
                    foreach (var dependency in dependencies)
                    {
                        writer.WriteLine($"        - {dependency}");
                    }
                }
                else
                {
                    writer.WriteLine($"    - No dependencies found for {scriptName}");
                }

                // Output GameObject/Prefab references
                if (gameObjectReferences.Count > 0)
                {
                    writer.WriteLine($"    - GameObjects/Prefabs referenced in {scriptName}:");
                    foreach (var gameObject in gameObjectReferences)
                    {
                        writer.WriteLine($"        - {gameObject}");
                    }
                }
                else
                {
                    writer.WriteLine($"    - No Explicit GameObjects/Prefabs referenced in {scriptName}");
                }

                writer.WriteLine(); // Add an empty line between each script
            }
        }
    }

    private static HashSet<string> GetValidClassNames(string[] scriptPaths)
    {
        HashSet<string> classNames = new HashSet<string>();

        foreach (var path in scriptPaths)
        {
            // Get the name of the script without the extension
            string scriptName = Path.GetFileNameWithoutExtension(path);
            classNames.Add(scriptName); // Add to the set of valid class names
            Debug.Log("ScriptName: " + scriptName);
        }

        return classNames;
    }

    private static List<string> ExtractDependencies(string scriptContents, HashSet<string> validClassNames)
    {
        // Use Regex to find class names that match the valid class names
        MatchCollection matches = Regex.Matches(scriptContents, @"(?<=\b)([A-Z][a-zA-Z0-9]*)");

        HashSet<string> dependencies = new HashSet<string>();
        foreach (Match match in matches)
        {
            string dependency = match.Value;
            // Add to dependencies only if it's in the list of valid class names
            if (validClassNames.Contains(dependency) && !dependencies.Contains(dependency))
            {
                dependencies.Add(dependency);
            }
        }

        return dependencies.ToList();
    }

    // New method to extract GameObject/Prefab references
    private static List<string> ExtractGameObjectReferences(string scriptContents)
    {
        List<string> gameObjectReferences = new List<string>();

        // Regex to find public GameObject declarations and GameObject instantiation
        // This pattern looks for public GameObject declarations and Instantiate calls
        string pattern = @"\b(public\s+)?GameObject\s+(\w+)\s*;|\bInstantiate\s*\(\s*.*?(\s*|""|'|)(\w+)(\s*|""|'|)\s*\)";
        MatchCollection matches = Regex.Matches(scriptContents, pattern);

        foreach (Match match in matches)
        {
            // Capture public GameObject names
            if (match.Groups[2].Success)
            {
                string referenceName = match.Groups[2].Value;
                if (!gameObjectReferences.Contains(referenceName))
                {
                    gameObjectReferences.Add(referenceName);
                }
            }

            // Capture instantiated GameObject names
            if (match.Groups[4].Success)
            {
                string instantiatedName = match.Groups[4].Value;
                if (!gameObjectReferences.Contains(instantiatedName))
                {
                    gameObjectReferences.Add(instantiatedName);
                }
            }
        }

        return gameObjectReferences;
    }

}