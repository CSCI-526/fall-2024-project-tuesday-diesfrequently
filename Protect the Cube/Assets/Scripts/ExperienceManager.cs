using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    int currentLevel, totalExperience;
    int previousLevelsExperience, nextLevelsExperience;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] Image experienceFill;

    void Start()
    {

    }

    void Update() 
    {
    
    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience; 

        levelText.text = currentLevel.ToString();
        experienceText.text = start + " exp / " + end + " exp";
        experienceFill.fillAmount = (float)start / (float)end;
    }
}