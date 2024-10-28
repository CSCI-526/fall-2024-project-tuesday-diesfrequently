// manages players inventory (turret prefabs + tracking each turret type)
// reward generation
// update player health + player ui

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // stores different combinations of reward prefabs
    [SerializeField] public List<GameObject> all_reward_prefabs = new List<GameObject>(); 

    //stores how many of each type of turret
    [SerializeField] public List<int> buildingCount = new List<int>();

    //stores the names of each type of turret
    [SerializeField] public List<string> buildingNames = new List<string>();

    // stores a progressive expansion of which rewards the player has access too
    private HashSet<GameObject> available_rewards = new HashSet<GameObject>();

    // Dictionary for String - Int Mapping of Buildings
    private Dictionary<string, int> buildingMapping = new Dictionary<string, int>()
    {
        { "Gun Turret", 1 },
        { "Gatling Turret", 2 },
        { "Flamethrower Turret", 3 },
        { "Sniper Turret", 4 },
        { "Turret Booster", 5 },
        { "Harvester", 6 },
        { "Slow Tower", 7 }
    };

    public PlayerHealth player;
    public Nexus nexus;
    public int healthIncrease = 2;
    private int rewardsPicked = 0;

    private void Start()
    {
        for(int i = 0; i < all_reward_prefabs.Count; ++i)
        {
            // Order: Gun Turret, Gatling Turret, Flamethrower Turret, Sniper Turret, Turret Booster, Harvester, Slow Tower
            Building building = all_reward_prefabs[i].GetComponent<Building>();
            
            string name = "";
            if (building != null)
            {
                name = building.buildingName;
            }

            if (name != "" && name != "Nexus")
            {
                buildingCount.Add(0);
                buildingNames.Add(all_reward_prefabs[i].GetComponent<Building>().buildingName);
            }
        }
    }

    public int ConvertBuildingNameToIndex(string buildingName)
    {
        // Check if the building name exists in the dictionary
        if (buildingMapping.TryGetValue(buildingName, out int index))
        {
            return index; // Return the associated index
        }
        else
        {
            Debug.LogWarning($"Building name '{buildingName}' not found in the mapping.");
            return -1; // Return -1 or any other default value to indicate not found
        }
    }

    public List<GameObject> GenerateUniqueRewards(List<GameObject> rewardsList, GameObject forceGameObject = null, int forceGameObjectCount = 0)
    {
        // check that ForceValueCount is only between 0 and 3
        forceGameObjectCount = Mathf.Clamp(forceGameObjectCount, 0, 3);

        // output list of rewards
        List<GameObject> finalRewards = new List<GameObject>();

        // optional argument to "force" a certain GameObject Reward to appear forceGameObjectCount times
        if (forceGameObject != null)
        {
            for (int i = 0; i < forceGameObjectCount; i++)
            {
                finalRewards.Add(forceGameObject);
            }
        }

        // calculate remaining spots left
        int remainingSlots = 3 - finalRewards.Count;

        if (remainingSlots > 0)
        {
            // filter out the selected rewards from available_rewards
            List<GameObject> availableRewards = rewardsList
                .Where(r => r != forceGameObject)   // Exclude forcedGameObject
                .OrderBy(x => Random.value)         // Shuffle the list
                .Take(remainingSlots)               // anywhere from 0 to 3 more rewards
                .ToList();

            // add the remaining unpicked rewards to finalRewards
            finalRewards.AddRange(availableRewards);
        }

        // Edge Case: originally less than 3 reward types avaialble
        while (finalRewards.Count < 3)
        {
            if (finalRewards.Count > 0)
            {
                finalRewards.Add(finalRewards[0]); // Add more of the first element
            }

        }

        // return the 3 randomly picked / shuffled rewards as a list
        return finalRewards.ToList();
    }

    public void GenerateRewards()
    {
        // get player level
        int playerLevel = GameManager.Instance.Player.GetComponent<PlayerLevels>().currentLevel;

        // to store the generated rewards
        GameObject forcedGameObject = null;
        int forceGameObjectCount = 0;

        if (playerLevel == 1) // Level 1: Player exposed to gun turret
        {
            int gunTurretIdx = buildingNames.IndexOf("Gun Turret");
            forcedGameObject = all_reward_prefabs[gunTurretIdx];
            forceGameObjectCount = 1;
            available_rewards.Add(all_reward_prefabs[gunTurretIdx]);
        }
        else if (playerLevel == 2) // Level 2: Player exposed to gatling turret
        {
            int gatlingTurretIdx = buildingNames.IndexOf("Gatling Turret");
            forcedGameObject = all_reward_prefabs[gatlingTurretIdx];
            forceGameObjectCount = 1;
            available_rewards.Add(all_reward_prefabs[gatlingTurretIdx]);
        }
        else if (playerLevel == 3) // Level 3: Player exposed to flamethrower
        {
            int flamethrowerIdx = buildingNames.IndexOf("Flamethrower Turret");
            forcedGameObject = all_reward_prefabs[flamethrowerIdx];
            forceGameObjectCount = 1;
            available_rewards.Add(all_reward_prefabs[flamethrowerIdx]);
        }
        else if (playerLevel % 5 == 0) // Level 5: Player gets a Harvestor every 5 levels
        {
            int harvesterIndex = buildingNames.IndexOf("Harvester");
            available_rewards.Add(all_reward_prefabs[harvesterIndex]);
            forcedGameObject = all_reward_prefabs[harvesterIndex];
            if (playerLevel == 5)
            {
                forceGameObjectCount = 3; // force 3 harvestors in lvl 5
            }
        }
        else if (playerLevel == 7) // Level 7: Player gets access to slow towers
        {
            int slowTowerIdx = buildingNames.IndexOf("Slow Tower");
            forcedGameObject = all_reward_prefabs[slowTowerIdx];
            forceGameObjectCount = 1;
            available_rewards.Add(all_reward_prefabs[slowTowerIdx]);
        }
        else if (playerLevel == 8) // Level 8: Player gets Turret Booster
        {
            int turretBoosterIdx = buildingNames.IndexOf("Turret Booster");
            forcedGameObject = all_reward_prefabs[turretBoosterIdx];
            forceGameObjectCount = 1;
            available_rewards.Add(all_reward_prefabs[turretBoosterIdx]);
        }

        // other building names: Sniper Turret, Turret Booster

        //Debug.Log("Size of Available_Rewards: " + available_rewards.ToList().Count());
        //Debug.Log("forcedGameObject: " + forcedGameObject);
        //Debug.Log("forceGameObjectCount: " + forceGameObjectCount);


        // pick 3 random rewards from what is available and display
        List<GameObject> chosen_rewards = GenerateUniqueRewards(available_rewards.ToList(), forcedGameObject, forceGameObjectCount);

        //Debug.Log("Size of Chosen Items: " + chosen_rewards.Count);
        //if (chosen_rewards[0] == null)
        //{
        //    Debug.Log(" Chosen 0 is NULL");
        //}
        //if (chosen_rewards[1] == null)
        //{
        //    Debug.Log(" Chosen 1 is NULL");
        //}
        //if (chosen_rewards[2] == null)
        //{
        //    Debug.Log(" Chosen 2 is NULL");
        //}

        string reward_name_0 = chosen_rewards[0].GetComponent<Building>().buildingName;
        string reward_name_1 = chosen_rewards[1].GetComponent<Building>().buildingName;
        string reward_name_2 = chosen_rewards[2].GetComponent<Building>().buildingName;

        //Debug.Log("Chosen Reward 0: " + reward_name_0);
        //Debug.Log("Chosen Reward 1: " + reward_name_1);
        //Debug.Log("Chosen Reward 2: " + reward_name_2);

        int rewardIDX0 = ConvertBuildingNameToIndex(reward_name_0);
        int rewardIDX1 = ConvertBuildingNameToIndex(reward_name_1);
        int rewardIDX2 = ConvertBuildingNameToIndex(reward_name_2);

        // Analytics Update
        // Order: Gun Turret, Gatling Turret, Flamethrower Turret, Sniper Turret, Turret Booster, Harvester, Slow Tower
        GameManager.Instance.AnalyticsManager.UpdateRewardsOffered(rewardIDX0, rewardIDX1, rewardIDX2);

        UpdateRewardDisplay(chosen_rewards[0], chosen_rewards[1], chosen_rewards[2]);
    }

    public void PickReward(string name)
    {
        if (name.Contains("HP"))
        {
            if (name == "Player HP")
            {
                player.currentHealth += healthIncrease;
                if (player.currentHealth > player.maxHealth)
                {
                    player.currentHealth = player.maxHealth;
                }
            }
            else if (name == "Nexus HP")
            {
                nexus.health += healthIncrease;
                if (nexus.health > nexus.maxHealth)
                {
                    nexus.health = nexus.maxHealth;
                }
            }
            GameManager.Instance.UIManager.UpdateUI();
        }
        else
        {
            int i = buildingNames.IndexOf(name);
            buildingCount[i]++;
            int turretIDX0 = ConvertBuildingNameToIndex(name);
            GameManager.Instance.AnalyticsManager.UpdateTotalAcquiredTurrets(turretIDX0);
            GameManager.Instance.UIManager.UpdateInventoryUI();
        }
        if (rewardsPicked == 0){
            GameManager.Instance.UIManager.ShowSelectGunTutorial();
            rewardsPicked += 1;
        }
    }

    public bool HasBuilding(string name)
    {
        int i = buildingNames.IndexOf(name);
        if (buildingCount[i] > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryPlaceBuilding(string name)
    {
        if(HasBuilding(name))
        {
            int i = buildingNames.IndexOf(name);
            buildingCount[i]--;
            GameManager.Instance.UIManager.UpdateInventoryUI();
            int turretIDX0 = ConvertBuildingNameToIndex(name);
            GameManager.Instance.AnalyticsManager.UpdateTotalPlacedTurrets(turretIDX0);
            return true;
        }
        return false;
    }

    public void UpdateRewardDisplay(GameObject b1, GameObject b2, GameObject b3)
    {
        GameManager.Instance.UIManager.UpdateRewardsUI(b1, b2, b3);
    }
}
