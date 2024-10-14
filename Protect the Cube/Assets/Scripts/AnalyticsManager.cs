using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsManager : MonoBehaviour
{
    [SerializeField] private string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSfjtjHdsAHNgMOAG65EgsWcrUNn7YornePsiKmTTvlsB_SjRg/formResponse";

    // form variables
    private int _sessionID;
    private int _waveDeathNum;
    private long _timestamp;

    private void Awake()
    {
        _timestamp = DateTime.Now.Ticks;  // Record the timestamp when the game starts
        _sessionID = GenerateSessionID();  // Generate session ID when the game starts
        _waveDeathNum = 0;  // Set waveDeathNum to 0 on game start

        // Log the start of the game with waveDeathNum = 0
        LogGameSpawn();
    }

    // Method to generate a random Session ID
    private int GenerateSessionID()
    {
        return UnityEngine.Random.Range(1000, 9999); // Use a 4-digit random number for the session ID
    }

    // Log when the game spawns (start)
    public void LogGameSpawn()
    {
        Debug.Log("Game Spawn: Logging initial analytics.");
        SendAnalytics();  // Log the event to Google Forms
    }

    // New method to log when a new wave starts
    public void LogUpdatedWaveStart(int waveNumber)
    {
        _waveDeathNum = waveNumber;
    }

    // Call this method when the player dies to log the death event
    public void PlayerDied()
    {
        SendAnalytics();  // Log the death event to Google Forms
    }

    // Send analytics data to Google Forms
    private void SendAnalytics()
    {
        StartCoroutine(Post(_sessionID.ToString(), _waveDeathNum.ToString(), _timestamp.ToString()));
    }

    // Post data to Google Forms using UnityWebRequest
    private IEnumerator Post(string sessionID, string waveDeathNum, string timestamp)
    {
        // Prepare the form data
        WWWForm form = new WWWForm();
        form.AddField("entry.87612042", sessionID);  // Google Form entry ID for sessionID
        form.AddField("entry.54409181", timestamp);  // Google Form entry ID for timestamp
        form.AddField("entry.560550316", waveDeathNum);  // Google Form entry ID for waveDeathNum

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Form upload failed: " + www.error);
            }
            else
            {
                Debug.Log("Analytics sent successfully!");
            }
        }
    }

    // Timer logic for game
    void Update() { }
}
