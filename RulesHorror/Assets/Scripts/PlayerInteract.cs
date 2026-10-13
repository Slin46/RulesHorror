
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    public string interactText = "Press E";
    public TextMeshProUGUI promptText;

    private bool playerInRange = false;
    private Interactable interactable;

    private void Start()
    {
        //find the Interactable component on this object.
        interactable = GetComponent<Interactable>();

        if (interactable == null)
        {
            Debug.LogError(
                "No Interactable script found on " + gameObject.name
            );
        }

        if (promptText != null)
            promptText.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled || !other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactable != null && interactable.interactionType == Interactable.InteractionType.BathroomDoor && interactable.BathroomOpened)
            return;

        if (promptText != null)
            promptText.text = GetPrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActiveAndEnabled || !other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (promptText != null && promptText.text == GetPrompt())
            promptText.text = "";
    }

    private void Update()
    {
        if (!playerInRange || interactable == null || PagePanelController.IsReading || NPCContact.IsTalking)
            return;

        if (interactable.interactionType == Interactable.InteractionType.BathroomDoor && !interactable.BathroomOpened && promptText != null)
            promptText.text = GetPrompt();

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Interacting with " + gameObject.name);

            interactable.Interact();
            if (interactable.interactionType == Interactable.InteractionType.BathroomDoor && interactable.BathroomOpened && promptText != null && promptText.text == interactText)
                promptText.text = "";
        }
    }

    private string GetPrompt()
    {
        if (interactable != null && interactable.interactionType == Interactable.InteractionType.BathroomDoor)
        {
            if (interactable.BathroomOpened) return "";
            if (!interactable.BathroomUnlocked) return "Locked";
        }
        return interactText;
    }
}
