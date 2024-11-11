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

    [SerializeField] public List<TextMeshProUGUI> displayedInventoryCount = new List<TextMeshProUGUI>();
    [SerializeField] public List<Image> inventoryGbox = new List<Image>();

    [SerializeField] protected TextMeshProUGUI goldUI;
    [SerializeField] protected GameObject gameOverScreen;
    [SerializeField] protected GameObject SelectGunTutorialUI;

    [SerializeField] public GameObject rewardMenu;
    [SerializeField] public GameObject upgradePanel;
    [SerializeField] public GameObject pauseUI;
    
    public Texture2D crosshairTexture;
    public GameObject HandTexture;

    private Nexus _nexus;
    private PlayerHealth _playerHP;
    private PlayerLevels _playerLVL;
    private Image goldImage;

    private InventoryManager inventoryManager;
    private GameObject minimap;
    private GameObject uiObject;
    private GameObject inventoryBar;
    private GameObject expBar;
    
    public bool pauseMenuActive = false;
    public bool rewardMenuActive = false;

    private void Awake()
    {
        // References to Managers
        inventoryManager = GameManager.Instance.InventoryManager;
    }


    // Start is called before the first frame update
    void Start()
    {
        SetCursorCrosshair();
        _nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        _playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
         _playerLVL = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        goldImage = goldUI.transform.Find("Gold").GetComponent<Image>();
        minimap = GameObject.Find("MinimapComponent");
        uiObject = GameObject.Find("UI");
        inventoryBar = uiObject.transform.Find("Inventory Bar").gameObject;
        expBar = uiObject.transform.Find("EXP").gameObject;
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

        // only show gold count when the player have a harvester
        if (_playerLVL.currentLevel > 4) UpdateGoldUI(1);
    }

    public void SetCursorCrosshair()
    {
        Vector2 crossHotspot = new Vector2(crosshairTexture.width / 2, crosshairTexture.height / 2);
        Cursor.visible = true;
        HandTexture.SetActive(false);
        Cursor.SetCursor(crosshairTexture, crossHotspot, CursorMode.Auto);
    }

    public void SetCursorHand()
    {
        HandTexture.transform.position = Input.mousePosition;
        HandTexture.SetActive(true);
        Cursor.visible = false;
    }

    public void ShowGameOverScreen()
    {
        minimap.SetActive(false);
        inventoryBar.SetActive(false);
        expBar.SetActive(false);
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
        minimap.SetActive(false);
        inventoryBar.SetActive(false);
        expBar.SetActive(false);
    }

    public void HidePauseScreen()
    {
        if(rewardMenuActive){
            rewardMenu.SetActive(true);
        } 
        else {
            Time.timeScale = 1.0f;
            minimap.SetActive(true);
            inventoryBar.SetActive(true);
            expBar.SetActive(true);
        }
        pauseMenuActive = false;
        pauseUI.SetActive(false);
    }

    public void ShowRewardScreen()
    {
        minimap.SetActive(false);
        rewardMenuActive = true;
        rewardMenu.SetActive(true);
        Time.timeScale = 0.0f;
        inventoryBar.SetActive(false);
        expBar.SetActive(false);
    }

    public void HideRewardScreen()
    {
        minimap.SetActive(true);
        rewardMenuActive = false;
        rewardMenu.SetActive(false);
        Time.timeScale = 1.0f;
        inventoryBar.SetActive(true);
        expBar.SetActive(true);
    }

    public void ShowSelectGunTutorial()
    {
        SelectGunTutorialUI.SetActive(true);
        
    }

    public void HideSelectGunTutorial()
    {
        SelectGunTutorialUI.SetActive(false);
    }

    public void ShowXPTutorial()
    {
        SelectGunTutorialUI.SetActive(true);
        
    }

    public void UpdateWaveUI()
    {
        if (_nexus && _playerHP) scoreBoard.text = "Wave: " + GameManager.Instance.WaveManager.wave;
    }

    public void UpdatePlayerXPUI()
    {
        if (_playerLVL)
        {
            expUI.text = ( _playerLVL.currentLevel+1).ToString();
            expSlider.value =  _playerLVL.currentXP;
            expSlider.maxValue =  _playerLVL.xpNeededForLevel;
        }
    }

    public void UpdateGoldUI(int amount)
    {
        goldUI.text = ": " +  _playerLVL.currentGold;
        if (goldImage != null) goldImage.fillAmount = Mathf.Clamp01(amount); // Ensures fill amount stays between 0 and 1
    }

    public void UpdateRewardsUI(GameObject reward_opt_1, GameObject reward_opt_2, GameObject reward_opt_3)
    {
        rewardMenu.GetComponent<RewardChoiceUI>().UpdateRewardChoices(reward_opt_1, reward_opt_2, reward_opt_3);
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < InventoryManager.NUM_PLACEABLE_ITEMS; i++)
        {
            displayedInventoryCount[i].text = "";

            // based on if inventory of single item - change text + change background color
            if (inventoryManager.InventoryItemCount[i] > 0)
            {
                displayedInventoryCount[i].text = inventoryManager.InventoryItemCount[i].ToString();
                inventoryGbox[i].enabled = false; // disable greyed out background
            }
            else inventoryGbox[i].enabled = true; // enable greyed out background
        }
    }

    public void updateUpgradeUI(string buildingName, int materialNum, int id)
    {
        upgradePanel.GetComponent<upgradeUI>().updateText(buildingName, materialNum, id);
    }

}
