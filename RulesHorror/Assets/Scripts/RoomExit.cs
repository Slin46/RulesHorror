using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class RoomExit : MonoBehaviour
{
    public int sceneIndex;
    public bool requiresRARelease;
    public Collider exitDoorCollider;

    [Header("Optional door key")]
    public string requiredKey;

    [Header("Optional knock quest")]
    public NPCProfile knockQuest;
    public TextMeshProUGUI promptText;

    private readonly HashSet<Collider> playersInRange = new HashSet<Collider>();

    private bool WaitingForJack => knockQuest != null &&
        knockQuest.currentPhase == NPCProfile.dialoguePhase.quest_started;

    private bool IsLocked =>
        (requiresRARelease && (InvestigationProgress.instance == null || !InvestigationProgress.instance.exitUnlocked)) ||
        (!string.IsNullOrEmpty(requiredKey) && (InventoryManager.instance == null || !InventoryManager.instance.HasItem(requiredKey)));

    private void Start()
    {
        UpdateExitDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (WaitingForJack || IsLocked)
            {
                playersInRange.Add(other);
                ShowDoorPrompt();
                return;
            }
            SceneManager.LoadScene(sceneIndex);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        if (playersInRange.Remove(other) && playersInRange.Count == 0 && promptText != null)
            promptText.text = "";
    }

    private void Update()
    {
        UpdateExitDoor();

        if (playersInRange.Count > 0 && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            Knock();
    }

    private void UpdateExitDoor()
    {
        if (!requiresRARelease || exitDoorCollider == null)
            return;

        exitDoorCollider.enabled = IsLocked;
        Renderer doorRenderer = exitDoorCollider.GetComponent<Renderer>();
        if (doorRenderer != null)
            doorRenderer.enabled = IsLocked;
    }

    public void Knock()
    {
        if (playersInRange.Count == 0 || knockQuest == null ||
            knockQuest.currentPhase != NPCProfile.dialoguePhase.quest_started) return;

        knockQuest.currentPhase = NPCProfile.dialoguePhase.quest_completed;
        if (promptText != null)
            promptText.text = "No one answered.";
    }

    private void ShowDoorPrompt()
    {
        if (promptText == null)
            return;
        promptText.text = WaitingForJack
            ? "Press [E] to knock"
            : "Locked";
    }

    private void OnDisable()
    {
        if (playersInRange.Count > 0 && promptText != null)
            promptText.text = "";
        playersInRange.Clear();
    }

}
