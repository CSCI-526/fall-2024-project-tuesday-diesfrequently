//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RewardChoiceUI : MonoBehaviour
//{
//    [SerializeField] protected RewardPanel panel1;
//    [SerializeField] protected RewardPanel panel2;
//    [SerializeField] protected RewardPanel panel3;
//    public void UpdateRewardChoices(GameObject b1, GameObject b2, GameObject b3)
//    {
//        panel1.UpdateRewardPanel(b1);
//        panel2.UpdateRewardPanel(b2);
//        panel3.UpdateRewardPanel(b3);
//    }
//}

using UnityEngine;

public class RewardChoiceUI : MonoBehaviour
{
    [SerializeField] protected RewardPanel panel1;
    [SerializeField] protected RewardPanel panel2;
    [SerializeField] protected RewardPanel panel3;

    public void UpdateRewardChoices(GameObject b1, GameObject b2, GameObject b3)
    {
        // Check if all three GameObjects are the same
        if (b1 == b2 && b2 == b3)
        {
            // Only show the middle panel (panel2)
            panel1.gameObject.SetActive(false);
            panel2.gameObject.SetActive(true);
            panel3.gameObject.SetActive(false);

            // Update the middle panel with the common GameObject
            panel2.UpdateRewardPanel(b2);
        }
        else
        {
            // Show all panels and update each one
            panel1.gameObject.SetActive(true);
            panel2.gameObject.SetActive(true);
            panel3.gameObject.SetActive(true);

            panel1.UpdateRewardPanel(b1);
            panel2.UpdateRewardPanel(b2);
            panel3.UpdateRewardPanel(b3);
        }
    }
}
