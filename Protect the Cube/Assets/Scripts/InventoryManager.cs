using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //stores turret prefabs
    [SerializeField] public List<GameObject> prefabs = new List<GameObject>();
    //stores how many of each type of turret
    [SerializeField] public List<GameObject> gunList = new List<GameObject>();
    [SerializeField] public List<int> buildingCount = new List<int>();
    //stores the names of each type of turret
    [SerializeField] public List<string> buildingNames = new List<string>();

    public PlayerHealth player;
    public Nexus nexus;
    public int healthIncrease = 2;

    private bool firstReward = true;

    private void Start()
    {
        for(int i = 0; i < prefabs.Count; ++i)
        {
            Building building = prefabs[i].GetComponent<Building>();
            string name = "";
            if (building != null)
            {
                name = building.buildingName;
            }
            if (name != "" && name != "Nexus")
            {
                buildingCount.Add(0);
                buildingNames.Add(prefabs[i].GetComponent<Building>().buildingName);
            }
        }
    }

    public List<GameObject> Get3UniqueRewards(List<GameObject> prefab_list){
        List<int> chosen_indexes = new List<int>();
        List<GameObject> chosen_rewards = new List<GameObject>();
        for (int i = 0; i<3; i++){
            do {
                int cur_ind = Random.Range(0, prefab_list.Count);
                if (!chosen_indexes.Contains(cur_ind)){
                    chosen_indexes.Add(cur_ind);
                    chosen_rewards.Add(prefab_list[cur_ind]);
                    break;
                }
                
            } while(true);
        }
        return chosen_rewards;
    }
    public void GenerateRewards()
    {
        List<GameObject> chosen_rewards;
        if (firstReward == true){
            chosen_rewards = Get3UniqueRewards(gunList);
            firstReward = false;

        }
        else{
            chosen_rewards = Get3UniqueRewards(prefabs);


        }
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
            //Debug.Log("Picked " + name);
            GameManager.Instance.UIManager.UpdateInventoryUI();
        }
    }

    public bool CanPlacebuilding(string name)
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
        if(CanPlacebuilding(name))
        {
            int i = buildingNames.IndexOf(name);
            buildingCount[i]--;
            GameManager.Instance.UIManager.UpdateInventoryUI();
            return true;
        }
        return false;
    }

    public void UpdateRewardDisplay(GameObject b1, GameObject b2, GameObject b3)
    {
        GameManager.Instance.UIManager.UpdateRewardsUI(b1, b2, b3);
    }
}
