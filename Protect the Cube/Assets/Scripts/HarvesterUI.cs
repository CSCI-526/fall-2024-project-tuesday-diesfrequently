using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HarvesterUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI tmp;
    [SerializeField] protected string title = "Harvester";

    private Nexus mNexus = null;
    private void Start()
    {   
        mNexus = GetComponent<Nexus>();
        if(mNexus != null)
        {
            mNexus.OnTakeDamage += UpdateHarvesterText;
        }
        UpdateHarvesterText();
    }

    // Update is called once per frame
    void UpdateHarvesterText()
    {
        tmp.text = title + ": (" + mNexus.health + "/" + mNexus.maxHealth + ")" + "\r\nDefend\r\nv";
    }
}
