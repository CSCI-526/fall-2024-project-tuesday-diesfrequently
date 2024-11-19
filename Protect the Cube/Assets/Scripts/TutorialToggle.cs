using UnityEngine;
using UnityEngine.UI;

public class TutorialToggle : MonoBehaviour
{
    public Toggle tutorialToggle; // toggle component from Main Menu
    private const string TutorialStorageKey = "IsTutorialEnabled";

    void Start()
    {
        // Retrieve current "stored" value of the tutorial
        bool TutorialStorageValue = PlayerPrefs.GetInt(TutorialStorageKey, 1) == 1;
        tutorialToggle.isOn = TutorialStorageValue;

        // Function that activates each time the toggle value is clicked
        tutorialToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        // stores TutorialStorageValue (isOn) into PlayerPrefs
        Debug.Log("Value of TutorialToggle: " + isOn);
        PlayerPrefs.SetInt(TutorialStorageKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnDestroy() { tutorialToggle.onValueChanged.RemoveListener(OnToggleValueChanged); }
}