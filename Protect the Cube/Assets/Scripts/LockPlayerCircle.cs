using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPlayerCircle : MonoBehaviour
{
private Quaternion requiredLocalRot;

void Awake()
{
    requiredLocalRot = transform.localRotation;
}

void Update()
{
    if (transform.hasChanged)
    {
        transform.localRotation = requiredLocalRot;
        transform.hasChanged = false;
    }
}
}
