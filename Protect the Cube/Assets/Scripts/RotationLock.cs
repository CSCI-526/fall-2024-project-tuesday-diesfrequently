using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RotationLock : MonoBehaviour
{
    [SerializeField] public quaternion rotation = quaternion.identity;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = rotation;   
    }
}
