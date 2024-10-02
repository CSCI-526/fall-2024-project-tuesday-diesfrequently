using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSummaryUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI gameSummary;


    private void OnEnable()
    {
        gameSummary.text = "You Reached Wave " + GameManager.Instance.WaveManager.wave;
    }
}
