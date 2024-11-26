using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InstructionPopup : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI escapeText;
    private RectTransform rectTransform;

    // new variables
    [SerializeField] private VerticalLayoutGroup layoutGroup;
    [SerializeField] private ContentSizeFitter contentSizeFitter;

    public enum ModalType
    {
        FullScreen,
        BottomBar
    }

    [Range(0.65f, 0.85f)]
    public float topHeightPercentage = 0.65f; // For BottomBar modal
    [Range(0.65f, 0.85f)]
    public float bottomHeightPercentage = 0.85f; // For BottomBar modal

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();
        layoutGroup = GetComponentInChildren<VerticalLayoutGroup>();
        contentSizeFitter = GetComponentInChildren<ContentSizeFitter>();
    }

    public void ShowInstruction(string message)
    {
        instructionText.text = message;
        gameObject.SetActive(true);

        //animator.Play("ModalFadeIn");
    }

    public void HideInstruction()
    {
        //animator.Play("ModalFadeOut");
        gameObject.SetActive(false); 
    }

    // Configure the modal dynamically
    public void ConfigureModal(ModalType modalType)
    {
        switch (modalType)
        {
            case ModalType.FullScreen:
                SetFullScreenModal();
                break;
            case ModalType.BottomBar:
                SetBottomBarModal();
                break;
        }
    }

    // Full-screen modal setup
    private void SetFullScreenModal()
    {
        rectTransform.anchorMin = new Vector2(0, 0); // Full screen bottom-left
        rectTransform.anchorMax = new Vector2(1, 1); // Full screen top-right
        rectTransform.offsetMin = Vector2.zero; 
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Text auto size settings
        instructionText.enableAutoSizing = true;
        instructionText.fontSizeMin = 10; // Minimum font size
        instructionText.fontSizeMax = 50; // Maximum font size

        // Layout setup
        layoutGroup.enabled = false;
        contentSizeFitter.enabled = true;

    }

    // Bottom-bar modal setup
    private void SetBottomBarModal()
    {
        rectTransform.anchorMin = new Vector2(0, 1 - bottomHeightPercentage); // Bottom boundary
        rectTransform.anchorMax = new Vector2(1, 1 - topHeightPercentage);   // Top boundary
        rectTransform.offsetMin = Vector2.zero; 
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Text auto size settings
        instructionText.enableAutoSizing = true;
        instructionText.fontSizeMin = 10; // Minimum font size
        instructionText.fontSizeMax = 20; // Maximum font size

        // Text auto size settings
        escapeText.enableAutoSizing = true;
        escapeText.fontSizeMin = 8; // Minimum font size
        escapeText.fontSizeMax = 13; // Maximum font size

        // Layout setup
        layoutGroup.enabled = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        contentSizeFitter.enabled = true;

        layoutGroup.childAlignment = TextAnchor.MiddleCenter; // Center the content within the modal
    }

    // Example Use
    //instructionPopup.ConfigureModal(InstructionPopup.ModalType.FullScreen);
    //instructionPopup.ShowInstruction("This is a full-screen message!");

    //instructionPopup.ConfigureModal(InstructionPopup.ModalType.BottomBar);
    //instructionPopup.ShowInstruction("This is a bottom-bar message!");


    public void ShowModal()
    {
        animator.SetTrigger("ShowModal");
    }

    public void HideModal()
    {
        animator.SetTrigger("HideModal");
    }
}


