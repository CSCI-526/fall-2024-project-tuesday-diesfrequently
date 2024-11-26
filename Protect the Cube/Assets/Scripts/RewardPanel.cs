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
    [SerializeField] protected Sprite emptyImage;
    [SerializeField] protected Image rewardImage;
    [SerializeField] protected TextMeshProUGUI displayedRewardName;
    [SerializeField] protected TextMeshProUGUI displayedRewardDescription;

    public void UpdateRewardPanel(GameObject reward)
    {
        if (reward != null)
        {
            RewardInfo reward_info = reward.GetComponent<RewardInfo>();

            // Check if Reward has "info" Script first
            if (reward_info != null)
            {   
                rewardImage.sprite = reward_info.RewardImage;
                displayedRewardName.text = reward_info.RewardName;

                // Update Reward Description (based on type of reward)
                if (reward_info.isStatBased) // needs an advanced description
                {
                    if (GameManager.Instance.DEBUG_REWARD_PANEL) Debug.Log("[Reward Panel] RewardDescription: " + reward_info.RewardDescription);
                    if (GameManager.Instance.DEBUG_REWARD_PANEL) Debug.Log("[Reward Panel] rangeDesc: " + reward_info.rangeDesc);
                    if (GameManager.Instance.DEBUG_REWARD_PANEL) Debug.Log("[Reward Panel] damageDesc: " + reward_info.damageDesc);
                    if (GameManager.Instance.DEBUG_REWARD_PANEL) Debug.Log("[Reward Panel] firerateDesc: " + reward_info.firerateDesc);

                    displayedRewardDescription.text = reward_info.RewardDescription +
                    "\n" + "[Range] " + String.Concat(Enumerable.Repeat("<sprite=0>", reward_info.rangeDesc)) +
                    "\n" + "[Damage] " + String.Concat(Enumerable.Repeat("<sprite=0>", reward_info.damageDesc)) +
                    "\n" + "[Fire Rate] " + String.Concat(Enumerable.Repeat("<sprite=0>", reward_info.firerateDesc));
                } else { displayedRewardDescription.text = reward_info.RewardDescription; }
            } else { // if no valid reward info found
                Debug.LogError("[Reward Panel] No Valid Reward Description");
                displayedRewardName.text = "Error: missing";
                displayedRewardDescription.text = "Error: missing";
            }
        } else { // if no valid reward is passed to the panel
            Debug.LogError("[Reward Panel] Reward Passed to Display UI is null");
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
        GameManager.Instance.UIManager.HideRewardUIMask();
        if (displayedRewardName.text.Length > 0)
        {
            GameManager.Instance.InventoryManager.HandlePickedReward(displayedRewardName.text);
        }
    }
}
