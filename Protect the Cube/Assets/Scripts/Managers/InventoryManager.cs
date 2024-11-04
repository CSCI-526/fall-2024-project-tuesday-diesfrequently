// manages players inventory (turret prefabs + tracking each turret type)
// reward generation
// update player health + player ui

// MATCHA: check analytics indices, check prefab (order it loads in) indices
// MATCHA: change the rewardMapping to inventoryMapping .. and InventoryItemCount to inventoryCounts

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // contant variables
    private const int _HEALTH_INCREASE = 2;
    private const int _NUM_REWARD_CHOICES = 3; 

    // references to GameObjects 
    private Nexus _nexus;
    private PlayerHealth _playerHP;
    private PlayerLevels _playerLevel;

    // stores reward-related and inventory-related information
    private Dictionary<string, int> _inventoryMapping; // Dictionary for Rewards String - Int Mapping
    private HashSet<GameObject> _availableRewards; // stores (increasing) list of valid rewards player can choose from
    public List<int> InventoryItemCount { get; private set; } // old: buildingCount, stores # of reward in inventory

    // store item prefabs
    [SerializeField] private List<GameObject> inventoryPrefabs;

    // event definitions
    public event Action UI_OnInventoryUpdated;
    public event Action<int> Analytics_OnInventoryAdded;
    public event Action<int> Analytics_OnInventoryUsed;
    public event Action<int, int, int> Analytics_OnRewardsUpdated;
    public event Action<List<GameObject>> UI_OnRewardsUpdated;

    private void Awake()
    {
        _availableRewards = new HashSet<GameObject>();
        _inventoryMapping = new Dictionary<string, int>()
        {
            { "Gun Turret", 1 },
            { "Gatling Turret", 2 },
            { "Flamethrower Turret", 3 },
            { "Sniper Turret", 4 },
            { "Turret Booster", 5 },
            { "Harvester", 6 },
            { "Slow Tower", 7 }
        };

        inventoryPrefabs = new List<GameObject>();
        InventoryItemCount = new List<int>();

    }

    private void Start()
    {
        // MATCHA: replace these with events?
        _nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        _playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        _playerLevel = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        InitializeInventoryPrefabs();
    }

    private void InitializeInventoryPrefabs()
    {
        inventoryPrefabs.Clear(); // Clear the existing list of inventory prefabs

        // Load all prefabs from Resources/Prefabs/ into GameObject Array
        // MATCHA: should be able to specify deeper into folders in the prefabs, do this once we organize prefabs into folders properly
        GameObject[] allPrefabs = Resources.LoadAll<GameObject>("Prefabs");
        Helper.checkArrayLengthSafe(allPrefabs, "No prefabs found in the Resources / Prefabs / directory.");

        foreach (var prefab in allPrefabs)
        {
            if (prefab == null) continue; // skip if prefab is invalid

            MultiTag prefabMultiTag = prefab.GetComponent<MultiTag>(); // get prefab MultiTag

            if (prefabMultiTag == null) continue; // skip if prefabMultiTag is invalid

            if (prefabMultiTag.HasTag("Inventory")) // if prefab tagged "Inventory"
            {
                inventoryPrefabs.Add(prefab); // add ACTUAL inventory prefab
                InventoryItemCount.Add(0); // add inventory slot for inventory prefab with count 0
            }
        }

        Debug.Log($"Initialized {inventoryPrefabs.Count} reward prefabs with the 'Inventory' tag.");
    }

    public void GenerateRewards()
    {
        GameObject forcedReward = null;
        int playerLevelSnapshot = _playerLevel.currentLevel; // get player current XP level
        int forcedRewardCount = 0; // how many times a specific reward is forced as choice

        // Level 1: Add "Gun Turret" as Valid Reward Choice
        if (playerLevelSnapshot == 1) { UpdateAvailableRewards("Gun Turret", ref forcedReward, ref forcedRewardCount); }
        // Level 2: Add "Gatling Turret" as Valid Reward Choice
        else if (playerLevelSnapshot == 2) { UpdateAvailableRewards("Gatling Turret", ref forcedReward, ref forcedRewardCount); }
        // Level 3: Add "FlameThrower Turret" as Valid Reward Choice
        else if (playerLevelSnapshot == 3) { UpdateAvailableRewards("Flamethrower Turret", ref forcedReward, ref forcedRewardCount); }
        // Level of Multiple 5: Force Harvestor on Levels of Multiple 5
        // Level 5: Force ONLY Harvestor Reward
        else if (playerLevelSnapshot % 5 == 0) { UpdateAvailableRewards("Harvester", ref forcedReward, ref forcedRewardCount, playerLevelSnapshot == 5 ? 3 : 0); }
        // Level 7: Add "Slow Tower" as Valid Reward Choice
        else if (playerLevelSnapshot == 7) { UpdateAvailableRewards("Slow Tower", ref forcedReward, ref forcedRewardCount); }
        // Level 7: Add "Turret Booster" as Valid Reward Choice
        else if (playerLevelSnapshot == 8) { UpdateAvailableRewards("Turret Booster", ref forcedReward, ref forcedRewardCount); }

        // MATCHA: other building names: Sniper Turret, Turret Booster

        // Pick 3 Random Rewards from Available Rewards
        var chosenRewards = GenerateUniqueRewards(_availableRewards.ToList(), forcedReward, forcedRewardCount);

        UpdateRewardDisplay(chosenRewards); // Updates Reward Display

        // MATCHA: THIS DOES NOT WORK FOR NON-BUILDING REWARDS ... fix later!

        // generate reward indexes for analytics
        int[] rewardIndices = chosenRewards.Select(reward => getItemIDX(reward.GetComponent<Building>().buildingName)).ToArray();

        // Analytics Update
        // Order: Gun Turret, Gatling Turret, Flamethrower Turret, Sniper Turret, Turret Booster, Harvester, Slow Tower
        Analytics_OnRewardsUpdated?.Invoke(rewardIndices[0], rewardIndices[1], rewardIndices[2]);
        //GameManager.Instance.AnalyticsManager.UpdateRewardsOffered(rewardIndices[0], rewardIndices[1], rewardIndices[2]);
    }

    private void UpdateAvailableRewards(string reward_name, ref GameObject forced_reward, ref int forced_reward_count, int force_count = 1)
    {
        int rewardIndex = getItemIDX(reward_name); // get index of reward prefab
        forced_reward = inventoryPrefabs[rewardIndex]; // get inventory prefab
        forced_reward_count = force_count > 0 ? force_count : 1; // set the # of times reward is forced as a choice
        _availableRewards.Add(forced_reward); // current reward becomes "available" as a choice
    }

    public List<GameObject> GenerateUniqueRewards(List<GameObject> rewards_list, GameObject forced_reward = null, int forced_reward_count = 0)
    {
        // sanity check for valid forced reward count
        forced_reward_count = Mathf.Clamp(forced_reward_count, 0, _NUM_REWARD_CHOICES);

        // create a final rewards list pre-populated with X counts of the forced_reward
        List<GameObject> finalRewards = new List<GameObject>(Enumerable.Repeat(forced_reward, forced_reward_count).Where(x => x != null));

        // calculate how many rewards we need to add 
        int remainingSlots = _NUM_REWARD_CHOICES - finalRewards.Count;

        if (remainingSlots > 0)
        {
            var availableRewards = rewards_list
                .Where(r => r != forced_reward) // don't choose forced_reward
                .OrderBy(x => UnityEngine.Random.value)     // shuffle the reward list
                .Take(remainingSlots);          // add X more rewards to

            finalRewards.AddRange(availableRewards); // add additional rewards to final output
        }

        // if we do not have 3 VALID reward choices, just duplicate existing rewards
        while (finalRewards.Count < 3) { finalRewards.Add(finalRewards[0]); }

        // return the 3 reward choices
        return finalRewards;
    }

    // How to handle which "reward" is picked
    // MATCHA: should rename to HandlePickedReward
    public void PickReward(string reward_name)
    {
        // Update Health if the reward is health-based
        // MATCHA: maybe handle this in another file? not rly inventory related
        if (reward_name.Contains("HP")) UpdateHealth(reward_name);

        // Update Inventory by Picked Reward
        UpdateInventoryCount(reward_name);
    }

    // MATCHA... would rather handle this NOT in inventory but in their respective files! (use events?)
    // MATCHA... add analytics event for "Player / Nexus HP" adding up
    private void UpdateHealth(string reward_name)
    {
        if (reward_name == "Player HP") // update player current health by HEALTH_INCREASE (if you can)
        {
            // do NOT do this here, send an event to "playerHP component"
            _playerHP.currentHealth = Mathf.Min(_playerHP.currentHealth + _HEALTH_INCREASE, _playerHP.maxHealth);
        }
        else if (reward_name == "Nexus HP") // update nexus current health by HEALTH_INCREASE (if you can)
        {
            // MATCHA: do not do this here, send an event to "nexus component" 
            _nexus.health = Mathf.Min(_nexus.health + _HEALTH_INCREASE, _nexus.maxHealth);
        }
    }

    // MATCHA (ADD TO INVENTORY) better function nam?
    private void UpdateInventoryCount(string item_name)
    {
        int itemIDX = getItemIDX(item_name);
        if (itemIDX < 0) Debug.Log("[InventoryManager] Cannot Find Item IDX to store in Inventory");

        InventoryItemCount[itemIDX]++; // add reward to "inventory"

        // Update Inventory UI
        UI_OnInventoryUpdated?.Invoke();
        //GameManager.Instance.UIManager.UpdateInventoryUI(); // MATCHA: is it good practice to call function from another manager?

        // MATCHA: only run this if reward is a "building" reward, new analytics for non-building rewards
        Analytics_OnInventoryAdded.Invoke(itemIDX);
        //GameManager.Instance.AnalyticsManager.UpdateTotalAcquiredTurrets(itemIDX);
    }

    // try to use an inventory item, use if item is available
    // MATCHA (TAKE FROM INVENTORY) better function nam?
    public bool TryUseInventoryItem(string item_name)
    {
        // if there is no inventory, cannot use item
        if (!isInventoryAvailable(item_name)) return false;

        int itemIDX = getItemIDX(item_name);
        InventoryItemCount[itemIDX]--;

        // Update Inventory UI
        UI_OnInventoryUpdated?.Invoke();
        //GameManager.Instance.UIManager.UpdateInventoryUI();

        // MATCHA: diff logic for non-building inventory items
        Analytics_OnInventoryUsed.Invoke(itemIDX);
        //GameManager.Instance.AnalyticsManager.UpdateTotalPlacedTurrets(turretIDX0);

        return true; // item has been used

    }

    private void UpdateRewardDisplay(List<GameObject> chosenRewards)
    {
        UI_OnRewardsUpdated?.Invoke(chosenRewards);
        //GameManager.Instance.UIManager.UpdateRewardsUI(chosenRewards[0], chosenRewards[1], chosenRewards[2]);
    }

    // function that returns whether there is inventory of a particular reward available
    public bool isInventoryAvailable(string reward_name) => InventoryItemCount[getItemIDX(reward_name)] > 0;

    // get IDX of reward given a rewardName
    public int getItemIDX(string rewardName) => _inventoryMapping.TryGetValue(rewardName, out int index) ? index : -1;
}
