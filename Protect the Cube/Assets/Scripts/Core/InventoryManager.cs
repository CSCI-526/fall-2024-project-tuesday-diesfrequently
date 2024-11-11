// manages players inventory (turret prefabs + tracking each turret type)
// reward generation
// update player health + player ui

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // constant variables
    private const int _NUM_REWARD_CHOICES = 3;

    public const int TOTAL_NUM_INVENTORY = 9;
    public const int NUM_PLACEABLE_ITEMS = 7;
    public const int NUM_NON_PLACEABLE_ITEMS = 2;

    public const int PLAYER_HEALTH_INCREASE = 2;
    public const int NEXUS_HEALTH_INCREASE = 5;

    // references to GameObjects 
    private Nexus _nexus;
    private PlayerHealth _playerHP;
    private PlayerLevels _playerLevel;

    // stores reward-related and inventory-related information
    private Dictionary<string, int> _inventoryMapping; // Dictionary for Rewards String - Int Mapping
    private HashSet<GameObject> _potentialRewards; // stores (increasing) list of valid rewards player can choose from
    public List<int> InventoryItemCount { get; private set; } // old: buildingCount, stores # of reward in inventory

    // store item prefabs
    private List<GameObject> inventoryPrefabs;

    // event definitions
    public event Action UI_OnInventoryUpdated;
    public event Action<int> Analytics_OnInventoryAdded;
    public event Action<int> Analytics_OnInventoryUsed;
    public event Action<int, int, int> Analytics_OnRewardsUpdated;
    public event Action<GameObject, GameObject, GameObject> UI_OnRewardsUpdated;
    public event Action PlayerHealth_OnPlayerHealthUpdate;
    public event Action Nexus_OnNexusHealthUpdate;

    private void Awake()
    {
        _potentialRewards = new HashSet<GameObject>();
        _inventoryMapping = new Dictionary<string, int>()
        {
            { "Gun Turret", 0 },
            { "Gatling Turret", 1 },
            { "Flamethrower Turret", 2 },
            { "Sniper Turret", 3 },
            { "Booster Turret", 4 },
            { "Harvester", 5 },
            { "Slow Turret", 6 },
            { "Player HP", 7 },
            { "Nexus HP", 8 }
        };

        // size of the potential number of dropped inventory items
        inventoryPrefabs = new List<GameObject>();
        InventoryItemCount = new List<int>();

        // setup inventory list && count
        for (int idx = 0; idx < TOTAL_NUM_INVENTORY; idx++)
        {
            // add placeholders for upcoming inventory storage
            inventoryPrefabs.Add(null);
            InventoryItemCount.Add(0);
        }
    }

    private void Start()
    {
        _nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        _playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        _playerLevel = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        InitializeInventoryPrefabs();
    }

    private void InitializeInventoryPrefabs()
    {
        // Load all prefabs from Resources/Prefabs/ into GameObject Array
        // MATCHA: should be able to specify deeper into folders in the prefabs, do this once we organize prefabs into folders properly
        GameObject[] allPrefabs = Resources.LoadAll<GameObject>("Prefabs");
        Helper.checkArrayLengthSafe(allPrefabs, "No prefabs found in the Resources / Prefabs / directory.");

        foreach (var prefab in allPrefabs)
        {
            if (prefab == null) continue; // skip if prefab is invalid

            MultiTag prefabMultiTag = prefab.GetComponent<MultiTag>(); // get prefab MultiTag

            if (prefabMultiTag == null || !prefabMultiTag.HasTag("Inventory")) continue; // skip if prefabMultiTag is invalid or no inventory tag

            // handle non-building prefabs / rewards here
            int inventoryIDX = getItemIDX(prefab.name); // Use prefab name or another identifier

            //Debug.Log("Inventory Item Name: " + prefab.name + " with IDX of inventory: " + inventoryIDX);

            if (inventoryIDX >= 0 && inventoryIDX < TOTAL_NUM_INVENTORY)
            {
                inventoryPrefabs[inventoryIDX] = prefab;
                InventoryItemCount[inventoryIDX] = 0;
            }
        }
        Debug.Log($"Initialized {inventoryPrefabs.Count} reward prefabs with the 'Inventory' tag.");
    }

    public void GenerateRewards()
    {
        GameObject forcedReward = null;
        int playerLevelSnapshot = _playerLevel.currentLevel; // get player current XP level
        int playerHealthSnapshot = _playerHP.currentHealth; // get player current HP level
        int nexusHealthSnapshot = _nexus.currentHealth; // get nexus current HP level
        int forcedRewardCount = 0; // how many times a specific reward is forced as choice

        // add "Player HP" reward if the player has less than max health
        if (playerHealthSnapshot < PlayerHealth.PLAYER_MAX_HEALTH) { AddPotentialReward("Player HP", ref forcedReward, ref forcedRewardCount); }
        else { RemovePotentialReward("Player HP"); }
        // post level 10, give nexus health regen as an option for players
        if (nexusHealthSnapshot < Nexus.NEXUS_MAX_HEALTH && playerLevelSnapshot >= 5) { AddPotentialReward("Nexus HP", ref forcedReward, ref forcedRewardCount); }
        else { RemovePotentialReward("Nexus HP"); }

        // Level 1: Add "Gun Turret" as Valid Reward Choice
        if (playerLevelSnapshot == 1) { AddPotentialReward("Gun Turret", ref forcedReward, ref forcedRewardCount); }
        // Level 2: Add "Gatling Turret" and "Sniper Turret" as Valid Reward Choice
        else if (playerLevelSnapshot == 2) { 
            AddPotentialReward("Gatling Turret", ref forcedReward, ref forcedRewardCount);
            AddPotentialReward("Sniper Turret", ref forcedReward, ref forcedRewardCount);
        }
        // Level 3: Add "FlameThrower Turret" as Valid Reward Choice
        else if (playerLevelSnapshot == 3) { AddPotentialReward("Flamethrower Turret", ref forcedReward, ref forcedRewardCount);
        }
        // Level of Multiple 5: Force Harvestor on Levels of Multiple 5
        // Level 5: Force ONLY Harvestor Reward
        else if (playerLevelSnapshot % 5 == 0) { AddPotentialReward("Harvester", ref forcedReward, ref forcedRewardCount, playerLevelSnapshot == 5 ? 3 : 0); }
        // Level 7: Add "Slow Tower" as Valid Reward Choice
        else if (playerLevelSnapshot == 7) { AddPotentialReward("Slow Turret", ref forcedReward, ref forcedRewardCount); }
        // Level 7: Add "Turret Booster" as Valid Reward Choice
        else if (playerLevelSnapshot == 8) { AddPotentialReward("Booster Turret", ref forcedReward, ref forcedRewardCount); }

        if (playerLevelSnapshot % 5 != 0) { RemovePotentialReward("Harvester"); }

        // MATCHA: other building names: Sniper Turret, Turret Booster

        // Pick 3 Random Rewards from Available Rewards
        var chosenRewards = GenerateUniqueRewards(_potentialRewards.ToList(), forcedReward, forcedRewardCount);


        UpdateRewardDisplay(chosenRewards); // Updates Reward Display

        // cancel placing if rewards display activates
        // MATCHA: change to event driven
        GameManager.Instance.Player.GetComponent<PlaceObject>().CancelPlace();

        // generate reward indexes for analytics
        int[] rewardIndices = chosenRewards.Select(reward => getItemIDX(reward.name)).ToArray();

        // Analytics Update
        // Order: Gun Turret, Gatling Turret, Flamethrower Turret, Sniper Turret, Turret Booster, Harvester, Slow Tower
        Analytics_OnRewardsUpdated?.Invoke(rewardIndices[0], rewardIndices[1], rewardIndices[2]);
    }

    private void AddPotentialReward(string reward_name, ref GameObject forced_reward, ref int forced_reward_count, int force_count = 1)
    {
        int rewardIndex = getItemIDX(reward_name); // get index of reward prefab
        //Debug.Log("[Inventory Prefabs] ADD POTENTIAL REWARD .. reward_name " + reward_name + "rewardIDX: " + rewardIndex);
        forced_reward = inventoryPrefabs[rewardIndex]; // get inventory prefab
        forced_reward_count = force_count > 0 ? force_count : 1; // set the # of times reward is forced as a choice
        _potentialRewards.Add(forced_reward); // current reward becomes "available" as a choice
    }

    private void RemovePotentialReward(string reward_name)
    {
        // Use a temporary list to collect items to remove
        List<GameObject> rewardsToRemove = new List<GameObject>();

        // iterate through potential rewards, identify rewards to remove
        foreach (var reward in _potentialRewards)
        {
            if (reward.name == reward_name) rewardsToRemove.Add(reward); // add all rewards to remove
        }

        // remove rewards as needed
        foreach (var reward in rewardsToRemove)
        {
            _potentialRewards.Remove(reward); // Remove from available rewards
        }

        if (rewardsToRemove.Count > 0)
        {
            Debug.Log($"Removed {rewardsToRemove.Count} instances of {reward_name} from available rewards.");
        }
    }

    public List<GameObject> GenerateUniqueRewards(List<GameObject> rewards_list, GameObject forced_reward = null, int forced_reward_count = 0)
    {
        Debug.Log(string.Join(", ", rewards_list.Select(reward => reward.name)));

        // sanity check for valid forced reward count
        forced_reward_count = Mathf.Clamp(forced_reward_count, 0, _NUM_REWARD_CHOICES);

        // create a final rewards list pre-populated with X counts of the forced_reward
        List<GameObject> finalRewards = new List<GameObject>(Enumerable.Repeat(forced_reward, forced_reward_count).Where(x => x != null));

        //Debug.Log(string.Join(", ", finalRewards.Select(reward => reward.name)));

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

        //Debug.Log(string.Join(", ", finalRewards.Select(reward => reward.name)));

        // if we do not have 3 VALID reward choices, just duplicate existing rewards
        while (finalRewards.Count < 3) { finalRewards.Add(finalRewards[0]); }

        // return the 3 reward choices
        return finalRewards;
    }

    // How to handle which "reward" is picked
    // MATCHA: should rename to HandlePickedReward
    public void HandlePickedReward(string reward_name)
    {
        // Update Health if the reward is health-based
        if (reward_name.Contains("HP")) UpdateHealth(reward_name);

        // Update Inventory by Picked Reward
        UpdateInventoryCount(reward_name);
    }

    private void UpdateHealth(string reward_name)
    {
        // update player / nexus current health by HEALTH_INCREASE (up to max hp)
        if (reward_name == "Player HP") PlayerHealth_OnPlayerHealthUpdate?.Invoke();
        else if (reward_name == "Nexus HP") Nexus_OnNexusHealthUpdate?.Invoke();
    }

    private void UpdateInventoryCount(string item_name)
    {
        int itemIDX = getItemIDX(item_name);
        if (itemIDX < 0) Debug.Log("[InventoryManager] Cannot Find Item IDX to store in Inventory");

        // add reward to "inventory"
        InventoryItemCount[itemIDX]++;

        // Update Inventory UI
        UI_OnInventoryUpdated?.Invoke();

        // Update Inventory Analytics
        Analytics_OnInventoryAdded?.Invoke(itemIDX);
    }

    // try to use an inventory item, use if item is available
    public bool TryUseInventoryItem(string item_name)
    {
        // if there is no inventory, cannot use item
        if (!isInventoryAvailable(item_name)) return false;

        int itemIDX = getItemIDX(item_name);
        InventoryItemCount[itemIDX]--;

        // Update Inventory UI
        UI_OnInventoryUpdated?.Invoke();

        // Update Analytics UI
        Analytics_OnInventoryUsed.Invoke(itemIDX);

        return true; // item has been used
    }

    private void UpdateRewardDisplay(List<GameObject> chosenRewards)
    {
        UI_OnRewardsUpdated?.Invoke(chosenRewards[0], chosenRewards[1], chosenRewards[2]);
    }

    // function that returns whether there is inventory of a particular reward available
    public bool isInventoryAvailable(string reward_name) => InventoryItemCount[getItemIDX(reward_name)] > 0;

    // get IDX of reward given a rewardName
    public int getItemIDX(string rewardName) => _inventoryMapping.TryGetValue(rewardName, out int index) ? index : -1;
}