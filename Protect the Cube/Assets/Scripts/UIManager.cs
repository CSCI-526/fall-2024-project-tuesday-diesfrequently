using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI scoreBoard;
    [SerializeField] protected TextMeshProUGUI expUI;
    [SerializeField] protected TextMeshProUGUI inventoryUI;
    [SerializeField] protected GameObject gameOverScreen;
    [SerializeField] protected GameObject rewardMenu;

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
    }
    
    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0.0f;
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

    public void UpdateRewardsUI(Building b1, Building b2, Building b3)
    {
        rewardMenu.GetComponent<RewardChoiceUI>().UpdateRewardChoices(b1, b2, b3);
    }

    public void UpdateInventoryUI()
    {
        string txt = "Buildings:\n";
        InventoryManager inv = GameManager.Instance.InventoryManager;

        for(int i = 0; i < inv.buildingCount.Count; i++)
        {
            if (inv.buildingCount[i] != 0)
            {
                int j = i + 1;
                txt += "\n" + j + ": " + inv.buildingNames[i] + " x" + inv.buildingCount[i];
            }
        }

        inventoryUI.text = txt;
    }
}
