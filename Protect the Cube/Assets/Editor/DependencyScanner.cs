using UnityEngine;
using UnityEditor;
using System.IO;

public class DependencyScanner : EditorWindow
{
    [MenuItem("Tools/Script Analysis Toolkit")]
    public static void ShowWindow()
    {
        GetWindow<DependencyScanner>("Script Analysis Toolkit");
    }

    private void OnGUI()
    {
        GUILayout.Label("Script Analysis Toolkit", EditorStyles.boldLabel);

        if (GUILayout.Button("Scan Prefab Script Dependencies"))
        {
            PrefabDependencies.ScanPrefabDependencies();
            Debug.Log($"Prefab dependencies have been saved to: Assets/Output/prefab_dependencies.txt");
        }

        if (GUILayout.Button("Analyze Script on Script Dependencies"))
        {
            ScriptDependencies.AnalyzeAllScripts();
            Debug.Log("Script dependencies have been saved to: Assets/Output/script_dependencies.txt");
        }

        if (GUILayout.Button("Analyze Function Definitions / Usages by Script"))
        {
            FunctionUseCases.AnalyzeAllFunctionUsages();
            Debug.Log("Function usage cases have been saved to: Assets/Output/function_usages.txt");
        }
    }
}
