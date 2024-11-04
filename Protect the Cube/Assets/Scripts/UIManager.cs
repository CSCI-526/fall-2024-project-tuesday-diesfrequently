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
    [SerializeField] public List<Image> inventoryGbox = new List<Image>();
    [SerializeField] protected TextMeshProUGUI goldUI;
    [SerializeField] protected GameObject gameOverScreen;
    [SerializeField] public GameObject rewardMenu;
    //[SerializeField] protected GameObject miniRewardMenu;
    [SerializeField] public GameObject upgradePanel;
    [SerializeField] public GameObject pauseUI;
    [SerializeField] protected GameObject SelectGunTutorialUI;
    // [SerializeField] protected GameObject HarvesterTutorialUI;

    public Texture2D crosshairTexture;
    public GameObject HandTexture;

    private Nexus nexus;
    private PlayerHealth playerHP;
    private PlayerLevels playerLevels;
    private Image goldImage;
    

    // Start is called before the first frame update
    void Start()
    {
        SetCursorCrosshair();
        nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        playerLevels = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        goldImage = goldUI.transform.Find("Gold").GetComponent<Image>();
        UpdateUI();

        //set cursor to be a crosshair
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
    public void SetCursorCrosshair(){
        Vector2 crossHotspot = new Vector2(crosshairTexture.width/2, crosshairTexture.height/2);
        Cursor.visible = true;
        HandTexture.SetActive(false);
        Cursor.SetCursor(crosshairTexture, crossHotspot, CursorMode.Auto);

    }
    public void SetCursorHand(){
        HandTexture.transform.position = Input.mousePosition;
        HandTexture.SetActive(true);
        Cursor.visible = false;

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
            if (inv.buildingCount[i] > 0){
                inventoryGbox[i].enabled = false;
            }else{
                 inventoryGbox[i].enabled = true;
            }
        }

    }
    public void updateUpgradeUI(string buildingName, int materialNum, int id)
    {
        upgradePanel.GetComponent<upgradeUI>().updateText(buildingName, materialNum, id);
    }

}
