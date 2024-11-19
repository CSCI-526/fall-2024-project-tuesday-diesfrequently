using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public GameObject HandTexture;



    public void SetCursorHand()
    {
        HandTexture.transform.position = Input.mousePosition;
        HandTexture.SetActive(true);
        Cursor.visible = false;

    }

    public void Start(){
        SetCursorHand();
    }

    public void Update(){
        Cursor.visible = false;
    }
}
