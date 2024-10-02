using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChoiceUI : MonoBehaviour
{
    [SerializeField] protected RewardPanel panel1;
    [SerializeField] protected RewardPanel panel2;
    [SerializeField] protected RewardPanel panel3;
    public void UpdateRewardChoices(Building b1, Building b2, Building b3)
    {
        panel1.UpdateRewardPanel(b1);
        panel2.UpdateRewardPanel(b2);
        panel3.UpdateRewardPanel(b3);
    }
}
