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

    public bool isXPOrbCollected { get; private set; } = false;

    [SerializeField] public List<GameObject> turretOptions = new List<GameObject>();

    [SerializeField] public int currentXP = 0;
    [SerializeField] public int currentLevel = 0;
    [SerializeField] public int currentGold = 0;

    private int levels_to_process = 0;
    private int accumulated_xp = 0;
    private bool isPlayerLevelTwo = false;

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

    public void markXPNotCollected() { isXPOrbCollected = false; }
    public void markXPCollected() { isXPOrbCollected = true; }

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
                    GetComponent<PlayerHealth>().AddPlayerHealth(1); // heal 1hp
                    if (currentLevel == 1) isPlayerLevelTwo = true; // starts at lvl 0
                    GameManager.Instance.AnalyticsManager.UpdatePlayerLevel(currentLevel); 

                    currentXP -= xpNeededForLevel;
                    levels_to_process += 1;
                    xpNeededForLevel = xpLinearScaler * currentLevel + xpNeededBase;
                }
                GameManager.Instance.UIManager.XPLevelUp.SetActive(true);
                Time.timeScale = 0.0f;
                GameManager.Instance.InventoryManager.GenerateRewards();
            }
            GameManager.Instance.UIManager.Tutorial_HideXPUI(); // hide xp UI
            //markXPNotCollected();
        }
        else if (Orb.tag == "GoldOrb"){
            if (!GameManager.Instance.InventoryManager.isFirstGoldCollected() && GameManager.Instance.IsTutorialEnabled)
            {
                GameManager.Instance.InventoryManager.setFirstGoldCollected(); // set to true
                GameManager.Instance.UIManager.Tutorial_HideXPUI(); // hide xp UI
                GameManager.Instance.UIManager.Tutorial_ShowGoldCollect(); // doesn't repeat
            }
            
            currentGold += resource_gained;
            GameManager.Instance.AnalyticsManager.UpdatePlayerAcquiredGold(resource_gained);

            if(currentGold > 4){
                clickUpgrade = FindObjectOfType<ClickUpgrade>();
                clickUpgrade.CheckForUpgradeableTurrets();
            }
        }
        GameManager.Instance.UIManager.UpdateUI();
    }

    public bool isLevelTwo() { return isPlayerLevelTwo; }
    public void ResetIsLevelTwo() { isPlayerLevelTwo = false; }

}
