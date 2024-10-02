using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour
{
    [SerializeField] protected Image rewardImage;
    [SerializeField] protected TextMeshProUGUI rewardName;
    [SerializeField] protected TextMeshProUGUI rewardDescription;
    public void UpdateRewardPanel(Building reward)
    {
        if(reward != null)
        {
            rewardName.text = reward.buildingName;
            rewardDescription.text = reward.buildingDesc;
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
            GameManager.Instance.InventoryManager.PickReward(rewardName.text);
        }
    }
}
