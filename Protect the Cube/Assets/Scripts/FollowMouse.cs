using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    // Update is called once per frame
    void Update() { transform.position = Input.mousePosition; }

    public void ActivateTutorialShootingCursor()
    {
        GameObject LMB_Help = transform.Find("CursorDiagram/LMB help")?.gameObject;
        GameObject RMB_Help = transform.Find("CursorDiagram/RMB help")?.gameObject;

        if (LMB_Help == null || RMB_Help == null) Debug.LogError("[Follow Mouse] LMB / RMB Component not found!");

        LMB_Help.SetActive(true);
        RMB_Help.SetActive(true);
    }

    public void DeactivateTutorialShootingCursor()
    {
        GameObject LMB_Help = transform.Find("CursorDiagram/LMB help")?.gameObject;
        GameObject RMB_Help = transform.Find("CursorDiagram/RMB help")?.gameObject;

        if (LMB_Help == null || RMB_Help == null) Debug.LogError("[Follow Mouse] LMB / RMB Component not found!");

        LMB_Help.SetActive(false);
        RMB_Help.SetActive(false);
    }
}
