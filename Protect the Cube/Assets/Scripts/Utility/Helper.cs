using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static void checkArrayLengthSafe(GameObject[] array, string error_msg)
    {
        // Check if any prefabs were found
        if (array.Length == 0)
        {
            Debug.LogWarning("No prefabs found in the Resources/Prefabs/ directory.");
            return; // Exit the method if no prefabs are found
        }
    }
    
}
