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

    public GameObject Player {  get; private set; }
    public GameObject Nexus {  get; set; }

    public bool useBulletPool = false;

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
    }

    void Start()
    {
        Time.timeScale = 1.0f;
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
