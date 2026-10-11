
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
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (promptText != null)
            promptText.text = interactText;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (promptText != null)
            promptText.text = "";
    }

    private void Update()
    {
        if (!playerInRange || interactable == null)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Interacting with " + gameObject.name);

            interactable.Interact();
        }
    }
}
