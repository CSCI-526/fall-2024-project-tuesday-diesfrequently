using System;
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

    [SerializeField] public GameObject Player;
    [SerializeField] public GameObject Nexus;
    [SerializeField] public GameObject Barrier;

    public bool useBulletPool = false;
    public bool enableTutorial = true;
    private bool isPaused = false;
    public bool modalAcknowleged = false;
    private bool isCameraTransitionDone = false;
    public bool inTutorialDeath = false;

    // tracks current phase
    [SerializeField] public GamePhase StartPhase;
    [SerializeField] public bool IsTutorialEnabled = false;

    [SerializeField] private GamePhase CurrentPhase;

    public GamePhase currentPhase
    {
        get { return CurrentPhase; }
        private set { CurrentPhase = value; }
    }

    [SerializeField] public bool DEBUG_WAVE_MANAGER;
    [SerializeField] public bool DEBUG_ORE_MANAGER;
    [SerializeField] public bool DEBUG_INVENTORY_MANAGER;
    [SerializeField] public bool DEBUG_REWARD_PANEL;
    [SerializeField] public bool DEBUG_GAME_MANAGER;
    [SerializeField] public bool DEBUG_CAMERA_FOLLOW;
    [SerializeField] public bool DEBUG_UI_MANAGER;
    [SerializeField] public bool DEBUG_ANALYTICS_MANAGER;

    // Set the field where TutorialStorageValue is stored
    private const string TutorialStorageKey = "IsTutorialEnabled";
    Vector3 origNexusPos;

    // states for GamePhase Tutorialization
    public enum GamePhase
    {
        Initialization,
        P1_Setup_Tutorial,
        P1_TakeDamage_Tutorial,
        P1_Movement_Tutorial,
        P1_Dodging_Tutorial,
        P1_Shooting_Tutorial,
        P1_XP_Collection_Tutorial,
        P2_Setup_Tutorial,
        BasicTutorial_Reward,
        BasicTutorial_Placement,
        HandCraftedWaveSetup,
        HandCraftedWaves,
        AdvancedTutorial,
        DynamicWaves,
        P1_Tutorial_Death
    }

    public void SetGamePhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        switch (currentPhase)
        {
            case GamePhase.Initialization:
                StartUIInitialization();
                break;
            case GamePhase.P1_Setup_Tutorial:
                SetupTutorial();
                break;
            case GamePhase.P1_TakeDamage_Tutorial:
                StartTakeDamageTutorial();
                break;
            case GamePhase.P1_Movement_Tutorial:
                StartMovementTutorial();
                break;
            case GamePhase.P1_Dodging_Tutorial:
                StartDodgingTutorial();
                break;
            case GamePhase.P1_Shooting_Tutorial:
                StartShootingTutorial();
                break;
            case GamePhase.P1_XP_Collection_Tutorial:
                StartXPTutorial();
                break;
            case GamePhase.P2_Setup_Tutorial:
                SetupTutorialPart2();
                break;
            case GamePhase.BasicTutorial_Reward:
                StartRewardTutorial();
                break;
            case GamePhase.BasicTutorial_Placement:
                StartPlacementTutorial();
                break;
            case GamePhase.HandCraftedWaveSetup:
                SetupHandCraftedWaves();
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

    private void HideExtraUIComponents()
    {
        UIManager.HideMinimap();
        UIManager.HideNexusHealthSlider();
        UIManager.HidePlayerHealthSlider();
        UIManager.HideInventoryUI();
        UIManager.HideEXPSlider();
        UIManager.HideWaveUI();
        UIManager.Tutorial_HideMovementUI();
        UIManager.HideRewardUIMask();
    }

    private void ResetGlobalTutorialFlags()
    {
        PlayerController.SetShotOnceFalse();
        UIManager.ResetRewardsScreenEnded();
        InventoryManager.ResetFirstGoldCollected();
        InventoryManager.ResetFirstGoldScouted();
        PlaceObject.ResetFirstTurretPlaced();
        PlaceObject.ResetFirstHarvesterPlaced();
        Player.GetComponent<PlayerLevels>().ResetIsLevelTwo();
        Player.GetComponent<PlayerLevels>().ResetTurretFirstUpgrade();
        Nexus.GetComponent<SpawnAnimation>().ResetIsNexusInSpawnPos();

        modalAcknowleged = false;
        isCameraTransitionDone = false;
        inTutorialDeath = false;
    }

    private void createArrowforXPOrb()
    {
        GameObject[] experienceOrbs = GameObject.FindGameObjectsWithTag("ExperienceOrb");

        foreach (GameObject exp_orb in experienceOrbs)
        {
            if (exp_orb == null) break;
            Player.GetComponent<PlayerLevels>().markXPNotCollected();
            Vector3 exp_orb_pos = exp_orb.transform.position;
            UIManager.Tutorial_ShowXPUI(exp_orb_pos); // show xp UI Arrow
            break;
        }
    }

    // Helper Re-Usables CoRoutines
    private IEnumerator BufferNextPhaseStart(GamePhase new_phase, string curr_phase, string next_phase, float buffer_amt = 0.5f)
    {
        yield return new WaitForSeconds(buffer_amt); // 0.5s buffer
        if (DEBUG_GAME_MANAGER) Debug.Log($"[PHASE] {curr_phase} (END)");
        if (DEBUG_GAME_MANAGER) Debug.Log("[Tutorial Death Value in Buffer]: " + inTutorialDeath);
        if (inTutorialDeath) yield break;
        else
        {
            SetGamePhase(new_phase);
            if (DEBUG_GAME_MANAGER) Debug.Log($"[PHASE] {next_phase} (START)");
        }
    }

    public IEnumerator WaitForModalAcknowlegement(int modal_type, string msg, bool isRespawnScreen = false)
    {
        modalAcknowleged = false;
        //if (DEBUG_GAME_MANAGER) Debug.Log("[Modal Window] In Modal CoRoutine");
        if (inTutorialDeath && !isRespawnScreen) yield break;
        else
        {
            //if (DEBUG_GAME_MANAGER) Debug.Log("[Modal Window] Setting Up Modal Window");
            UIManager.ConfigModalWindow(modal_type);
            //if (DEBUG_GAME_MANAGER) Debug.Log("[Modal Window] CONFIG Done");
            //UIManager.ShowRewardUIMask();
            UIManager.HideMinimap();
            UIManager.ShowModalWindow(msg);
            //if (DEBUG_GAME_MANAGER) Debug.Log("[Modal Window] Instruction Message Set");
            yield return new WaitForSeconds(1.0f);
            //if (DEBUG_GAME_MANAGER) Debug.Log("[Modal Window] Rudimentary Wait Time Complete");
        }

        //if (DEBUG_GAME_MANAGER) Debug.Log("[inTutorialDeath] " + inTutorialDeath + " and [isRespawnScreen] " + isRespawnScreen);

        if (DEBUG_GAME_MANAGER) Debug.Log("[Modal Window] Waiting for Acknowledgement for msg: " + msg);

        if (inTutorialDeath && !isRespawnScreen) yield break;
        //yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (inTutorialDeath && !isRespawnScreen)
            {
                if (DEBUG_GAME_MANAGER) Debug.Log("[Wait for Modal] Skipped due to Tutorial Death");
                yield break;
            }
            yield return null; // Wait for the next frame
        }

        modalAcknowleged = true;
        UIManager.HideModalWindow();
        if (currentPhase == GamePhase.Initialization ||
            currentPhase == GamePhase.P1_Setup_Tutorial ||
            currentPhase == GamePhase.P1_TakeDamage_Tutorial ||
            currentPhase == GamePhase.P1_Movement_Tutorial ||
            currentPhase == GamePhase.P1_Dodging_Tutorial)
        {
            UIManager.HideMinimap();
        } else { UIManager.ShowMinimap(); }
        
        if (inTutorialDeath && !isRespawnScreen) yield break;
    }

    private IEnumerator MoveCameraToTargetX(GameObject entity, float transition_time, IEnumerator NextCoroutine = null)
    {
        isCameraTransitionDone = false;
        Player.GetComponent<PlayerController>().LockMovement();
        MainCamera.GetComponent<CameraFollow>().SetNewTarget(entity, transition_time, OnCameraTransitionComplete);
        //Debug.Log("[Game Manager] CAMERA: Waiting for transition...");
        yield return new WaitUntil(() => isCameraTransitionDone);
        //Debug.Log("[Game Manager] CAMERA: Transition complete!");

        if (NextCoroutine == null) { }
        else { StartCoroutine(NextCoroutine); }

    }

    private void OnCameraTransitionComplete()
    {
        //Debug.Log("[Game Manager] CAMERA: Transition completed!");
        isCameraTransitionDone = true;
        //Debug.Log("[Game Manager] CAMERA: Set isCameraTransitionDone to TRUE");
    }

    ////////////////////////
    // TUTORIAL FUCNTIONS //
    ////////////////////////

    private void StartUIInitialization(GamePhase origin_phase = GamePhase.Initialization)
    {
        if (DEBUG_GAME_MANAGER) Debug.Log("[Start UI] Initialization Tasks");

        RespawnPlayer(origin_phase);
        
    }

    

    public void RespawnPlayer(GamePhase death_phase, bool isPauseMenuRestart = false)
    {
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Respawning Player");

        HideExtraUIComponents();
        if (DEBUG_GAME_MANAGER) Debug.Log("[Respawn] Hiding UI Components");
        UIManager.HideModalWindow();
        if (DEBUG_GAME_MANAGER) Debug.Log("[Respawn] Hiding Modal Window");
        WaveManager.KillAllEnemyEntities();

        // Activate Correct Cursor to use
        UIManager.ActivateCustomCursor();

        Player.GetComponent<PlayerController>().LockShooting();
        Player.GetComponent<PlayerController>().LockMovement();

        // MATCHA: Set Nexus to Original Position
        Nexus.transform.position = Nexus.GetComponent<SpawnAnimation>().startPosition;
        if (DEBUG_GAME_MANAGER) Debug.Log("[Respawn] Set Nexus OG Position");

        ResetGlobalTutorialFlags();

        UIManager.ResetDamageEffect();

        if (currentPhase == GamePhase.Initialization && !inTutorialDeath)
        {
            if (isPauseMenuRestart)
            {
                if (DEBUG_GAME_MANAGER) Debug.Log("[Respawn] Starting RESTART from Init Phase");
                StartCoroutine(BufferNextPhaseStart(GamePhase.P1_Setup_Tutorial, "[RESTART] " + death_phase, "Respawn - Initialization", 1.0f));
            }
            else
            {
                if (DEBUG_GAME_MANAGER) Debug.Log("[Respawn] Starting from Init Phase");
                StartCoroutine(BufferNextPhaseStart(GamePhase.P1_Setup_Tutorial, death_phase.ToString(), "Respawn - Initialization", 1.0f));
            }
        }
        else return;
    }

    private void SetupTutorial()
    {
        // Setup Small Barrier
        EnableBarrier();
        SetSmallBarrier();

        // Activate Player State 1
        Player.SetActive(true);
        Player.GetComponent<PlayerController>().LockMovement();
        Player.GetComponent<PlayerController>().LockShooting();
        Player.transform.position = new Vector3(1.5f, 1.0f, 1.5f);

        string modal_msg = "Welcome to the tutorial for\n<color=red>Interstellar Pest Control</color>!";
        StartCoroutine(WaitForModalAcknowlegement(1, modal_msg));
        StartCoroutine(OriginMessage());
    }

    private IEnumerator OriginMessage()
    {
        yield return new WaitUntil(() => modalAcknowleged); // wait for modal windows to be acknowledged
        modalAcknowleged = false; // set messageAck to false again

        // State Change 
        if (currentPhase == GamePhase.P1_Setup_Tutorial && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.P1_TakeDamage_Tutorial, "P1_Setup_Tutorial", "P1_TakeDamage_Tutorial"));
        }
        else yield break;
    }

    private void StartTakeDamageTutorial()
    {
        WaveManager.SpawnSingleEnemy("tutorial", Player.transform.position, 6.0f);
        WaveManager.LockAllEnemiesMovement();

        string modal_msg = "<color=red>ENEMIES</color> are dangerous!\nTouching them will damage your <color=#90d5ff>HEALTH</color>!";
        StartCoroutine(WaitForModalAcknowlegement(1, modal_msg));
        StartCoroutine(ContinueTakeDamageTutorial());
    }

    private IEnumerator ContinueTakeDamageTutorial()
    {
        yield return new WaitUntil(() => modalAcknowleged); // wait for modal windows to be acknowledged
        if (inTutorialDeath) yield break;
        modalAcknowleged = false; // set messageAck to false agains
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Modal Acknowledged");

        UIManager.ShowPlayerHealthSlider();
        WaveManager.UnlockAllEnemiesMovement();

        yield return new WaitForSeconds(3.0f); // player takes 3 hits of dmg

        WaveManager.LockAllEnemiesMovement();

        Player.GetComponent<PlayerController>().OffsetPlayerPos(new Vector3(-2.0f, 0.0f, -2.0f));// offset player slightly
        yield return new WaitForSeconds(0.1f); // player takes 3 hits of dmg
        // State Change
        if (currentPhase == GamePhase.P1_TakeDamage_Tutorial && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.P1_Movement_Tutorial, "P1_TakeDamage_Tutorial", "P1_Movement_Tutorial"));
        }
        else yield break;

    }

    private void StartMovementTutorial()
    {
        UIManager.Tutorial_ShowMovementUI(); // Shows the Animated 4 WASD Keys
        StartCoroutine(WaitForMovementInput());
    }

    private IEnumerator WaitForMovementInput()
    {
        if (inTutorialDeath) yield break;
        Player.GetComponent<PlayerController>().UnlockMovement();

        yield return new WaitForSeconds(1.0f); // buffer 1s (no accidental press)
        if (inTutorialDeath) yield break;
        yield return new WaitUntil(() => PlayerController.HasPressedMovementKeys());
        if (inTutorialDeath) yield break;
        else StartCoroutine(ContinueMovementTutorial());
    }

    private IEnumerator ContinueMovementTutorial()
    {
        yield return new WaitForSeconds(2.0f); // free movement for 3 seconds 
        UIManager.Tutorial_HideMovementUI(); // hide WASD movement keys

        // State Change
        if (currentPhase == GamePhase.P1_Movement_Tutorial && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.P1_Dodging_Tutorial, "P1_Movement_Tutorial", "P1_Dodging_Tutorial"));
        }
        else yield break;
    }

    private void StartDodgingTutorial()
    {
        string modal_msg = "Avoid the <color=red>ENEMIES</color>!";
        StartCoroutine(WaitForModalAcknowlegement(1, modal_msg));
        StartCoroutine(ContinueDodgingTutorial());
    }

    private IEnumerator ContinueDodgingTutorial()
    {
        yield return new WaitUntil(() => modalAcknowleged); // wait for modal windows to be acknowledged
        if (inTutorialDeath) yield break;
        modalAcknowleged = false; // set messageAck to false agains
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Modal Acknowledged");

        if (inTutorialDeath) yield break;
        WaveManager.UnlockAllEnemiesMovement();
        WaveManager.EnablePlayerTargetOnly();
        yield return new WaitForSeconds(3.0f);

        if (inTutorialDeath) yield break;
        WaveManager.SpawnSingleEnemy("tutorial", Player.transform.position, 6.0f);
        WaveManager.EnablePlayerTargetOnly();
        yield return new WaitForSeconds(4.0f);

        if (inTutorialDeath) yield break;
        WaveManager.SpawnSingleEnemy("tutorial", Player.transform.position, 6.0f);
        WaveManager.EnablePlayerTargetOnly();
        yield return new WaitForSeconds(4.0f);

        if (inTutorialDeath) yield break;

        // Clean Up Current Enemies
        WaveManager.LockAllEnemiesMovement();
        WaveManager.KillAllEnemyEntities(true); // kill the enemies that we were dodging
        yield return new WaitUntil(() => WaveManager.AllEnemiesKilled()); // wait for all enemies to die before moving on
        if (inTutorialDeath) yield break;

        Player.GetComponent<PlayerController>().LockMovement();

        // State Change
        if (currentPhase == GamePhase.P1_Dodging_Tutorial && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.P1_Shooting_Tutorial, "P1_Dodging_Tutorial", "P1_Shooting_Tutorial", 2.0f));
        }
        else yield break;

    }

    private void StartShootingTutorial()
    {
        UIManager.ShowMinimap();
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 12.0f, 1);
        WaveManager.LockAllEnemiesMovement();
        WaveManager.EnablePlayerTargetOnly();
        StartCoroutine(MoveCameraToTargetX(WaveManager.AllEnemyEntities[0], 1.0f, PlayEnemyWalkAnimation()));
        //StartCoroutine(PlayEnemyWalkAnimation());
    }

    private IEnumerator PlayEnemyWalkAnimation()
    {
        WaveManager.UnlockAllEnemiesMovement();
        yield return new WaitForSeconds(4.5f); // delay 4.5 seconds for enemy to come close to nexus
        if (inTutorialDeath) yield break;
        WaveManager.LockAllEnemiesMovement();

        StartCoroutine(MoveCameraToTargetX(Player, 1.0f, ContinueShootingTutorial()));
    }

    private IEnumerator ContinueShootingTutorial()
    {
        Debug.Log("[Shooting Tutorial] Entered ContinueShootingTutorial");
        if (inTutorialDeath) yield break;
        Player.GetComponent<PlayerController>().UnlockMovement();

        UIManager.ActivateCustomShootingCursor(); // Show Custom Shooting Cursor
        Player.GetComponent<PlayerController>().UnlockShooting();

        yield return new WaitUntil(() => PlayerController.HasShotOnce() && WaveManager.AllEnemiesKilled());
        Debug.Log("[Shooting Tutorial] Broke Shooting Flag");
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }

        // State Change
        if (currentPhase == GamePhase.P1_Shooting_Tutorial && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.P1_XP_Collection_Tutorial, "P1_Shooting_Tutorial", "P1_XP_Collection_Tutorial"));
        }
        else yield break;
    }

    private void StartXPTutorial()
    {
        UIManager.ActivateEXPUI();
        StartCoroutine(ContinueShootingTutorial2());
    }

    private IEnumerator ContinueShootingTutorial2()
    {
        createArrowforXPOrb();
        yield return new WaitUntil(() => Player.GetComponent<PlayerLevels>().isXPOrbCollected);

        Debug.Log("[Shooting Tutorial] Entered ContinueShootingTutorial_2");
        yield return new WaitForSeconds(2.0f);
        if (inTutorialDeath) yield break;
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 8.0f, 1);
        WaveManager.EnablePlayerTargetOnly();
        PlayerController.SetShotOnceFalse(); // reset flag for next WaitUntil()
        yield return new WaitUntil(() => PlayerController.HasShotOnce() && WaveManager.AllEnemiesKilled());
        createArrowforXPOrb();

        Debug.Log("[Shooting Tutorial] Broke Shooting Flag 2");
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }

        Debug.Log("[Shooting Tutorial] Entered ContinueShootingTutorial_2");
        yield return new WaitForSeconds(3.0f);
        if (inTutorialDeath) yield break;
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 6.0f, 1);
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 4.0f, 0);
        WaveManager.EnablePlayerTargetOnly();
        PlayerController.SetShotOnceFalse(); // reset flag for next WaitUntil()
        yield return new WaitUntil(() => PlayerController.HasShotOnce() && WaveManager.AllEnemiesKilled());
        Debug.Log("[Shooting Tutorial] Broke Shooting Flag 2");
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }
        createArrowforXPOrb();
        yield return new WaitUntil(() => Player.GetComponent<PlayerLevels>().isXPOrbCollected);
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }

        UIManager.ActivateCustomCursor(); // show shooting Crosshair

        // State Change
        if (currentPhase == GamePhase.P1_XP_Collection_Tutorial && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.P2_Setup_Tutorial, "P1_XP_Collection_Tutorial", "P2_Setup_Tutorial", 2.0f));
        }
        else yield break;

    }

    private void SetupTutorialPart2()
    {
        StartCoroutine(SpawnNexus());
    }

    private IEnumerator SpawnNexus()
    {
        // Reset Player Pos + Lock Movement
        if (inTutorialDeath) yield break;
        Player.transform.position = new Vector3(-3.0f, 1.0f, -3.0f);
        Player.GetComponent<PlayerController>().LockMovement();
        Player.GetComponent<PlayerController>().LockShooting();

        // Set Barrier Medium Size
        SetMediumBarrier();

        // Show Nexus Animation
        Nexus.GetComponent<SpawnAnimation>().TriggerSpawnSequence();
        yield return new WaitUntil(() => Nexus.GetComponent<SpawnAnimation>().isNexusInSpawnPos());
        if (inTutorialDeath) yield break;

        string modal_msg = "This is your <color=#90d5ff>NEXUS</color>! CAREFUL...\n<color=red>ENEMIES</color> will attack & damage your <color=#90d5ff>NEXUS</color>!";
        StartCoroutine(WaitForModalAcknowlegement(1, modal_msg));
        yield return new WaitUntil(() => modalAcknowleged); // wait for modal windows to be acknowledged
        if (inTutorialDeath) yield break;
        modalAcknowleged = false; // set messageAck to false again
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Modal Acknowledged");

        UIManager.ShowNexusHealthSlider();

        if (inTutorialDeath) yield break;
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 5.0f, 0);
        WaveManager.LockAllEnemiesMovement();
        StartCoroutine(MoveCameraToTargetX(WaveManager.AllEnemyEntities[0], 1.0f, SpawnKamikazeEnemies()));
    }
    private IEnumerator SpawnKamikazeEnemies()
    {
        WaveManager.UnlockAllEnemiesMovement();
        yield return new WaitUntil(() => WaveManager.AllEnemiesKilled());
        if (inTutorialDeath) yield break;
        Debug.Log("[Shooting Tutorial] Broke Shooting Flag 2");
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }

        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 7.0f, 0);
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 8.0f, 0);
        WaveManager.LockAllEnemiesMovement();
        StartCoroutine(MoveCameraToTargetX(WaveManager.AllEnemyEntities[0], 1.0f, SpawnKamikazeEnemies2()));
    }
    private IEnumerator SpawnKamikazeEnemies2()
    {
        WaveManager.UnlockAllEnemiesMovement();
        yield return new WaitUntil(() => WaveManager.AllEnemiesKilled());
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }
        Debug.Log("[Shooting Tutorial] Broke Shooting Flag 2");

        string modal_msg = "Shoot the <color=red>ENEMIES</color> to protect <color=#90d5ff>YOUR NEXUS</color>!\n Collect the <color=#00FF15>experience</color> orbs they drop to level up!";
        StartCoroutine(WaitForModalAcknowlegement(1, modal_msg));
        yield return new WaitUntil(() => modalAcknowleged); // wait for modal windows to be acknowledged
        if (inTutorialDeath) yield break;
        modalAcknowleged = false; // set messageAck to false again
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Modal Acknowledged");
        UIManager.ActivateCustomShootingCursor(); // sets CustomCursor
        Player.GetComponent<PlayerController>().UnlockMovement();
        Player.GetComponent<PlayerController>().UnlockShooting();

        yield return new WaitForSeconds(1.0f);
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 10.0f, 0);
        yield return new WaitForSeconds(1.0f);
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 11.0f, 0);
        yield return new WaitForSeconds(1.0f);
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }
        WaveManager.SpawnSingleEnemy("shoot_tutorial", Player.transform.position, 12.0f, 1);
        yield return new WaitUntil(() => WaveManager.AllEnemiesKilled());
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }
        Debug.Log("[Shooting Tutorial] Broke Shooting Flag 2");
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }

        createArrowforXPOrb();
        yield return new WaitUntil(() => Player.GetComponent<PlayerLevels>().isXPOrbCollected);
        Debug.Log("[Shooting Tutorial] Broke Shooting Flag 2");
        if (inTutorialDeath) { WaveManager.SetConstantXPDrops(0); WaveManager.KillAllEnemyEntities(); yield break; }

        // State Change
        if (currentPhase == GamePhase.P2_Setup_Tutorial && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.BasicTutorial_Reward, "P2_Setup_Tutorial", "BasicTutorial_Reward", 2.0f));
        }
        else yield break;
    }

    private void StartRewardTutorial()
    {
        if (currentPhase == GamePhase.BasicTutorial_Reward && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.BasicTutorial_Placement, "BasicTutorial_Reward", "BasicTutorial_Placement", 2.0f));
        }
        else Debug.Log("[Game Manager] Reward Error!!!!!"); return;
    }

    private void StartPlacementTutorial()
    {
        WaveManager.LockAllEnemiesMovement();
        StartCoroutine(WaitForLevelTwo());
    }

    private IEnumerator WaitForLevelTwo()
    {
        yield return new WaitUntil(() => Player.GetComponent<PlayerLevels>().isLevelTwo());
        if (inTutorialDeath) yield break;
        Player.GetComponent<PlayerController>().LockMovement();
        Player.GetComponent<PlayerController>().LockShooting();
        UIManager.ActivateCustomCursor(); // sets CustomCursor

        // wait until player has "PLACED" turret
        yield return new WaitUntil(() => PlaceObject.firstTurretPlaced());
        if (inTutorialDeath) yield break;
        WaveManager.UnlockAllEnemiesMovement();

        string modal_msg = "GOOD LUCK!\nStarting <color=red>[Wave 1]</color>!";
        StartCoroutine(WaitForModalAcknowlegement(1, modal_msg));
        yield return new WaitUntil(() => modalAcknowleged); // wait for modal windows to be acknowledged
        if (inTutorialDeath) yield break;
        modalAcknowleged = false; // set messageAck to false again
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Modal Acknowledged");

        // State Change
        if (currentPhase == GamePhase.BasicTutorial_Placement && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.HandCraftedWaves, "BasicTutorial_Placement", "HandCraftedWaves", 0.5f));
        }
        else yield break;
    }

    private void SetupHandCraftedWaves()
    {
        UIManager.ShowNexusHealthSlider();
        UIManager.ShowPlayerHealthSlider();
        UIManager.ActivateInventoryUI();
        UIManager.ShowEXPSlider();
        UIManager.ShowWaveUI();
        UIManager.Tutorial_HideMovementUI();
        UIManager.ShowMinimap();

        Nexus.transform.position = Nexus.GetComponent<SpawnAnimation>().endPosition;

        UIManager.HideModalWindow();
        WaveManager.KillAllEnemyEntities();

        // Activate Correct Cursor to use
        UIManager.ActivateShootingCursor();

        ResetGlobalTutorialFlags();

        // State Change
        if (currentPhase == GamePhase.HandCraftedWaveSetup && !inTutorialDeath)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.HandCraftedWaves, "HandCraftedWaveSetup", "HandCraftedWaves", 0.50f));
        }
        else Debug.Log("Problem with Hand Crafted Wave Setup"); return;
    }

    private void StartHandCraftedWaves()
    {
        DisableBarrier();
        UIManager.ActivateShootingCursor(); // sets CustomCursor
        Player.GetComponent<PlayerController>().UnlockMovement();
        Player.GetComponent<PlayerController>().UnlockShooting();
        Player.GetComponent<PlayerHealth>().ResetPlayerHealth();
        Nexus.GetComponent<Nexus>().ResetNexusHealth();

        UIManager.ShowWaveUI();
        WaveManager.UnlockAllEnemiesMovement();
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

        // Instance.Player = GameObject.FindGameObjectWithTag("Player");
        // Instance.Nexus = GameObject.FindGameObjectWithTag("Nexus");

        // Serialized Game Objects
        if (Instance.Player == null) Debug.LogError("ERROER EREORORER PLAYER");
        if (Instance.Nexus == null) Debug.LogError("ERROER EREORORER NEXUS");

        // Script Components
        Instance.WaveManager = GetComponent<WaveManager>();
        Instance.UIManager = GetComponent<UIManager>();
        Instance.InventoryManager = GetComponent<InventoryManager>();
        Instance.AnalyticsManager = GetComponent<AnalyticsManager>();
        Instance.OreManager = GetComponent<OreManager>();

        // Activate "Hidden Nexus"
        Nexus.SetActive(true);
        origNexusPos = Nexus.transform.position;

        //// Global DEBUG Values
        //DEBUG_WAVE_MANAGER = false;
        //DEBUG_ORE_MANAGER = false;
        //DEBUG_INVENTORY_MANAGER = true;
        //DEBUG_REWARD_PANEL = false;
        //DEBUG_GAME_MANAGER = true; 
    }

    private void OnEnable()
    {
        Player.GetComponent<PlayerHealth>().Tutorial_OnDeath += OnPlayerTutorialDeath;
        InventoryManager.Tutorial_OnFirstHarvester += OnFirstHarvesterAquirement;
        InventoryManager.Tutorial_OnFirstGold += OnGoldCoinDiscovery;
    }

    private void OnDisable()
    {
        Player.GetComponent<PlayerHealth>().Tutorial_OnDeath -= OnPlayerTutorialDeath;
        InventoryManager.Tutorial_OnFirstHarvester -= OnFirstHarvesterAquirement;
        InventoryManager.Tutorial_OnFirstGold -= OnGoldCoinDiscovery;
    }

    private void OnGoldCoinDiscovery(GameObject gold_orb)
    {
        StartCoroutine(OnGoldDiscovery(gold_orb));
    }

    private IEnumerator OnGoldDiscovery(GameObject gold_orb)
    {
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] First Gold Coin has Spawned");

        Vector3 orb_pos = gold_orb.transform.position;
        UIManager.Tutorial_ShowXPUI(orb_pos);

        // Stop All Actions
        LockAllTurretShooting();
        WaveManager.LockAllEnemiesMovement();
        Player.GetComponent<PlayerController>().LockShooting();
        Player.GetComponent<PlayerController>().UnlockMovement();
        //yield return new WaitForSeconds(0.5f);
        //Player.GetComponent<PlayerController>().LockMovement();

        modalAcknowleged = false;
        string modal_msg = "Collect <color=yellow>GOLD</color> to upgrade your turrets!";
        StartCoroutine(WaitForModalAcknowlegement(1, modal_msg));

        
        // wait till gold ore collected 
        yield return new WaitUntil(() => modalAcknowleged && InventoryManager.isFirstGoldCollected()); // wait for modal windows to be acknowledged

        modalAcknowleged = false; // set messageAck to false against
        Debug.Log("[Tutorial Gold] Modal Acknowledged for : " + modal_msg);
        Debug.Log("[Tutorial Gold] Gold Collection Acknowledged FOR");

        Player.GetComponent<PlayerController>().UnlockShooting();
        Player.GetComponent<PlayerController>().UnlockMovement();
        WaveManager.UnlockAllEnemiesMovement();
        UnlockAllTurretShooting();
        UIManager.ActivateShootingCursor();
    }

    private void OnPlayerTutorialDeath(GamePhase death_phase)
    {
        StartCoroutine(OnTutorialDeath(death_phase));
    }

    private void OnFirstHarvesterAquirement()
    {
        StartCoroutine(OnFirstHarvester());
    }

    private void LockAllTurretShooting()
    {
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject turret in turrets)
        {
            if (turret == null) break;
            turret.GetComponent<turretShoot>().DisableShooting();
        }
    }

    private void UnlockAllTurretShooting()
    {
        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Turret");
        foreach (GameObject turret in turrets)
        {
            if (turret == null) break;
            turret.GetComponent<turretShoot>().EnableShooting();
        }
    }

    private IEnumerator OnFirstHarvester()
    {
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Entering First Harvestor");

        LockAllTurretShooting();
        WaveManager.LockAllEnemiesMovement();
        Player.GetComponent<PlayerController>().LockShooting();
        yield return new WaitForSeconds(0.5f);
        Player.GetComponent<PlayerController>().LockMovement();

        UIManager.ShowSelectGunTutorial();

        modalAcknowleged = false;
        string modal_msg = "<color=#808080>HARVESTERS</color> generate\n<color=yellow>GOLD</color> for powerful\n turret upgrades!";
        StartCoroutine(WaitForModalAcknowlegement(1, modal_msg));
        yield return new WaitUntil(() => modalAcknowleged && PlaceObject.firstHarvesterPlaced()); // wait for modal windows to be acknowledged
        modalAcknowleged = false; // set messageAck to false against
        Debug.Log("[Tutorial Harvester] Modal Acknowledged for : " + modal_msg);
        Debug.Log("[Tutorial Harvester] Harvestor PlaceBool Acknowledged FOR");

        Player.GetComponent<PlayerController>().UnlockShooting();
        Player.GetComponent<PlayerController>().UnlockMovement();
        WaveManager.UnlockAllEnemiesMovement();
        UnlockAllTurretShooting();
        UIManager.ActivateShootingCursor();
    }

    private IEnumerator OnTutorialDeath(GamePhase death_phase)
    {
        if (inTutorialDeath)
        {
            Debug.Log("[Tutorial Death] Already in tutorial death state. Ignoring player death.");
            yield break; // in tutorial death, shouldnt be in here twice
        }

        StartCoroutine(BufferNextPhaseStart(GamePhase.P1_Tutorial_Death, death_phase.ToString(), "P1_Tutorial_Death", 0.0f));

        yield return new WaitUntil(() => currentPhase == GamePhase.P1_Tutorial_Death); // wait for state change
        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Entering Tutorial Death");
        inTutorialDeath = true;
        UIManager.ActivateCustomCursor();

        // Code to End all other WaitUntil()
        WaveManager.LockAllEnemiesMovement();
        Player.GetComponent<PlayerController>().LockShooting();
        WaveManager.KillAllEnemyEntities(true);
        PlayerController.SetShotOnceTrue();
        HideExtraUIComponents();

        yield return new WaitForSeconds(2.0f);
        Player.GetComponent<PlayerController>().LockMovement();

        modalAcknowleged = false;
        string modal_msg = "<color=red>YOU DIED</color> :(\n<color=#90d5ff>Respawning...</color>";
        StartCoroutine(WaitForModalAcknowlegement(2, modal_msg, true));

        yield return new WaitUntil(() => modalAcknowleged); // wait for modal windows to be acknowledged
        modalAcknowleged = false; // set messageAck to false against
        if (DEBUG_GAME_MANAGER) Debug.Log("[Tutorial Death] Modal Acknowledged for : " + modal_msg);

        Player.GetComponent<PlayerHealth>().ResetPlayerHealth();

        if (DEBUG_GAME_MANAGER) Debug.Log("[Game Manager] Exiting Tutorial Death");
        inTutorialDeath = false;

        if (currentPhase == GamePhase.P1_Tutorial_Death)
        {
            StartCoroutine(BufferNextPhaseStart(GamePhase.Initialization, "P1_Tutorial_Death", "Initialization", 0.5f));
        }
        else Debug.Log("[Game Manager] ERRROERIRJEOIJR");
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
            IsTutorialEnabled = true;
            currentPhase = GamePhase.Initialization;
            SetGamePhase(currentPhase);
            Debug.Log("[GAME MANAGER] Tutorial Has Been ENABLED");
        }
        else
        {
            IsTutorialEnabled = false;
            currentPhase = StartPhase;
            SetGamePhase(currentPhase);
            Debug.Log("[GAME MANAGER] Tutorial Has Been SKIPPED");
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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single); // ??
        IsTutorialEnabled = false;
        SceneManager.LoadScene("Main menu", LoadSceneMode.Single);
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
        Barrier.transform.position = Nexus.transform.position + new Vector3(6.0f, 22.33f, -6.0f);
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
