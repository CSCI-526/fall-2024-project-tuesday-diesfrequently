using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsManager : MonoBehaviour
{
    [SerializeField] private string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfjtjHdsAHNgMOAG65EgsWcrUNn7YornePsiKmTTvlsB_SjRg/formResponse";

    // [Old Analytics]
    // @SendOn (1) Session Start (2) Session End (when player dies)
    // sessionID (same session ID for each sent instance) 
    // timestamp
    // waveDeathNum (defaults to 0 at start, non-zero number on session end)

    // [New Analytics]
    // @SendOn (1) Session Start (most things init to 0), Session Start
    // Metadata: Timestamp, Session ID (same for each session)
    // Metric #1: [Wave Stats] Player Hitpoint Loss Wave, Player Death Wave
    // Metric #2: [Player Stats] Player Level, Player Accumulated XP, Player Acquired Gold, Player Spent Gold
    // Metric #3: [Turret Stats] {Total} {By Turret Type} # of Acquired Turrets, # of Placed Turrets, # of Level 2 Turrets, # of Level 3 Turrets
    // Metric #4: [Reward Stats] {By Reward Type} Options Offered

    // Lock Object for Thread Safety //
    private readonly object _lockObject = new object();

    // analytics tracked variables (Metadata)
    private int _sessionID;
    private long _timestamp;

    // analytics tracked variables (metric #1)
    private List<int> _playerHitpointLossWaves; // make this a list
    private int _waveDeathNum;

    // analytics tracked variables (metric #2)
    private int _playerLevel;
    private int _playerXP;
    private int _playerAcquiredGold;
    private int _playerSpentGold;

    // analytics tracked variabels (metric #3)
    private List<int> _totalAcquiredTurrets; // list of [0] is total, [x] is by turret type
    private List<int> _totalPlacedTurrets; // list of [0] is total, [x] is by turret type
    private List<int> _lvl2Turrets; // list of [0] is total, [x] is by turret type
    private List<int> _lvl3Turrets; // list of [0] is total, [x] is by turret type

    // analytics tracked variabels (metric #4)
    private List<int> _rewardsOffered; // list of [0] is total rewards offered, [x] is how much of each reward


    private void Awake()
    {
        _timestamp = DateTime.Now.Ticks;    // Record the timestamp when the game starts
        _sessionID = GenerateSessionID();   // Generate session ID when the game starts
        InitializeSessionAnalyticsVariables();          // Initialize all analytics stat variables

         //[DEBUG] Send Session Start Analytics Report
         SendSessionStartAnalytics();
    }

    // Description: Generates a random 4-digit session ID.
    // Returns: An integer between 1000 and 9999.
    private int GenerateSessionID()
    {
        return UnityEngine.Random.Range(1000, 9999); // Use a 4-digit random number for the session ID
    }

    // Description: Initializes all analytics variables to their default state at the start of the session.
    private void InitializeSessionAnalyticsVariables()
    {
        lock (_lockObject)
        {
            // Metric #1
            _waveDeathNum = 0;
            _playerHitpointLossWaves = new List<int>(new int[1]); // dynamic list (size will grow)

            // Metric #2
            _playerLevel = 1;
            _playerXP = 0;
            _playerAcquiredGold = 0;
            _playerSpentGold = 0;

            // Metric #3
            _totalAcquiredTurrets = new List<int>(new int[8]); // [All Turrets (0), Gun Turret (1), Gatling Turret (2), Flamethrower (3), Sniper Turret (4), Turret Booster (5), Harvestor (6), Slow Turret (7)]
            _totalPlacedTurrets = new List<int>(new int[8]); // [All Turrets (0), Gun Turret (1), Gatling Turret (2), Flamethrower (3), Sniper Turret (4), Turret Booster (5), Harvestor (6), Slow Turret (7)]
            _lvl2Turrets = new List<int>(new int[8]); // [All Turrets (0), Gun Turret (1), Gatling Turret (2), Flamethrower (3), Sniper Turret (4), Turret Booster (5), Harvestor (6), Slow Turret (7)]
            _lvl3Turrets = new List<int>(new int[8]); // [All Turrets (0), Gun Turret (1), Gatling Turret (2), Flamethrower (3), Sniper Turret (4), Turret Booster (5), Harvestor (6), Slow Turret (7)]

            // Metric #4
            _rewardsOffered = new List<int>(new int[8]); // // [All Rewards (0), Gun Turret (1), Gatling Turret (2), Flamethrower (3), Sniper Turret (4), Turret Booster (5), Harvestor (6), Slow Turret (7)]
        }
    }

    // Description: Logs the initial game spawn event and sends analytics data.
    public void SendSessionStartAnalytics()
    {
        Debug.Log("Game Spawn: Sending Session Start Analytics for Session ID " + _sessionID);
        PostAnalytics();  // Log the event to Google Forms
    }

    /************/
    /* Metric 1 */
    /************/

    // Description: Appends the given wave number to the list of hitpoint loss waves.
    // Parameters: waveNumber - The wave number where a player has lost hitpoints.
    public void UpdateHitpointLossWave(int waveNumber)
    {
        lock (_lockObject)
        {
            _playerHitpointLossWaves.Add(waveNumber);  // Append the wave number to the list
            
        }
    }

    // Description: Updates the wave number
    // Parameters: waveNumber - The current wave number
    public void UpdateWaveNumber(int waveNumber)
    {
        lock (_lockObject)
        {
            _waveDeathNum = waveNumber;
        }
    }

    /************/
    /* Metric 2 */
    /************/
    // Description: Updates the player's level
    // Parameters: playerLevel - The new level of the player
    public void UpdatePlayerLevel(int playerLevel)
    {
        lock (_lockObject)
        {
            _playerLevel = playerLevel;
        }
    }

    // Description: Updates the player's experience points
    // Parameters: playerXP - The new XP of the player
    public void UpdatePlayerXP(int playerXP)
    {
        lock (_lockObject)
        {
            _playerXP = playerXP;
        }
    }

    // Description: Updates the total gold acquired by the player
    // Parameters: playerAcquiredGold - The total gold acquired by the player
    public void UpdatePlayerAcquiredGold(int playerAcquiredGold)
    {
        lock (_lockObject)
        {
            _playerAcquiredGold = playerAcquiredGold;
        }
    }

    // Description: Updates the total gold spent by the player
    // Parameters: playerSpentGold - The total gold spent by the player
    public void UpdatePlayerSpentGold(int playerSpentGold)
    {
        lock (_lockObject)
        {
            _playerSpentGold = playerSpentGold;
        }
    }

    /************/
    /* Metric 3 */
    /************/
    // [All Turrets (0), Gun Turret (1), Gatling Turret (2), Flamethrower (3), Sniper Turret (4), Turret Booster (5), Harvestor (6), Slow Turret (7)]

    // Description: Increments the count of acquired turrets for the given turret index
    // Parameters: turretIDX - The index of the turret type acquired
    public void UpdateTotalAcquiredTurrets(int turretIDX)
    {
        lock (_lockObject)
        {
            _totalAcquiredTurrets[turretIDX] += 1;      // indicate which specific turret type was obtained
            _totalAcquiredTurrets[0] += 1;              // increase total # of turrets acquired
        }
    }

    // Description: Increments the count of placed turrets for the given turret index
    // Parameters: turretIDX - The index of the turret type placed
    public void UpdateTotalPlacedTurrets(int turretIDX)
    {
        lock (_lockObject)
        {
            _totalPlacedTurrets[turretIDX] += 1;      // indicate which specific turret type was placed
            _totalPlacedTurrets[0] += 1;              // increase total # of turrets placed
        }
    }

    // Description: Increments the count of turrets upgraded to level 2 for the given turret index
    // Parameters: turretIDX - The index of the turret type upgraded
    public void UpdateLvl2Turrets(int turretIDX)
    {
        lock (_lockObject)
        {
            _lvl2Turrets[turretIDX] += 1;      // indicate which specific turret type was upgraded to lvl 2
            _lvl2Turrets[0] += 1;              // increase total # of turrets upgraded to lvl 2
        }
    }

    // Description: Increments the count of turrets upgraded to level 3 for the given turret index
    // Parameters: turretIDX - The index of the turret type upgraded
    public void UpdateLvl3Turrets(int turretIDX)
    {
        lock (_lockObject)
        {
            _lvl3Turrets[turretIDX] += 1;      // indicate which specific turret type was upgraded to lvl 3
            _lvl3Turrets[0] += 1;              // increase total # of turrets upgraded to lvl 3
        }
    }

    /************/
    /* Metric 4 */
    /************/

    // Description: Increments the count of rewards offered for the given reward IDX
    // Parameters: rewardIDX - The index of the reward type offered
    public void UpdateRewardsOffered(int rewardIDX1, int rewardIDX2, int rewardIDX3)
    {
        lock (_lockObject)
        {
            _rewardsOffered[rewardIDX1] += 1;      // indicate which specific reward type was offered
            _rewardsOffered[rewardIDX2] += 1;      // indicate which specific reward type was offered
            _rewardsOffered[rewardIDX3] += 1;      // indicate which specific reward type was offered
            _rewardsOffered[0] += 3;              // increase total # of rewards offered by 3
        }
    }

    // Description: Logs the session end (death, force quit, etc.) and sends analytics data.
    public void SendSessionEndAnalytics()
    {
        Debug.Log("Game Spawn: Sending Session End Analytics for Session ID " + _sessionID);
        PostAnalytics();  // Log the session ends to Google Forms
    }


    // Description: prepares and sends the analytics data for the session, locking access to ensure thread safety
    // This function collects all tracked variables and sends them via the Post coroutine
    private void PostAnalytics()
    {
        lock (_lockObject)
        {
            StartCoroutine(Post(
                _sessionID.ToString(),                      // [Metadata]
                _timestamp.ToString(),                      // [Metadata]
                string.Join("_", _playerHitpointLossWaves), // [Metric 1] list values sep by '_'
                _waveDeathNum.ToString(),                   // [Metric 1]
                _playerLevel.ToString(),                    // [Metric 2] 
                _playerXP.ToString(),                       // [Metric 2] 
                _playerAcquiredGold.ToString(),             // [Metric 2] 
                _playerSpentGold.ToString(),                // [Metric 2] 
                string.Join("_", _totalAcquiredTurrets),    // [Metric 3] list values sep by '_'
                string.Join("_", _totalPlacedTurrets),      // [Metric 3] list values sep by '_'
                string.Join("_", _lvl2Turrets),             // [Metric 3] list values sep by '_'
                string.Join("_", _lvl3Turrets),             // [Metric 3] list values sep by '_'
                string.Join("_", _rewardsOffered)           // [Metric 4] list values sep by '_'
            ));
        }
    }

    // Description: Sends the prepared analytics data to Google Forms using UnityWebRequest
    // Parameters:
    // - sessionID: The unique identifier for the player session
    // - timestamp: The timestamp when the player session started
    // - hitpointLossWaves: A string representation of the waves during which the player lost hitpoints
    // - waveDeathNum: The wave number at which the player died
    // - playerLevel: The player's current level by death
    // - playerXP: The player's total experience points by death
    // - playerAcquiredGold: The total amount of gold acquired by the player by death
    // - playerSpentGold: The total amount of gold spent by the player by death
    // - totalAcquiredTurrets: A string representation of total turrets acquired by the player by death
    // - totalPlacedTurrets: A string representation of total turrets placed by the player by death
    // - lvl2Turrets: A string representation of turrets upgraded to level 2 by death
    // - lvl3Turrets: A string representation of turrets upgraded to level 3 by death
    // - rewardsOffered: A string representation of rewards offered to the player by death
    private IEnumerator Post(string metadata_sessionID, string metadata_timestamp, string m1_hitpointLossWaves, string m1_waveDeathNum, string m2_playerLevel, string m2_playerXP, string m2_playerAcquiredGold, string m2_playerSpentGold, string m3_totalAcquiredTurrets, string m3_totalPlacedTurrets, string m3_lvl2Turrets, string m3_lvl3Turrets, string m4_rewardsOffered)
    {
        // Prepare the form data
        WWWForm form = new WWWForm();

        Debug.Log("metadata_sessionID " + metadata_sessionID);
        Debug.Log("metadata_timestamp " + metadata_timestamp);

        Debug.Log("m1_hitpointLossWaves " + m1_hitpointLossWaves);
        Debug.Log("m1_waveDeathNum " + m1_waveDeathNum);

        Debug.Log("m2_playerLevel " + m2_playerLevel);
        Debug.Log("m2_playerXP " + m2_playerXP);
        Debug.Log("m2_playerAcquiredGold: " + m2_playerAcquiredGold);
        Debug.Log("m2_playerSpentGold: " + m2_playerSpentGold);

        Debug.Log("m3_totalAcquiredTurrets: " + m3_totalAcquiredTurrets);
        Debug.Log("m3_totalPlacedTurrets: " + m3_totalPlacedTurrets);
        Debug.Log("m3_lvl2Turrets: " + m3_lvl2Turrets);
        Debug.Log("m3_lvl3Turrets: " + m3_lvl3Turrets);

        Debug.Log("m4_rewardsOffered: " + m4_rewardsOffered);


        // Form Metadata
        form.AddField("entry.87612042", metadata_sessionID);             // Google Form entry ID for sessionID
        form.AddField("entry.54409181", metadata_timestamp);             // Google Form entry ID for timestamp

        // Form Metric #1
        form.AddField("entry.123608719", m1_hitpointLossWaves);     
        form.AddField("entry.560550316", m1_waveDeathNum);       

        // Form Metric #2
        form.AddField("entry.1288335364", m2_playerLevel);          
        form.AddField("entry.2076320647", m2_playerXP);              
        form.AddField("entry.52450435", m2_playerAcquiredGold);   
        form.AddField("entry.1843765570", m2_playerSpentGold);      

        // Form Metric #3
        form.AddField("entry.271943376", m3_totalAcquiredTurrets); 
        form.AddField("entry.175754866", m3_totalPlacedTurrets);   
        form.AddField("entry.1573936588", m3_lvl2Turrets);        
        form.AddField("entry.2011166216", m3_lvl3Turrets);         

        // Form Metric #4
        form.AddField("entry.234620807", m4_rewardsOffered);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Form upload failed: " + www.error);
                Debug.LogError("Response Code: " + www.responseCode);
                Debug.LogError("Response: " + www.downloadHandler.text); // Log the server response for more info
            }
            else
            {
                Debug.Log("Analytics sent successfully!");
            }
        }
    }
}
