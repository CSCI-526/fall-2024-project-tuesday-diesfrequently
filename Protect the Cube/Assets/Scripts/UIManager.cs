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
    [SerializeField] public List<TextMeshProUGUI> inventoryCount = new List<TextMeshProUGUI>();
    [SerializeField] protected TextMeshProUGUI goldUI;
    [SerializeField] protected GameObject gameOverScreen;
    [SerializeField] protected GameObject rewardMenu;
    //[SerializeField] protected GameObject miniRewardMenu;
    [SerializeField] protected GameObject upgradePanel;
    [SerializeField] protected GameObject pauseUI;
    private Nexus nexus;
    private PlayerHealth playerHP;
    private PlayerLevels playerLevels;

    // Start is called before the first frame update
    void Start()
    {
        nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        playerLevels = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        UpdateUI();
    }

    // Update is called once per frame
    public void UpdateUI()
    {
        UpdateWaveUI();
        UpdatePlayerXPUI();
        UpdateInventoryUI();
        UpdateGoldUI();
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ShowPauseScreen()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void HidePauseScreen()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ShowRewardScreen()
    {
        rewardMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void HideRewardScreen()
    {
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
            expUI.text = "Player Level: " + playerLevels.currentLevel +
                "\r\nExp: " + playerLevels.currentXP + "/" + playerLevels.xpNeededForLevel;
        }
    }
    public void UpdateGoldUI()
    {
        goldUI.text = "Gold: " + playerLevels.currentGold;
        
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
