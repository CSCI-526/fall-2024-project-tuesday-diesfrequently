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

    // references to gameobject + components
    private Nexus nexus;
    private PlayerHealth playerHP;
    private PlayerLevels playerLevels;

    // references to managers
    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        // References to Components
        nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        playerLevels = GameManager.Instance.Player.GetComponent<PlayerLevels>();

        // References to Managers
        inventoryManager = GameManager.Instance.InventoryManager;

        UpdateUI();
    }

    private void OnEnable()
    {
        inventoryManager.UI_OnInventoryUpdated += UpdateInventoryUI;
        inventoryManager.UI_OnRewardsUpdated += UpdateRewardsUI;
    }

    private void OnDisable()
    {
        inventoryManager.UI_OnInventoryUpdated -= UpdateInventoryUI;
        inventoryManager.UI_OnRewardsUpdated -= UpdateRewardsUI;
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
            scoreBoard.text = "Wave: " + GameManager.Instance.WaveManager.currentWaveIndex +
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

    //public void UpdateRewardsUI(GameObject b1, GameObject b2, GameObject b3)
    //{
    //    rewardMenu.GetComponent<RewardChoiceUI>().UpdateRewardChoices(b1, b2, b3);
    //}

    // chosenRewards contains the top 3 rewards generated
    public void UpdateRewardsUI(List<GameObject> chosenRewards)
    {
        // Call the method to update the reward choices with the new rewards
        rewardMenu.GetComponent<RewardChoiceUI>().UpdateRewardChoices(chosenRewards[0], chosenRewards[1], chosenRewards[2]);
    }

    public void UpdateInventoryUI()
    {
        // take snapshot of updated inventory item count
        List<int> inventoryItemCountSnapshot = inventoryManager.InventoryItemCount;

        for(int i = 0; i < inventoryItemCountSnapshot.Count; i++)
        {
            inventoryCount[i].text = "x" + inventoryItemCountSnapshot[i];
        }
    }
    public void updateUpgradeUI(string buildingName, int materialNum, int id)
    {
        upgradePanel.GetComponent<upgradeUI>().updateText(buildingName, materialNum, id);
    }

}
