// manages players inventory (turret prefabs + tracking each turret type)
// reward generation
// update player health + player ui

// MATCHA: check analytics indices, check prefab (order it loads in) indices
// MATCHA: change the rewardMapping to inventoryMapping .. and rewardCounts to inventoryCounts

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private const int _HEALTH_INCREASE = 2;
    private const int _NUM_REWARD_CHOICES = 3; 

    private Nexus _nexus;
    private PlayerHealth _playerHP;
    private PlayerLevels _playerLevel;
    private Dictionary<string, int> _rewardMapping; // Dictionary for Rewards String - Int Mapping
    private HashSet<GameObject> _availableRewards; // stores (increasing) list of valid rewards player can choose from

    [SerializeField] private List<GameObject> rewardPrefabs;
    [SerializeField] private List<int> rewardCounts; // old: buildingCount, stores # of reward in inventory

    //stores the names of each type of turret
    //[SerializeField] public List<string> buildingNames = new List<string>();

    private void Awake()
    {
        _availableRewards = new HashSet<GameObject>();
        _rewardMapping = new Dictionary<string, int>()
        {
            { "Gun Turret", 1 },
            { "Gatling Turret", 2 },
            { "Flamethrower Turret", 3 },
            { "Sniper Turret", 4 },
            { "Turret Booster", 5 },
            { "Harvester", 6 },
            { "Slow Tower", 7 }
        };

        rewardPrefabs = new List<GameObject>();
        rewardCounts = new List<int>();

    }

    private void Start()
    {
        _nexus = GameManager.Instance.Nexus.GetComponent<Nexus>();
        _playerHP = GameManager.Instance.Player.GetComponent<PlayerHealth>();
        _playerLevel = GameManager.Instance.Player.GetComponent<PlayerLevels>();
        InitializeRewardPrefabs();
    }

    private void InitializeRewardPrefabs()
    {
        rewardPrefabs.Clear(); // Clear the existing list of reward prefabs

        // Load all prefabs from Resources/Prefabs/ into GameObject Array
        // MATCHA: should be able to specify deeper into folders in the prefabs, do this once we organize prefabs into folders properly
        GameObject[] allPrefabs = Resources.LoadAll<GameObject>("Prefabs");
        Helper.checkArrayLengthSafe(allPrefabs, "No prefabs found in the Resources / Prefabs / directory.");

        foreach (var prefab in allPrefabs)
        {
            if (prefab == null) continue; // skip if prefab is invalid

            MultiTag prefabMultiTag = prefab.GetComponent<MultiTag>(); // get prefab MultiTag

            if (prefabMultiTag == null) continue; // skip if prefabMultiTag is invalid

            if (prefabMultiTag.HasTag("Reward")) // if prefab tagged "Reward"
            {
                rewardPrefabs.Add(prefab); // add ACTUAL reward prefab
                rewardCounts.Add(0); // add inventory slot for reward prefab
            }
        }

        Debug.Log($"Initialized {rewardPrefabs.Count} reward prefabs with the 'Reward' tag.");
    }

    public void GenerateRewards()
    {
        GameObject forcedReward = null;
        int currentPlayerLevel = _playerLevel.currentLevel; // get player current XP level
        int forcedRewardCount = 0; // how many times a specific reward is forced as choice

        // Level 1: Add "Gun Turret" as Valid Reward Choice
        if (currentPlayerLevel == 1) { UpdateAvailableRewards("Gun Turret", ref forcedReward, ref forcedRewardCount); }
        // Level 2: Add "Gatling Turret" as Valid Reward Choice
        else if (currentPlayerLevel == 2) { UpdateAvailableRewards("Gatling Turret", ref forcedReward, ref forcedRewardCount); }
        // Level 3: Add "FlameThrower Turret" as Valid Reward Choice
        else if (currentPlayerLevel == 3) { UpdateAvailableRewards("Flamethrower Turret", ref forcedReward, ref forcedRewardCount); }
        // Level of Multiple 5: Force Harvestor on Levels of Multiple 5
        // Level 5: Force ONLY Harvestor Reward
        else if (currentPlayerLevel % 5 == 0) { UpdateAvailableRewards("Harvester", ref forcedReward, ref forcedRewardCount, currentPlayerLevel == 5 ? 3 : 0); }
        // Level 7: Add "Slow Tower" as Valid Reward Choice
        else if (currentPlayerLevel == 7) { UpdateAvailableRewards("Slow Tower", ref forcedReward, ref forcedRewardCount); }
        // Level 7: Add "Turret Booster" as Valid Reward Choice
        else if (currentPlayerLevel == 8) { UpdateAvailableRewards("Turret Booster", ref forcedReward, ref forcedRewardCount); }

        // MATCHA: other building names: Sniper Turret, Turret Booster

        // Pick 3 Random Rewards from Available Rewards
        var chosenRewards = GenerateUniqueRewards(_availableRewards.ToList(), forcedReward, forcedRewardCount);

        UpdateRewardDisplay(chosenRewards); // Updates Reward Display

        // MATCHA: THIS DOES NOT WORK FOR NON-BUILDING REWARDS ... fix later!

        // generate reward indexes for analytics
        int[] rewardIndices = chosenRewards.Select(reward => getRewardIDX(reward.GetComponent<Building>().buildingName)).ToArray();

        // Analytics Update
        // Order: Gun Turret, Gatling Turret, Flamethrower Turret, Sniper Turret, Turret Booster, Harvester, Slow Tower
        GameManager.Instance.AnalyticsManager.UpdateRewardsOffered(rewardIndices[0], rewardIndices[1], rewardIndices[2]);
    }

    private void UpdateAvailableRewards(string reward_name, ref GameObject forced_reward, ref int forced_reward_count, int force_count = 1)
    {
        int rewardIndex = getRewardIDX(reward_name); // get index of reward prefab
        forced_reward = rewardPrefabs[rewardIndex]; // get reward prefab
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
                .OrderBy(x => Random.value)     // shuffle the reward list
                .Take(remainingSlots);          // add X more rewards to

            finalRewards.AddRange(availableRewards); // add additional rewards to final output
        }

        // if we do not have 3 VALID reward choices, just duplicate existing rewards
        while (finalRewards.Count < 3) { finalRewards.Add(finalRewards[0]); }

        // return the 3 reward choices
        return finalRewards;
    }

    // How to handle which "reward" is picked
    // MATCHA: should rename to handlePickedReward
    public void PickReward(string reward_name)
    {
        // Update Health if the reward is health-based
        // MATCHA: maybe handle this in another file? not rly inventory related
        if (reward_name.Contains("HP")) UpdateHealth(reward_name);

        // Update Inventory by Picked Reward
        UpdateInventoryCount(reward_name);
    }

    // MATCHA... would rather handle this NOT in inventory but in their respective files! (use events?) 
    private void UpdateHealth(string reward_name)
    {
        if (reward_name == "Player HP") // update player current health by HEALTH_INCREASE (if you can)
        {
            _playerHP.currentHealth = Mathf.Min(_playerHP.currentHealth + _HEALTH_INCREASE, _playerHP.maxHealth);
        }
        else if (reward_name == "Nexus HP") // update nexus current health by HEALTH_INCREASE (if you can)
        {
            _nexus.health = Mathf.Min(_nexus.health + _HEALTH_INCREASE, _nexus.maxHealth);
        }

        // MATCHA: would rather handle this in UIManager ... not in Analytics Manager! 
        GameManager.Instance.UIManager.UpdateUI();
    }

    private void UpdateInventoryCount(string item_name)
    {
        int rewardIDX = getRewardIDX(item_name);
        if (rewardIDX < 0) Debug.Log("[InventoryManager] Cannot Find Picked Reward to store in Inventory");

        rewardCounts[rewardIDX]++; // add reward to "inventory"

        // MATCHA: only run this if reward is a "building" reward, new analytics for non-building rewards
        GameManager.Instance.AnalyticsManager.UpdateTotalAcquiredTurrets(rewardIDX);
        GameManager.Instance.UIManager.UpdateInventoryUI(); // MATCHA: is it good practice to call function from another manager?
    }

    // try to use an inventory item, use if item is available
    public bool TryUseInventoryItem(string item_name)
    {
        // if there is no inventory, cannot use item
        if (!isInventoryAvailable(item_name)) return false;

        int rewardIDX = getRewardIDX(item_name);
        rewardCounts[rewardIDX]--;

        // update UI
        GameManager.Instance.UIManager.UpdateInventoryUI();

        // MATCHA: diff logic for non-building inventory items
        int turretIDX0 = getRewardIDX(name);
        GameManager.Instance.AnalyticsManager.UpdateTotalPlacedTurrets(turretIDX0);

        return true; // item has been used

    }

    private void UpdateRewardDisplay(List<GameObject> chosenRewards)
    {
        GameManager.Instance.UIManager.UpdateRewardsUI(chosenRewards[0], chosenRewards[1], chosenRewards[2]);
    }

    // function that returns whether there is inventory of a particular reward available
    public bool isInventoryAvailable(string reward_name) => rewardCounts[getRewardIDX(reward_name)] > 0;

    // get IDX of reward given a rewardName
    public int getRewardIDX(string rewardName) => _rewardMapping.TryGetValue(rewardName, out int index) ? index : -1;
}
