using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour
{
    [SerializeField] protected Image rewardImage;
    [SerializeField] protected Sprite emptyImage;
    [SerializeField] protected TextMeshProUGUI displayedRewardName;
    [SerializeField] protected TextMeshProUGUI displayedRewardDescription;

    public void UpdateRewardPanel(GameObject reward)
    {
        if (reward != null)
        {
            RewardInfo reward_info = reward.GetComponent<RewardInfo>();
            Building building = reward.GetComponent<Building>();

            // Prioritize "Reward Info" over BuildingInfo
            if (reward_info.rangeDesc != 0)
            {   
                rewardImage.sprite = reward_info.RewardImage;
                displayedRewardName.text = reward_info.RewardName;
                if (!reward_info.RewardName.Contains("HP"))
                {
                    displayedRewardDescription.text = reward_info.RewardDescription +
                    "\n" + "Range:" + String.Concat(Enumerable.Repeat("<sprite=0>", reward_info.rangeDesc)) +
                    "\n" + "Damage:" + String.Concat(Enumerable.Repeat("<sprite=0>", reward_info.damageDesc)) +
                    "\n" + "Fire Rate:" + String.Concat(Enumerable.Repeat("<sprite=0>", reward_info.firerateDesc));
                } else
                {
                    displayedRewardDescription.text = reward_info.RewardDescription;
                }
                
            }
            else if (building != null)
            {   
                rewardImage.sprite = reward_info.RewardImage;
                displayedRewardName.text = building.buildingName;
                displayedRewardDescription.text = building.buildingDesc;
            }
        }
        else
        {
            Debug.Log("In ERROR.cs");
            displayedRewardName.text = "Error: missing";
            displayedRewardDescription.text = "Error: missing";
        }
    }

    public void ClearRewardPanel()
    {
        displayedRewardName.text = "";
        displayedRewardDescription.text = "";
    }

    public void OnPick()
    {
        if (displayedRewardName.text.Length > 0)
        {
            GameManager.Instance.InventoryManager.HandlePickedReward(displayedRewardName.text);
        }
    }
}
