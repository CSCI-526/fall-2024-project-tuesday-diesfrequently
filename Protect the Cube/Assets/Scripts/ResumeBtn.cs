using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeBtn : MonoBehaviour
{
    public void resumeGame(){
        GameManager.Instance.UIManager.HidePauseScreen();
    }
}
