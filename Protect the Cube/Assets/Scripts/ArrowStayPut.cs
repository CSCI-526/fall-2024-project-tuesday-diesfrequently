using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ArrowStayPut : MonoBehaviour
{
    public Vector3 screenPos = new Vector3();

    void Update()
    {
        gameObject.transform.position = Camera.main.WorldToScreenPoint(screenPos);
    }
}
