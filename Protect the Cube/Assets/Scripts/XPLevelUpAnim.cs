using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPLevelUpAnim : MonoBehaviour
{
    // Start is called before the first frame update
    public void ShowRewardScreen()
    {
        GameManager.Instance.UIManager.ShowRewardScreen();
    }
}
