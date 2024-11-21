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

    public GameObject Player {  get; private set; }
    public GameObject Nexus {  get; set; }

    public bool useBulletPool = false;
    public bool enableTutorial = true;
    private bool isPaused = false;

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
    [SerializeField] public GamePhase CurrentPhase = GamePhase.Initialization;
    [SerializeField] public bool DEBUG_WAVE_MANAGER;
    [SerializeField] public bool DEBUG_ORE_MANAGER;
    [SerializeField] public bool DEBUG_INVENTORY_MANAGER;


    public void SetGamePhase(GamePhase newPhase)
    {
        CurrentPhase = newPhase;
        switch (CurrentPhase)
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

    private void StartInitialization() { SetGamePhase(GamePhase.BasicTutorial_Start); Debug.Log("Finished GamePhase.Initialization Phase"); }

    private void StartBasicTutorialStart()
    {
        // Spawn the player and nexus on the map
        Debug.Log("Starting GamePhase.BasicTutorial_Start Phase");
        Player.SetActive(true);
        Nexus.SetActive(true);

        Player.GetComponent<PlayerController>().LockShooting();
        Player.GetComponent<PlayerController>().LockMovement();
        //Player.GetComponent<PlayerLevels>().LockRewardUI();

        WaveManager.LockAllEnemiesMovement();
        UIManager.DeactivateInventoryUI();
        UIManager.DeactivateEXPUI();

        Debug.Log("Ending GamePhase.BasicTutorial_Start Phase");
        SetGamePhase(GamePhase.BasicTutorial_Movement);
    }

    private void StartMovementTutorial()
    {
        Debug.Log("Starting GamePhase.BasicTutorial_Movement Phase");
        StartCoroutine(WaitForMovementInput());
    }

    private IEnumerator WaitForMovementInput()
    {
        yield return new WaitForSeconds(2.0f); // free movement for 3 seconds
        UIManager.Tutorial_ShowMovementUI(); // Shows the Animated 4 WASD KEys
        Player.GetComponent<PlayerController>().UnlockMovement();
        yield return new WaitUntil(() => PlayerController.HasPressedMovementKeys());        
        yield return new WaitForSeconds(3.0f); // free movement for 3 seconds
        UIManager.Tutorial_HideMovementUI(); // hide WASD movement keys
        Debug.Log("Ending GamePhase.BasicTutorial_Movement Phase");
        SetGamePhase(GamePhase.BasicTutorial_Shooting);
    }

    private void StartShootingTutorial()
    {
        Debug.Log("Starting GamePhase.BasicTutorial_Shooting Phase");
        WaveManager.SpawnSingleEnemy();
        StartCoroutine(WaitForShootingInput());
    }

    private IEnumerator WaitForShootingInput()
    {
        yield return new WaitForSeconds(10.0f); // delay 10 seconds for enemy to come close to nexus
        WaveManager.LockAllEnemiesMovement();
        WaveManager.SetConstantXPDrops(4);
        Player.GetComponent<PlayerController>().LockMovement();

        UIManager.Tutorial_ShowShootingUI(); // show shooting UI
        Player.GetComponent<PlayerController>().UnlockShooting();

        // continue when player has shot + all enemies are dead
        yield return new WaitUntil(() => PlayerController.HasShotOnce() && WaveManager.AllEnemiesKilled());

        UIManager.Tutorial_HideShootingUI(); // hide shooting UI
        Debug.Log("Ending GamePhase.BasicTutorial_Shooting Phase");
        SetGamePhase(GamePhase.BasicTutorial_XP);
    }

    private void StartXPTutorial()
    {
        Debug.Log("Starting GamePhase.BasicTutorial_XP Phase");
        UIManager.ActivateEXPUI();
        UIManager.Tutorial_ShowXPUI(new Vector3(12.0f, 1.0f, 0.0f)); // show xp UI
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

        Player.GetComponent<PlayerController>().LockMovement();
        Player.GetComponent<PlayerController>().LockShooting();
        //StartCoroutine(WaitForPlacementInput());

        Debug.Log("Ending GamePhase.BasicTutorial_Placement Phase");
        SetGamePhase(GamePhase.HandCraftedWaves);
    }

    //private IEnumerator WaitForPlacementInput()
    //{
    //    // wait until player has "PICKED UP"
    //    //yield return new WaitUntil(() => PlaceObject.turretPickedUp());

    //    // UIManager.Tutorial_HideInventoryBouncingArrow; // hide

    //    // wait until player has "PLACED" turret
    //    //yield return new WaitUntil(() => PlaceObject.firstTurretPlaced());

    //    // ngl if they cancel we're screwed bc i dont want to write a while loop for that here! 

    //    //yield return new WaitForSeconds(4.0f); // delay 4 seconds


    //    Debug.Log("Ending GamePhase.BasicTutorial_Placement Phase");
    //    SetGamePhase(GamePhase.HandCraftedWaves);
    //}

    private void StartHandCraftedWaves()
    {
        Debug.Log("ENTERING GamePhase.HandCraftedWaves Phase");
        Player.GetComponent<PlayerController>().UnlockMovement();
        Player.GetComponent<PlayerController>().UnlockShooting();
        WaveManager.UnlockAllEnemiesMovement();
    }

    private void StartDynamicWaves()
    {
        Debug.Log("ENTERING GamePhase.DynamicWaves Phase");
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
        DEBUG_ORE_MANAGER = true;
        DEBUG_INVENTORY_MANAGER = false;
    }

    void Start()
    {
        // retrieve Tutorial Value
        bool TutorialStorageValue = PlayerPrefs.GetInt(TutorialStorageKey, 1) == 1;
        Time.timeScale = 1.0f;

        // Tutorial Logic
        if (TutorialStorageValue && enableTutorial)
        {
            Debug.Log("Tutorial is Enabled");
            SetGamePhase(GamePhase.BasicTutorial_Start);
        }
        else
        {
            SetGamePhase(CurrentPhase);
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
}
