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
    [SerializeField] public List<Image> inventoryWbox = new List<Image>();
    [SerializeField] protected TextMeshProUGUI goldUI;
    [SerializeField] protected GameObject gameOverScreen;
    [SerializeField] protected GameObject SelectGunTutorialUI;

    [SerializeField] public GameObject rewardMenu;
    [SerializeField] public GameObject upgradePanel;
    [SerializeField] public GameObject pauseUI;
    
    public GameObject crosshairTexture;
    public GameObject HandTexture;

    private Nexus _nexus;
    private PlayerHealth _playerHP;
    private PlayerLevels _playerLVL;
    private Image goldImage;

    private InventoryManager inventoryManager;
    
    public bool pauseMenuActive = false;
    public bool rewardMenuActive = false;

    public Image damageEffect;

    private int _currentHealth = 5;

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
        UpdateUI();
    }
        
    private void Update()
    {
        Cursor.visible = false;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuActive) HidePauseScreen();
            else ShowPauseScreen();
        }
        float atarget = (5 - _currentHealth)/10.0f;
        if(damageEffect.color.a > atarget){
            var color = damageEffect.color;
            color.a -= 0.01f;
            damageEffect.color = color;
        }
        foreach (Image wbox in inventoryWbox){
            if(wbox.color.a > 0){
                Color c = wbox.color;
                c.a -= 0.005f;
                wbox.color = c;
            }
        }
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
        crosshairTexture.transform.position = Input.mousePosition;
        crosshairTexture.SetActive(true);
        HandTexture.SetActive(false);
    }

    public void SetCursorHand()
    {
        HandTexture.transform.position = Input.mousePosition;
        HandTexture.SetActive(true);
        crosshairTexture.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ShowPauseScreen()
    {
        if(rewardMenuActive) rewardMenu.SetActive(false);
        pauseMenuActive = true;
        pauseUI.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void HidePauseScreen()
    {
        if(rewardMenuActive) rewardMenu.SetActive(true);
        else Time.timeScale = 1.0f;
        
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

    public void DamageEffect(int health)
    {   
        Color color = damageEffect.color;
        if (health > _currentHealth){
            color.a = (5 - _currentHealth)/10.0f;
        } else {
            color.a = 0.8f;
        }
        _currentHealth = health;
        damageEffect.color = color;
    }

    public void FlashInventory(int itemIDX)
    {   
        Color c = inventoryWbox[itemIDX].color;
        c.a = 1.0f;
        inventoryWbox[itemIDX].color = c;
    }
}
