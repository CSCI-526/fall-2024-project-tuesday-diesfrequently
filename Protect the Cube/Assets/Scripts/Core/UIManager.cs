using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class UIManager : MonoBehaviour
{
    public GameObject WASD;
    public GameObject inventoryArrow;
    public GameObject xpArrow;
    public float xpArrowOffset;
    public GameObject holdMouse;
    public GameObject XPLevelUp;
    public TextMeshProUGUI expAnimText;
    public GameObject playerHPSlider;
    public GameObject nexusHPSlider;
    public GameObject gold;
    public GameObject pauseButton;

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
    [SerializeField] public GameObject inventoryBar;
    [SerializeField] public Image damageEffect;

    public GameObject crosshairTexture;
    public GameObject HandTexture;

    private Nexus _nexus;
    private PlayerHealth _playerHP;
    private PlayerLevels _playerLVL;
    private Image goldImage;

    private InventoryManager inventoryManager;
    private GameObject minimap;
    private GameObject uiObject;
    private GameObject expBar;

    public bool pauseMenuActive = false;
    private bool canPause = true;
    public bool rewardMenuActive = false;
    private int _currentHealth = 5;
    private bool isRewardLocked = true;
    static private bool firstRewardScreenEnded = false;
    private bool goldActivated = false;

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

    private void Update()
    {
        Cursor.visible = false;
        if (Input.GetKeyDown(KeyCode.P) && canPause)
        {
            if (pauseMenuActive) HidePauseScreen();
            else ShowPauseScreen();
        }

        // code for UI Screen Flash on Dmg Taken
        float atarget = (5 - _currentHealth) / 10.0f;
        if (damageEffect.color.a > atarget) {
            var color = damageEffect.color;
            color.a -= 0.01f;
            damageEffect.color = color;
        }

        // Displays the Inventory Panel
        foreach (Image wbox in inventoryWbox)
        {
            if (wbox.color.a > 0)
            {
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


    public void UpdateUI()
    {
        UpdateWaveUI();
        UpdatePlayerXPUI();
        UpdateInventoryUI();

        // Only show gold count when the player has a harvester
        if (_playerLVL.currentLevel > 4)
        {
            UpdateGoldUI(1);
            goldActivated = true;
        }
    }

    public void ActivateInventoryUI()
    {
        inventoryBar.SetActive(true);
    }

    public void DeactivateInventoryUI()
    {
        inventoryBar.SetActive(false);
    }

    public void ActivateEXPUI()
    {
        expUI.gameObject.SetActive(true);
        expSlider.gameObject.SetActive(true);
    }

    public void DeactivateEXPUI()
    {
        expUI.gameObject.SetActive(false);
        expSlider.gameObject.SetActive(false);
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
        canPause = false;
        playerHPSlider.SetActive(false);
        nexusHPSlider.SetActive(false);
        gold.SetActive(false);
        pauseButton.SetActive(false);
        minimap.SetActive(false);
        inventoryBar.SetActive(false);
        expBar.SetActive(false);
        gameOverScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ShowPauseScreen()
    {
        playerHPSlider.SetActive(false);
        nexusHPSlider.SetActive(false);
        gold.SetActive(false);
        pauseButton.SetActive(false);
        if (rewardMenuActive) {
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
        pauseButton.SetActive(true);
        playerHPSlider.SetActive(true);
        nexusHPSlider.SetActive(true);
        if (rewardMenuActive) {
            rewardMenu.SetActive(true);
        }
        else {
            Time.timeScale = 1.0f;
            minimap.SetActive(true);
            inventoryBar.SetActive(true);
            expBar.SetActive(true);
            if(goldActivated)
            {
                gold.SetActive(true);
            }
        }
        pauseMenuActive = false;
        pauseUI.SetActive(false);
    }

    public void ShowRewardScreen()
    {
        //canPause = false;
        //playerHPSlider.SetActive(false);
        //nexusHPSlider.SetActive(false);
        gold.SetActive(false);
        XPLevelUp.SetActive(false);
        minimap.SetActive(false);
        rewardMenuActive = true;
        DeactivateInventoryUI(); // tutorial
        rewardMenu.SetActive(true);
        Time.timeScale = 0.0f;
        inventoryBar.SetActive(false);
        expBar.SetActive(false);
    }

    public void HideRewardScreen()
    {
        //canPause = true;
        //playerHPSlider.SetActive(true);
        //nexusHPSlider.SetActive(true);
        if (goldActivated)
        {
            gold.SetActive(true);
        }
        minimap.SetActive(true);
        rewardMenuActive = false;
        rewardMenu.SetActive(false);
        ActivateInventoryUI(); // tutorial
        Time.timeScale = 1.0f;
        inventoryBar.SetActive(true);
        if (!firstRewardScreenEnded)
        {
            ShowSelectGunTutorial(); // show tutorial text (first time only
            firstRewardScreenEnded = true;
        }
        expBar.SetActive(true);
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
        //SelectGunTutorialUI.SetActive(true);
        inventoryArrow.SetActive(true);
    }

    public void HideSelectGunTutorial()
    {
        //SelectGunTutorialUI.SetActive(false);
        inventoryArrow.SetActive(false);
    }

    public void ShowXPTutorial()
    {
        SelectGunTutorialUI.SetActive(true);

    }

    // Author: Isabel --> Tutorial Functions
    public void Tutorial_ShowMovementUI()
    {
        WASD.SetActive(true);
    }

    public void Tutorial_HideMovementUI()
    {
        WASD.SetActive(false);
    }

    public void Tutorial_ShowShootingUI()
    {
        holdMouse.SetActive(true);
    }

    public void Tutorial_HideShootingUI()
    {
        holdMouse.SetActive(false);
    }

    public void Tutorial_ShowXPUI(Vector3 pos)
    {
        xpArrow.SetActive(true);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
        screenPos.y += xpArrowOffset;
        xpArrow.transform.position = screenPos;
    }

    public void Tutorial_HideXPUI()
    {
        xpArrow.SetActive(false);
    }

    public void UpdateWaveUI()
    {
        if ((_nexus && _playerHP) && ((GameManager.Instance.CurrentPhase == GameManager.GamePhase.HandCraftedWaves)
            || (GameManager.Instance.CurrentPhase == GameManager.GamePhase.DynamicWaves)))
        {
            scoreBoard.text = "Wave: " + GameManager.Instance.WaveManager.wave_count;
        }
    }

    public void UpdatePlayerXPUI()
    {
        if (_playerLVL)
        {
            expUI.text = ( _playerLVL.currentLevel+1).ToString();
            expAnimText.text = expUI.text;
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

    public void LockRewardUI()
    {
        isRewardLocked = true;
    }

    public void UnlockRewardUI()
    {
        isRewardLocked = false;
    }

    public static bool FirstRewardScreenEnded()
    {
        return firstRewardScreenEnded;
    }
}
