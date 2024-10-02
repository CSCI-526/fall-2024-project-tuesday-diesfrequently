using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerLevels : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] public int xpNeededForLevel = 2;

    [SerializeField] public List<GameObject> turretOptions = new List<GameObject>();

    [SerializeField] public int currentXP = 0;
    [SerializeField] public int currentLevel = 0;
    private int levels_to_process = 0;

    private bool isSelectingTurret = false;
    private List<GameObject> selectedTurrets = new List<GameObject>();

    
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSelectingTurret && levels_to_process > 0){
            isSelectingTurret = true;
            levels_to_process -=1;
            List<GameObject> turretsOptionsTemp = turretOptions;
            for(int i=0; i<2; i++){
                //Debug.Log(turretsOptionsTemp.Count);
                int index = Random.Range(0, turretsOptionsTemp.Count);
                //Debug.Log(index);
                selectedTurrets.Add(turretsOptionsTemp[index]);

                turretsOptionsTemp.Remove(turretsOptionsTemp[index]);
            }
            //update hud to show the 2 turret options and save the 2 turret options as the values

        }
        if (isSelectingTurret){
            
        }
        
    }

    public void add_exp(int xp_gained){
        currentXP += xp_gained;
        if(currentXP >= xpNeededForLevel)
        {
            while (currentXP >= xpNeededForLevel)
            {
                currentLevel += 1;
                currentXP -= xpNeededForLevel;
                levels_to_process += 1;
                xpNeededForLevel++;
            }
            GameManager.Instance.UIManager.ShowRewardScreen();
            GameManager.Instance.InventoryManager.GenerateRewards();
        }
        GameManager.Instance.UIManager.UpdateUI();
    }
}
