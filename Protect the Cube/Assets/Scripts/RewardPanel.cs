using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour
{
    [SerializeField] protected Image rewardImage;
    [SerializeField] protected TextMeshProUGUI displayedRewardName;
    [SerializeField] protected TextMeshProUGUI rewardDescription;

    public void UpdateRewardPanel(GameObject reward)
    {
        if (reward != null)
        {
            RewardInfo reward_info = reward.GetComponent<RewardInfo>();
            Building building = reward.GetComponent<Building>();

            // Prioritize "INFO" over BuildingInfo
            if (reward_info != null)
            {
                displayedRewardName.text = reward_info.RewardName;
                rewardDescription.text = reward_info.RewardDescription;
            }
            else if (building != null)
            {
                displayedRewardName.text = building.buildingName;
                rewardDescription.text = building.buildingDesc;
            }
        }
        else
        {
            Debug.Log("In ERROR.cs");
            displayedRewardName.text = "Error: missing";
            rewardDescription.text = "Error: missing";
        }
    }

    public void ClearRewardPanel()
    {
        displayedRewardName.text = "";
        rewardDescription.text = "";
    }

    public void OnPick()
    {
        if (displayedRewardName.text.Length > 0)
        {
            GameManager.Instance.InventoryManager.HandlePickedReward(displayedRewardName.text);
        }
    }
}
