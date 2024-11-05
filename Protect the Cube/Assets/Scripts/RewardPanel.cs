using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour
{
    [SerializeField] protected Image rewardImage;
    [SerializeField] protected TextMeshProUGUI rewardName;
    [SerializeField] protected TextMeshProUGUI rewardDescription;
    public void UpdateRewardPanel(GameObject reward)
    {
        Debug.Log("In RewardPanel ... reward_name: " + reward.name);
        if(reward != null)
        {
            Building building = reward.GetComponent<Building>();
            Info info = reward.GetComponent<Info>();

            // prioritize "INFO" before Building Info
            if (info != null)
            {
                rewardName.text = info.name;
                rewardDescription.text = info.Description;
            }
            else if (building != null)
            {
                rewardName.text = building.buildingName;
                rewardDescription.text = building.buildingDesc;
            }
        }
        else
        {
            rewardName.text = "Error: missing";
            rewardDescription.text = "Error: missing";
        }
    }

    public void ClearRewardPanel()
    {
        rewardName.text = "";
        rewardDescription.text = "";
    }

    public void OnPick()
    {
        if (rewardName.text.Length > 0)
        {
            GameManager.Instance.InventoryManager.HandlePickedReward(rewardName.text);
        }
    }
}
