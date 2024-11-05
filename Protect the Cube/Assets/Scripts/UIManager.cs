using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI scoreBoard;
    [SerializeField] protected TextMeshProUGUI expUI;
    [SerializeField] protected Slider expSlider;
    [SerializeField] public List<TextMeshProUGUI> inventoryCount = new List<TextMeshProUGUI>();
    [SerializeField] protected TextMeshProUGUI goldUI;
    [SerializeField] protected GameObject gameOverScreen;
    [SerializeField] protected GameObject rewardMenu;
    //[SerializeField] protected GameObject miniRewardMenu;
    [SerializeField] protected GameObject upgradePanel;
    [SerializeField] protected GameObject pauseUI;
    [SerializeField] protected GameObject SelectGunTutorialUI;
    // [SerializeField] protected GameObject HarvesterTutorialUI;

    private Nexus nexus;
    private PlayerHealth playerHP;
    private PlayerLevels playerLevels;
    private Image goldImage;

    public bool pauseMenuActive = false;
    public bool rewardMenuActive = false;

    // Start is called before the first frame update
    void Start()
    {
        nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        playerLevels = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        goldImage = goldUI.transform.Find("Gold").GetComponent<Image>();
        UpdateUI();
    }

    // Update is called once per frame
    public void UpdateUI()
    {
        UpdateWaveUI();
        UpdatePlayerXPUI();
        UpdateInventoryUI();
        if(playerLevels.currentLevel > 4){
            // only show gold count when the player have a harvester
            UpdateGoldUI(1);
        }
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ShowPauseScreen()
    {
        if(rewardMenuActive){
            rewardMenu.SetActive(false);
        }
        pauseMenuActive = true;
        pauseUI.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void HidePauseScreen()
    {
        if(rewardMenuActive){
            rewardMenu.SetActive(true);
        } else {
            Time.timeScale = 1.0f;
        }
        pauseMenuActive = false;
        pauseUI.SetActive(false);
        
    }

    public void ShowRewardScreen()
    {
        rewardMenuActive = true;
        rewardMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void HideRewardScreen()
    {
        rewardMenuActive = false;
        rewardMenu.SetActive(false);
        Time.timeScale = 1.0f;
            
    }

    public void ShowUpgradeScreen()
    {
        upgradePanel.SetActive(true);
        Invoke("HideUpgradeScreen", 5.0f);
    }

    public void HideUpgradeScreen()
    {
        upgradePanel.SetActive(false);
    }

    public void ShowSelectGunTutorial()
    {
        SelectGunTutorialUI.SetActive(true);
        
    }

    public void HideSelectGunTutorial()
    {
        SelectGunTutorialUI.SetActive(false);
    }

    // public void ShowHarvesterTutorial()
    // {
    //     HarvesterTutorialUI.SetActive(true);
        
    // }
    // public void HideHarvesterTutorial()
    // {
    //     HarvesterTutorialUI.SetActive(false);
    // }
    public void ShowXPTutorial()
    {
        SelectGunTutorialUI.SetActive(true);
        
    }
    

    public void UpdateWaveUI()
    {
        if (nexus && playerHP)
        {
            scoreBoard.text = "Wave: " + GameManager.Instance.WaveManager.wave +
            "\r\nNexus: " + nexus.health + "/" + nexus.maxHealth +
            "\r\nHP: " + playerHP.currentHealth + "/" + playerHP.maxHealth;
        }
    }

    public void UpdatePlayerXPUI()
    {
        if (playerLevels)
        {
            /*expUI.text = "Player Level: " + playerLevels.currentLevel +
                "\r\nExp: " + playerLevels.currentXP + "/" + playerLevels.xpNeededForLevel;*/
            expUI.text = playerLevels.currentLevel.ToString();
            expSlider.value = playerLevels.currentXP;
            expSlider.maxValue = playerLevels.xpNeededForLevel;
        }
    }
    public void UpdateGoldUI(int amount)
    {
        goldUI.text = ": " + playerLevels.currentGold;
        if (goldImage != null)
        {
            goldImage.fillAmount = Mathf.Clamp01(amount); // Ensures fill amount stays between 0 and 1
        }
    }

    public void UpdateRewardsUI(GameObject b1, GameObject b2, GameObject b3)
    {
        rewardMenu.GetComponent<RewardChoiceUI>().UpdateRewardChoices(b1, b2, b3);
    }


    public void UpdateInventoryUI()
    {
        InventoryManager inv = GameManager.Instance.InventoryManager;

        for(int i = 0; i < inv.buildingCount.Count; i++)
        {
            inventoryCount[i].text = "x" + inv.buildingCount[i];
        }

    }
    public void updateUpgradeUI(string buildingName, int materialNum, int id)
    {
        upgradePanel.GetComponent<upgradeUI>().updateText(buildingName, materialNum, id);
    }

}
