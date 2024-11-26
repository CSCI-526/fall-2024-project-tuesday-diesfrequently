using System.Collections.Generic;
using UnityEngine;

public class RewardChoiceUI : MonoBehaviour
{
    [SerializeField] protected RewardPanel panel1;
    [SerializeField] protected RewardPanel panel2;
    [SerializeField] protected RewardPanel panel3;

    public void UpdateRewardChoices(GameObject b1, GameObject b2, GameObject b3)
    {
        GameObject[] rewards = new GameObject[] { b1, b2, b3 }; // array to store reward obj
        var uniqueRewards = new HashSet<GameObject>(rewards); // determine unique rewards 

        // start by de-activating all reward panels
        panel1.gameObject.SetActive(false);
        panel2.gameObject.SetActive(false);
        panel3.gameObject.SetActive(false);

        // only one panel per valid unique reward
        if (uniqueRewards.Count == 1)
        {
            panel2.gameObject.SetActive(true); // 1 valid reward = show middle panel
            panel2.UpdateRewardPanel(b1);
        }
        else if (uniqueRewards.Count == 2)
        {
            // two panels = two unique rewards
            if (b1 == b2)
            {
                panel1.gameObject.SetActive(true);
                panel1.UpdateRewardPanel(b1);

                panel2.gameObject.SetActive(true);
                panel2.UpdateRewardPanel(b3);
            }
            else if (b1 == b3)
            {
                panel1.gameObject.SetActive(true);
                panel1.UpdateRewardPanel(b1);

                panel2.gameObject.SetActive(true);
                panel2.UpdateRewardPanel(b2);
            }
            else if (b2 == b3)
            {
                panel1.gameObject.SetActive(true);
                panel1.UpdateRewardPanel(b1);

                panel2.gameObject.SetActive(true);
                panel2.UpdateRewardPanel(b2);
            }
        }
        else if (uniqueRewards.Count == 3)
        {
            // normal behavior
            panel1.gameObject.SetActive(true);
            panel1.UpdateRewardPanel(b1);

            panel2.gameObject.SetActive(true);
            panel2.UpdateRewardPanel(b2);

            panel3.gameObject.SetActive(true);
            panel3.UpdateRewardPanel(b3);
        }
    }
}
