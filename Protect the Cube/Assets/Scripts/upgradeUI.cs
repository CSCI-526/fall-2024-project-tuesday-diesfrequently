using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class upgradeUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI upgradeInfo;
    [SerializeField] protected Button upgradeBtn;
    private upgradeBtn btn;

    public void updateText(string buildingName, int materialNum, int id)
    {
        upgradeInfo.text = "building required: " + buildingName + ":\t1\ngold required:\t" + materialNum;
        btn = upgradeBtn.GetComponent<upgradeBtn>();
        btn.id = id;
    }
}
