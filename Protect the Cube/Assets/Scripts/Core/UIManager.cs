using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject TutorialShootingCursor;
    public GameObject XPLevelUp;
    public TextMeshProUGUI expAnimText;
    [SerializeField] public GameObject playerHPSlider;
    [SerializeField] public GameObject nexusHPSlider;
    public GameObject gold;
    public GameObject pauseButton;
    public GameObject pauseButtonBackground;
    public GameObject RewardUIMask;
    public GameObject goldHighlight;
    public GameObject goldCollect;  

    [SerializeField] protected TextMeshProUGUI scoreBoard;
    [SerializeField] protected TextMeshProUGUI expUI;
    [SerializeField] protected Slider expSlider;

    [SerializeField] public List<TextMeshProUGUI> displayedInventoryCount = new List<TextMeshProUGUI>();

    [SerializeField] protected TextMeshProUGUI goldUI;
    [SerializeField] protected GameObject gameOverScreen;
    [SerializeField] protected GameObject SelectGunTutorialUI;

    [SerializeField] public GameObject rewardMenu;
    [SerializeField] public GameObject upgradePanel;
    [SerializeField] public GameObject pauseUI;
    [SerializeField] public GameObject inventoryBar;
    [SerializeField] public Image[] inventorySlots;
    [SerializeField] public Image[] inventoryItemPrefabs;
    [SerializeField] public Image damageEffect;

    [SerializeField] public GameObject InstructionModalWindow; 

    public GameObject ShootingCursor;
    public GameObject CustomCursor;

    private Nexus _nexus;
    private PlayerHealth _playerHP;
    private PlayerLevels _playerLVL;
    private Image goldImage;

    private InventoryManager inventoryManager;
    [SerializeField] public GameObject minimap;
    public GameObject uiObject;
    private GameObject expBar;

    public bool pauseMenuActive = false;
    private bool canPause = true;
    public bool rewardMenuActive = false;
    private int _currentHealth = 5;
    static private bool firstRewardScreenEnded = false;
    private bool goldActivated = false;
    private bool showNexusBar = false;
    private bool showPlayerBar = false;
    private bool showExpBar = false;
    private bool showInvenMini = false;
    public Dictionary<string, int> inventSlotMapping;

    private void Awake()
    {
        // References to Managers
        inventoryManager = GameManager.Instance.InventoryManager;
        inventSlotMapping = new Dictionary<string, int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        _playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        _playerLVL = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        goldImage = goldUI.transform.Find("Gold").GetComponent<Image>();
        //minimap = GameObject.Find("MinimapComponent");
        uiObject = GameObject.Find("UI");
        inventoryBar = uiObject.transform.Find("Inventory Bar").gameObject;
        expBar = uiObject.transform.Find("EXP").gameObject;

        RewardUIMask.SetActive(false);

        ActivateCustomCursor();
        UpdateUI();
    }

    public void ActivateDefaultCursor() {

        // one hot encoding for (4) cursor options
        Cursor.visible = true;
        ShootingCursor.SetActive(false);
        CustomCursor.SetActive(false);
        TutorialShootingCursor.SetActive(false);
    }

    public void ActivateCustomCursor() // SetCursorHand
    {
        CustomCursor.transform.position = Input.mousePosition;

        // one hot encoding for (4) cursor options
        ShootingCursor.SetActive(false);
        TutorialShootingCursor.SetActive(false);
        Cursor.visible = false;

        CustomCursor.SetActive(true);
        CustomCursor.GetComponent<FollowMouse>().ActivateTutorialShootingCursor();
    }

    public void ActivateCustomPlacementCursor() // SetCursorHand
    {
        CustomCursor.transform.position = Input.mousePosition;

        // one hot encoding for (4) cursor options
        ShootingCursor.SetActive(false);
        TutorialShootingCursor.SetActive(false);
        Cursor.visible = false;
        CustomCursor.SetActive(true);
        CustomCursor.GetComponent<FollowMouse>().ActivateTutorialShootingCursor();
    }

    public void ActivateShootingCursor()
    {
        ShootingCursor.transform.position = Input.mousePosition;

        // one hot encoding for (4) cursor options
        Cursor.visible = false;
        CustomCursor.SetActive(false);
        TutorialShootingCursor.SetActive(false);
        ShootingCursor.SetActive(true);
    }

    public void ActivateCustomShootingCursor() // SetCursorHand
    {
        CustomCursor.transform.position = Input.mousePosition;

        // one hot encoding for (4) cursor options
        CustomCursor.SetActive(false);
        ShootingCursor.SetActive(false);
        Cursor.visible = false;
        TutorialShootingCursor.SetActive(true);
    }

    private void Update()
    {
        // prevent "esc" error in WebGL builds
        //if (Input.GetMouseButtonDown(0)) Cursor.visible = false;

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
        // foreach (Image wbox in inventoryWbox)
        // {
        //     if (wbox.color.a > 0)
        //     {
        //         Color c = wbox.color;
        //         c.a -= 0.005f;
        //         wbox.color = c;
        //     }
        // }
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
        showInvenMini = true;
    }

    public void HideInventoryUI()
    {
        inventoryBar.SetActive(false);
        showInvenMini = false;
    }

    public void ActivateEXPUI()
    {
        expUI.gameObject.SetActive(true);
        expSlider.gameObject.SetActive(true);
        showExpBar = true;
    }

    public void HideEXPSlider()
    {
        expUI.gameObject.SetActive(false);
        expSlider.gameObject.SetActive(false);
        showExpBar = false;
    }

    public void ShowNexusHealthSlider() { 
        nexusHPSlider.SetActive(true);
        showNexusBar = true;
    }
    public void HideNexusHealthSlider() { 
        nexusHPSlider.SetActive(false); 
        showNexusBar = false;
    }

    public void ShowPlayerHealthSlider() { 
        playerHPSlider.SetActive(true); 
        showPlayerBar = true;
    }
    public void HidePlayerHealthSlider() { 
        playerHPSlider.SetActive(false); 
        showPlayerBar = false;
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
        pauseButtonBackground.SetActive(false);
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
        pauseButtonBackground.SetActive(true);
        if (rewardMenuActive) {
            rewardMenu.SetActive(true);
            if(goldActivated)
            {
                gold.SetActive(true);
            }
            if(showNexusBar)
            {
                nexusHPSlider.SetActive(true);
            }
            if(showPlayerBar)
            {
                playerHPSlider.SetActive(true);
            }
        }
        else {
            Time.timeScale = 1.0f;
            if(goldActivated)
            {
                gold.SetActive(true);
            }
            if(showNexusBar)
            {
                nexusHPSlider.SetActive(true);
            }
            if(showPlayerBar)
            {
                playerHPSlider.SetActive(true);
            }
            if(showExpBar)
            {
                expUI.gameObject.SetActive(true);
                expSlider.gameObject.SetActive(true);
            }
            if(showInvenMini)
            {
                inventoryBar.SetActive(true);
                minimap.SetActive(true);
            }
        }
        pauseMenuActive = false;
        pauseUI.SetActive(false);
    }

    public void ShowRewardUIMask()
    {
        RewardUIMask.SetActive(true);
    }

    public void ShowRewardScreen()
    {
        ActivateCustomCursor();
        gold.SetActive(false);
        XPLevelUp.SetActive(false);
        minimap.SetActive(false);
        rewardMenuActive = true;
        HideInventoryUI(); // tutorial
        rewardMenu.SetActive(true);
        Time.timeScale = 0.0f;
        inventoryBar.SetActive(false);
        expBar.SetActive(false);
    }

    public void HideRewardUIMask()
    {
        RewardUIMask.SetActive(false);
    }

    public void ShowMinimap() { minimap.SetActive(true); }

    public void HideMinimap() { minimap.SetActive(false); }

    public void HideRewardScreen()
    {
        ActivateShootingCursor();

        if (goldActivated) { gold.SetActive(true); }

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

    public void ShowModalWindow(string msg)
    {
        InstructionModalWindow.GetComponent<InstructionPopup>().ShowInstruction(msg);
    }

    public void HideModalWindow()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InstructionModalWindow.GetComponent<InstructionPopup>().HideInstruction();
        }
    }

    // 0 is full modal, 1 is bottom modal
    public void ConfigModalWindow(int modal_type)
    {
        InstructionModalWindow.SetActive(true); // modal needs to be active to call functions on it
        if (modal_type == 0) {
            InstructionModalWindow.GetComponent<InstructionPopup>().ConfigureModal(InstructionPopup.ModalType.FullScreen);
        } else if (modal_type == 1) {
            InstructionModalWindow.GetComponent<InstructionPopup>().ConfigureModal(InstructionPopup.ModalType.BottomBar);
        } else { Debug.LogError("[UI Manager] Error! Incorrect Modal_Type."); }
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

    public void Tutorial_ShowGoldHighlight()
    {
        goldHighlight.SetActive(true);
    }

    public void Tutorial_HideGoldHighlight()
    {
        goldHighlight.SetActive(false);
    } 
    
    public void Tutorial_ShowGoldCollect()
    {
        goldCollect.SetActive(true); 
    }

    public void UpdateWaveUI()
    {
        if (GameManager.Instance.WaveManager.wave_count == 0) scoreBoard.text = "";
        else { scoreBoard.text = "Wave: " + GameManager.Instance.WaveManager.wave_count; }
    }

    public void ShowWaveUI() { scoreBoard.gameObject.SetActive(true); }

    public void HideWaveUI() { scoreBoard.gameObject.SetActive(false);  }


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

    public void addInventoryToInventoryBar(int inventoryIDX, string inventoryName, int inventoryCount){
        if (inventSlotMapping.ContainsKey(inventoryName))
        {
            //update number directly
            Debug.Log("try update number directly");
            Image slot = inventorySlots[inventSlotMapping[inventoryName]];
            TextMeshProUGUI count = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (count != null)
            {
                count.text = inventoryCount.ToString();
            }
            else
            {
                Debug.LogError("Text component not found.");
            }
        }
        else
        {
            //Find next empty slot
            Debug.Log("try find empty slot");
            for (int i = 0; i < inventorySlots.Length; i++){
                Image slot = inventorySlots[i];
                if(slot.transform.childCount == 0){
                    Debug.Log("Found a slot");
                    Image inventoryItemPrefab = inventoryItemPrefabs[inventoryIDX];
                    Image instantiatedPrefab = Instantiate(inventoryItemPrefab, slot.transform);
                    instantiatedPrefab.transform.SetParent(slot.transform);
                    inventSlotMapping.Add(inventoryName, i);
                    break;
                }
            }
        }
    }

    public void removeInventoryFromInventoryBar(String inventoryName){
        Image slot = inventorySlots[inventSlotMapping[inventoryName]];
        foreach (Transform child in slot.transform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
        inventSlotMapping.Remove(inventoryName);
    }

    public void UpdateInventoryUI()
    {        
        for (int i = 0; i < InventoryManager.NUM_PLACEABLE_ITEMS; i++)
        {
            // based on if inventory of single item - add to inventory bar
            string inventoryName = inventoryManager.inventoryMapping.FirstOrDefault(x => x.Value == i).Key;
            if (inventoryManager.InventoryItemCount[i] > 0)
            {   
                addInventoryToInventoryBar(i, inventoryName, inventoryManager.InventoryItemCount[i]);
            }else if(inventoryManager.InventoryItemCount[i] == 0&&
                        inventSlotMapping.ContainsKey(inventoryName)){
                removeInventoryFromInventoryBar(inventoryName);
            }
            // else inventoryGbox[i].enabled = true; // enable greyed out background
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

    // public void FlashInventory(int itemIDX)
    // {   
    //     Color c = inventoryWbox[itemIDX].color;
    //     c.a = 1.0f;
    //     inventoryWbox[itemIDX].color = c;
    // }

    public static bool FirstRewardScreenEnded()
    {
        return firstRewardScreenEnded;
    }
}
