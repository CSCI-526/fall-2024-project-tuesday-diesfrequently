using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerLevels : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public int xpNeededForLevel = 2;
    [SerializeField] public int xpNeededBase = 2;
    [SerializeField] public int xpLinearScaler = 2;

    [SerializeField] public List<GameObject> turretOptions = new List<GameObject>();

    [SerializeField] public int currentXP = 0;
    [SerializeField] public int currentLevel = 0;
    [SerializeField] public int currentGold = 0;

    private int levels_to_process = 0;
    private int accumulated_xp = 0;

    private bool isSelectingTurret = false;
    private List<GameObject> selectedTurrets = new List<GameObject>();

    public ClickUpgrade clickUpgrade;

    // Update is called once per frame
    void Update()
    {
        if (!isSelectingTurret && levels_to_process > 0){
            isSelectingTurret = true;
            levels_to_process -=1;
            List<GameObject> turretsOptionsTemp = turretOptions;
            for(int i=0; i<2; i++){
                //Debug.Log(turretsOptionsTemp.Count);
                int index = Random.Range(0, turretsOptionsTemp.Count);
                //Debug.Log(index);
                selectedTurrets.Add(turretsOptionsTemp[index]);

                turretsOptionsTemp.Remove(turretsOptionsTemp[index]);
            }
            //update hud to show the 2 turret options and save the 2 turret options as the values

        }
        if (isSelectingTurret){
            
        }
        
    }

    public void add_exp(int resource_gained, GameObject Orb){
        if (Orb.tag == "ExperienceOrb"){
            currentXP += resource_gained;
            accumulated_xp += currentXP;
            GameManager.Instance.AnalyticsManager.UpdatePlayerXP(accumulated_xp);

            if (currentXP >= xpNeededForLevel)
            {
                while (currentXP >= xpNeededForLevel)
                {
                    currentLevel += 1;
                    GameManager.Instance.AnalyticsManager.UpdatePlayerLevel(currentLevel); 

                    currentXP -= xpNeededForLevel;
                    levels_to_process += 1;
                    xpNeededForLevel = xpLinearScaler * currentLevel + xpNeededBase;
                }
                GameManager.Instance.UIManager.XPLevelUp.SetActive(true);
                Time.timeScale = 0.0f;
                //GameManager.Instance.UIManager.ShowRewardScreen();
                GameManager.Instance.InventoryManager.GenerateRewards();

                

            }
        }
        else if (Orb.tag == "GoldOrb"){
            currentGold += resource_gained;
            GameManager.Instance.AnalyticsManager.UpdatePlayerAcquiredGold(resource_gained);
            if(currentGold > 4){
                clickUpgrade = FindObjectOfType<ClickUpgrade>();
                clickUpgrade.CheckForUpgradeableTurrets();
            }
        }
        GameManager.Instance.UIManager.UpdateUI();
    }
}
