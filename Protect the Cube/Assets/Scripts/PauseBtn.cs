using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBtn : MonoBehaviour
{   
    public void pauseGame(){
        GameManager.Instance.UIManager.ShowPauseScreen();
    }
    
}
