using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardInfo : MonoBehaviour
{
    [SerializeField] public string RewardName = "missing name";
    [SerializeField] public string RewardDescription = "missing description";
    [SerializeField] public Sprite RewardImage;
    [SerializeField] public bool isStatBased = false;
    [SerializeField] public int rangeDesc = 0;
    [SerializeField] public int damageDesc = 0;
    [SerializeField] public int firerateDesc = 0;
}
