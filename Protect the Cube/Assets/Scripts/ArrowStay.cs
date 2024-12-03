using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowStay : MonoBehaviour
{
    public Vector3 gamePos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(gamePos);
        screenPos.y += GameManager.Instance.UIManager.xpArrowOffset;
        transform.position = screenPos;
    }
}
