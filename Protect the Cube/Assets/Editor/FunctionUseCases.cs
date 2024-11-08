using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class FunctionUseCases
{
    private const string OutputPath = "Assets/Output/function_usages.txt";

    // A dictionary to hold function names with their corresponding class names
    private static Dictionary<string, List<string>> functionDefinitions = new Dictionary<string, List<string>>();

    public static void AnalyzeAllFunctionUsages()
    {
        // Ensure the Output directory exists
        string directoryPath = Path.GetDirectoryName(OutputPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Step 1: Collect defined functions
        CollectFunctionDefinitions();

        using (StreamWriter writer = new StreamWriter(OutputPath, false))
        {
            // Step 2: Iterate through each function and find usages
            foreach (var functionEntry in functionDefinitions)
            {
                string functionName = functionEntry.Key;
                List<string> classNames = functionEntry.Value;

                writer.WriteLine($"Function: {functionName}");
                writer.WriteLine($"Defined In Classes: {string.Join(", ", classNames)}");

                // Check where the function is used
                FindFunctionUsage(functionName, writer);
                writer.WriteLine(); // Add an empty line for separation
            }
        }
    }

    private static void CollectFunctionDefinitions()
    {
        // Get all script files
        string[] scriptPaths = Directory.GetFiles("Assets/Scripts", "*.cs", SearchOption.AllDirectories);

        foreach (var path in scriptPaths)
        {
            // Read the script contents
            string scriptContents = File.ReadAllText(path);
            string className = Path.GetFileNameWithoutExtension(path); // Assumes class name matches file name

            // Extract function names
            List<string> functionNames = ExtractFunctionNames(scriptContents);

            foreach (var functionName in functionNames)
            {
                if (!functionDefinitions.ContainsKey(functionName))
                {
                    functionDefinitions[functionName] = new List<string>();
                }
                functionDefinitions[functionName].Add(className);
            }
        }
    }

    private static List<string> ExtractFunctionNames(string scriptContents)
    {
        List<string> functionNames = new List<string>();

        // Regex pattern to match method signatures, including the class name
        string pattern = @"(?<!\w)(?!if\b|for\b|foreach\b|while\b|switch\b|case\b|return\b|new\b|do\b|try\b|catch\b|finally\b)(\w+)\s*\([^)]*\)\s*(?=\{|;)";
        MatchCollection matches = Regex.Matches(scriptContents, pattern);

        foreach (Match match in matches)
        {
            string functionName = match.Groups[1].Value;
            if (!functionNames.Contains(functionName))
            {
                functionNames.Add(functionName);
            }
        }

        return functionNames;
    }

    private static void FindFunctionUsage(string functionName, StreamWriter writer)
    {
        // Get all script files
        string[] scriptPaths = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories);

        foreach (var path in scriptPaths)
        {
            string scriptContents = File.ReadAllText(path);
            if (scriptContents.Contains(functionName))
            {
                writer.WriteLine($"    - Function '{functionName}' is called in: {path}");
            }
        }
    }
}