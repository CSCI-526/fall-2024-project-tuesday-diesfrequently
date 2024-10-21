using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniRewardChoiceUI : MonoBehaviour
{
    [SerializeField] protected RewardPanel midPanel;

    public void UpdateMiniRewardChoices(GameObject m1)
    {
        midPanel.UpdateRewardPanel(m1);
    }
}
