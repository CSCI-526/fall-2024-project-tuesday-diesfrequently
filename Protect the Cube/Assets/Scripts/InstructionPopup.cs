using UnityEngine;
using TMPro;

public class InstructionPopup : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private TextMeshProUGUI instructionText;

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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowModal()
    {
        animator.SetTrigger("ShowModal");
    }

    public void HideModal()
    {
        animator.SetTrigger("HideModal");
    }
}


