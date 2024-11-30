using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public WaveManager WaveManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public InventoryManager InventoryManager { get; private set; }
    public AnalyticsManager AnalyticsManager { get; private set; }
    public OreManager OreManager { get; private set; }
    [SerializeField] private GameObject MainCamera;

    public GameObject Player {  get; private set; }
    public GameObject Nexus {  get; set; }
    [SerializeField] public GameObject Barrier;

    public bool useBulletPool = false;
    public bool enableTutorial = true;
    private bool isPaused = false;
    private bool modalAcknowleged = false;
    private bool isCameraTransitionDone = false;

    // Set the field where TutorialStorageValue is stored
    private const string TutorialStorageKey = "IsTutorialEnabled";
    // states for GamePhase Tutorialization
    public enum GamePhase
    {
        Initialization,
        BasicTutorial_Start,
        BasicTutorial_Movement,
        BasicTutorial_Shooting,
        BasicTutorial_XP,
        BasicTutorial_Reward,
        BasicTutorial_Placement,
        HandCraftedWaves,
        AdvancedTutorial,
        DynamicWaves
    }

    // tracks current phase
    [SerializeField] public GamePhase StartPhase;
    public GamePhase currentPhase { get; private set; }
    [SerializeField] public bool DEBUG_WAVE_MANAGER;
    [SerializeField] public bool DEBUG_ORE_MANAGER;
    [SerializeField] public bool DEBUG_INVENTORY_MANAGER;
    [SerializeField] public bool DEBUG_REWARD_PANEL;

    public void SetGamePhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        switch (currentPhase)
        {
            case GamePhase.Initialization:
                StartInitialization();
                break;
            case GamePhase.BasicTutorial_Start:
                StartBasicTutorialStart();
                break;
            case GamePhase.BasicTutorial_Movement:
                StartMovementTutorial();
                break;
            case GamePhase.BasicTutorial_Shooting:
                StartShootingTutorial();
                break;
            case GamePhase.BasicTutorial_XP:
                StartXPTutorial();
                break;
            case GamePhase.BasicTutorial_Reward:
                StartRewardTutorial();
                break;
            case GamePhase.BasicTutorial_Placement:
                StartPlacementTutorial();
                break;
            case GamePhase.HandCraftedWaves:
                StartHandCraftedWaves();
                break;
            case GamePhase.DynamicWaves:
                StartDynamicWaves();
                break;

            // can include future states here 
        }
    }

    private void StartInitialization() {
        // do not show certain UI elements
        UIManager.HideModalWindow();
        UIManager.HideMinimap();
        UIManager.HideNexusHealthSlider();
        UIManager.HidePlayerHealthSlider();
        UIManager.HideInventoryUI();
        UIManager.HideEXPSlider();
        UIManager.HideWaveUI();

        // Activate Nexus + Player Animations
        Player.SetActive(true);
        Nexus.SetActive(true);
        EnableBarrier();
        SetSmallBarrier();
        Nexus.GetComponent<SpawnAnimation>().TriggerSpawnSequence();


        SetGamePhase(GamePhase.BasicTutorial_Start);
        Debug.Log("Finished GamePhase.Initialization Phase");
    }

    private void StartBasicTutorialStart()
    {
        // Spawn the player and nexus on the map
        Debug.Log("Starting GamePhase.BasicTutorial_Start Phase");
        

        Player.GetComponent<PlayerController>().LockShooting();
        Player.GetComponent<PlayerController>().LockMovement();
        Player.GetComponent<PlayerController>().DeactivatePlayerGun();

        WaveManager.LockAllEnemiesMovement();

        UIManager.ActivateCustomCursor(); // sets CustomCursor        

        Debug.Log("Ending GamePhase.BasicTutorial_Start Phase");
        StartCoroutine(WaitForInitializationEnd());
    }

    private IEnumerator WaitForInitializationEnd()
    {
        yield return new WaitForSeconds(0.5f); // wait 1s before state change
        SetGamePhase(GamePhase.BasicTutorial_Movement);
    }

    private void StartMovementTutorial()
    {
        Debug.Log("Starting GamePhase.BasicTutorial_Movement Phase");
        UIManager.Tutorial_ShowMovementUI(); // Shows the Animated 4 WASD Keys
        Player.GetComponent<PlayerController>().UnlockMovement();
        StartCoroutine(WaitForMovementInput());
    }

    private IEnumerator WaitForMovementInput()
    {
        yield return new WaitUntil(() => PlayerController.HasPressedMovementKeys());
        StartCoroutine(WaitForMovementTutorialEnd()); 
    }

    private IEnumerator WaitForMovementTutorialEnd()
    {
        yield return new WaitForSeconds(3.0f); // free movement for 3 seconds
        UIManager.Tutorial_HideMovementUI(); // hide WASD movement keys
        UIManager.ConfigModalWindow(1);
        UIManager.ShowModalWindow("Defend your Nexus!");
        StartCoroutine(WaitForNexusMessageAcknowlegement());
    }

    private IEnumerator WaitForNexusMessageAcknowlegement()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        UIManager.HideModalWindow();

        Debug.Log("Ending GamePhase.BasicTutorial_Movement Phase");
        SetGamePhase(GamePhase.BasicTutorial_Shooting);

    }

    private void StartShootingTutorial()
    {
        Debug.Log("Starting GamePhase.BasicTutorial_Shooting Phase");

        WaveManager.SpawnSingleEnemy(2);
        WaveManager.SetConstantXPDrops(0);
        WaveManager.LockAllEnemiesMovement();
        Player.GetComponent<PlayerController>().LockMovement();

        UIManager.ShowModalWindow("Enemies deal damage to your nexus (blue)");
        UIManager.ShowNexusHealthSlider();

        StartCoroutine(WaitForNexusHPAcknowledgement());
    }

    private IEnumerator WaitForNexusHPAcknowledgement()
    {
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        UIManager.HideModalWindow();
        WaveManager.UnlockAllEnemiesMovement();
        yield return new WaitForSeconds(2.0f);

        Player.GetComponent<PlayerController>().UnlockMovement();

        WaveManager.SpawnSingleEnemy(1);
        WaveManager.LockAllEnemiesMovement();
        StartCoroutine(WaitForCameraTransition());
    }

    private IEnumerator WaitForCameraTransition()
    {
        UIManager.ShowModalWindow("Touching enemies lowers your HP! (red)");
        UIManager.ShowPlayerHealthSlider();
        yield return new WaitForSeconds(1.0f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        UIManager.HideModalWindow();
        modalAcknowleged = false;
        //modalAcknowleged = true;
        //yield return new WaitUntil(() => modalAcknowleged); // wait for modal windows to be acknowledged

        MainCamera.GetComponent<CameraFollow>().SetNewTarget(WaveManager.AllEnemyEntities[0], 3.0f, OnCameraTransitionComplete);
        Debug.Log("Arrived Back in GameManager.");
        yield return new WaitUntil(() => isCameraTransitionDone);
        isCameraTransitionDone = false;
        
        StartCoroutine(WaitForShootingInput());
    }

    private void OnCameraTransitionComplete()
    {
        isCameraTransitionDone = true;
        Debug.Log("Set isCameraTransitionDone to TRUE!");
    }

    private IEnumerator WaitForShootingInput()
    {
        WaveManager.UnlockAllEnemiesMovement();
        yield return new WaitForSeconds(4.5f); // delay 4.5 seconds for enemy to come close to nexus
        WaveManager.LockAllEnemiesMovement();

        isCameraTransitionDone = false;
        MainCamera.GetComponent<CameraFollow>().SetNewTarget(Player, 1.5f, OnCameraTransitionComplete);
        yield return new WaitUntil(() => isCameraTransitionDone);

        UIManager.ActivateCustomShootingCursor(); // Show Custom Shooting Cursor
        Player.GetComponent<PlayerController>().ActivatePlayerGun(); // show player gun
        WaveManager.SetConstantXPDrops(4);
        Player.GetComponent<PlayerController>().UnlockShooting();
        
        // continue when player has shot + all enemies are dead
        yield return new WaitUntil(() => PlayerController.HasShotOnce() && WaveManager.AllEnemiesKilled());

        UIManager.ActivateShootingCursor(); // show shooting Crosshair
        Debug.Log("Ending GamePhase.BasicTutorial_Shooting Phase");
        SetGamePhase(GamePhase.BasicTutorial_XP);
    }

    private void StartXPTutorial()
    {
        Debug.Log("Starting GamePhase.BasicTutorial_XP Phase");
        UIManager.ActivateEXPUI();
        UIManager.Tutorial_ShowXPUI(new Vector3(-5.0f, 1.0f, 5.0f)); // show xp UI
        Debug.Log("Ending GamePhase.BasicTutorial_XP Phase");
        SetGamePhase(GamePhase.BasicTutorial_Reward);
    }

    private void StartRewardTutorial()
    {
        Debug.Log("Starting GamePhase.BasicTutorial_Reward Phase");
        StartCoroutine(WaitForRewardScreenEnd());
        Debug.Log("Ending GamePhase.BasicTutorial_Reward Phase");
        SetGamePhase(GamePhase.BasicTutorial_Placement);
    }

    private IEnumerator WaitForRewardScreenEnd()
    {
        yield return new WaitUntil(() => UIManager.FirstRewardScreenEnded());
        Debug.Log("Ending GamePhase.BasicTutorial_Reward Phase");
        SetGamePhase(GamePhase.BasicTutorial_Placement);
    }

   
    private void StartPlacementTutorial()
    {
        Debug.Log("Starting GamePhase.BasicTutorial_Placement Phase");
        SetMediumBarrier();
        WaveManager.LockAllEnemiesMovement();
        StartCoroutine(WaitForLevelTwo());
    }

    private IEnumerator WaitForLevelTwo()
    {
        yield return new WaitUntil(() => Player.GetComponent<PlayerLevels>().isLevelTwo());
        Player.GetComponent<PlayerController>().LockMovement();
        Player.GetComponent<PlayerController>().LockShooting();
        Player.GetComponent<PlayerController>().DeactivatePlayerGun();
        UIManager.ActivateCustomCursor(); // sets CustomCursor
        StartCoroutine(WaitForPlacementInput());
    }

    private IEnumerator WaitForPlacementInput()
    {
        // wait until player has "PLACED" turret
        yield return new WaitUntil(() => PlaceObject.firstTurretPlaced());
        WaveManager.UnlockAllEnemiesMovement();
        Player.GetComponent<PlayerController>().ActivatePlayerGun();

        Debug.Log("Ending GamePhase.BasicTutorial_Placement Phase");
        SetGamePhase(GamePhase.HandCraftedWaves);
    }

    private void StartHandCraftedWaves()
    {
        Debug.Log("ENTERING GamePhase.HandCraftedWaves Phase");
        DisableBarrier();
        Player.GetComponent<PlayerController>().UnlockMovement();
        Player.GetComponent<PlayerController>().UnlockShooting();
        Player.GetComponent<PlayerController>().ActivatePlayerGun();
        UIManager.ShowWaveUI();
        WaveManager.UnlockAllEnemiesMovement();
        UIManager.ShowMinimap();
    }

    private void StartDynamicWaves()
    {
        Debug.Log("ENTERING GamePhase.DynamicWaves Phase");
        DisableBarrier();
        Player.GetComponent<PlayerController>().UnlockMovement();
        Player.GetComponent<PlayerController>().UnlockShooting();
        WaveManager.UnlockAllEnemiesMovement();
    }

    /////////////////////////////
    // NON-TUTORIAL STATE CODE // 
    /////////////////////////////
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this.gameObject);
        else Instance = this;

        Instance.Player = GameObject.FindGameObjectWithTag("Player");
        Instance.Nexus = GameObject.FindGameObjectWithTag("Nexus");
        Instance.WaveManager = GetComponent<WaveManager>();
        Instance.UIManager = GetComponent<UIManager>();
        Instance.InventoryManager = GetComponent<InventoryManager>();
        Instance.AnalyticsManager = GetComponent<AnalyticsManager>();
        Instance.OreManager = GetComponent<OreManager>();

        DEBUG_WAVE_MANAGER = false;
        DEBUG_ORE_MANAGER = false;
        DEBUG_INVENTORY_MANAGER = true;
        DEBUG_REWARD_PANEL = false;
    }

    void Start()
    {
        // find main camera
        if (MainCamera == null)
            MainCamera = GameObject.FindWithTag("Main Camera"); // Ensure the Main Camera tag is set

        // retrieve Tutorial Value
        bool TutorialStorageValue = PlayerPrefs.GetInt(TutorialStorageKey, 1) == 1;
        Time.timeScale = 1.0f;

        // Tutorial Logic
        if (TutorialStorageValue && enableTutorial)
        {
            Debug.Log("Tutorial is Enabled");
            currentPhase = GamePhase.Initialization;
            SetGamePhase(currentPhase);
        }
        else
        {
            currentPhase = StartPhase;
            SetGamePhase(currentPhase);
            Debug.Log("Tutorial is SKIPPED");
        }
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0;  // freeze game time
        isPaused = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1;  // resume game time
        isPaused = false;
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);   
    }

    public void TriggerGameOver()
    {
        Instance.AnalyticsManager.SendSessionEndAnalytics(); // Send Analytics of Game End
        Instance.UIManager.ShowGameOverScreen();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackHome()
    {
        SceneManager.LoadScene(0);
    }

    public void SetSmallBarrier()
    {
        Barrier.transform.localScale = new Vector3(1.25f, 1.8f, 1.25f);
    }

    public void SetMediumBarrier()
    {
        Barrier.transform.localScale = new Vector3(1.5f, 1.8f, 1.5f);
    }

    private void EnableBarrier()
    {
        if (Barrier != null) Barrier.SetActive(true);
        Barrier.transform.position = Nexus.transform.position + new Vector3(5.0f, 22.33f, -5.0f);
    }

    private void DisableBarrier()
    {
        if (Barrier != null) Barrier.SetActive(false);
    }
}
