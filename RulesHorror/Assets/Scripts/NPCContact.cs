using System;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;
using UnityEngine.SceneManagement;

public class NPCContact : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public string interactText = "Press [E]";
    public bool canInteract;
    public DialogueRunner npcDialogue;
    public NPCProfile storyData;

    private static NPCContact activeContact;
    public static bool IsTalking => activeContact != null;
    private readonly HashSet<Collider> playersInRange = new HashSet<Collider>();
    private readonly List<Behaviour> pausedControllers = new List<Behaviour>();
    private StarterAssetsInputs playerInput;
    private bool previousCursorLocked;
    private bool previousLookInput;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLock;
    private bool talking;

    public void Start()
    {
        if (storyData != null && storyData.currentPhase == NPCProfile.dialoguePhase.departed)
            gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled || !other.CompareTag("Player"))
            return;
        playersInRange.Add(other);
        canInteract = true;
        if (activeContact == null)
            setText(interactText);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!isActiveAndEnabled || !other.CompareTag("Player"))
            return;
        playersInRange.Remove(other);
        canInteract = playersInRange.Count > 0;
        if (!canInteract && activeContact == null)
            setText("");
    }

    public void setText(string txt)
    {
        if (promptText != null)
            promptText.text = txt;
    }

    private void Update()
    {
        if (canInteract && activeContact == null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            StartConversation();
    }

    public async void StartConversation()
    {
        if (PagePanelController.IsReading)
            return;
        if (!isActiveAndEnabled || (storyData != null && storyData.currentPhase == NPCProfile.dialoguePhase.departed))
            return;
        if (activeContact != null || npcDialogue == null || npcDialogue.IsDialogueRunning)
            return;
        if (storyData == null || npcDialogue.YarnProject == null ||
            npcDialogue.YarnProject.Program == null || Array.IndexOf(npcDialogue.YarnProject.NodeNames, storyData.startingNode) < 0)
        {
            Debug.LogError("Assign an NPCProfile and a Yarn Project with the matching starting node on " + name, this);
            return;
        }

        activeContact = this;
        talking = true;
        setText("");
        PausePlayer();
        npcDialogue.onDialogueComplete.AddListener(EndConversation);
        try
        {
            await npcDialogue.StartDialogue(storyData.startingNode);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
            EndConversation();
        }
    }

    private void PausePlayer()
    {
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            foreach (Behaviour controller in player.GetComponentsInChildren<Behaviour>())
            {
                if (controller.enabled && (controller is ThirdPersonController || controller is FirstPersonController))
                {
                    pausedControllers.Add(controller);
                    controller.enabled = false;
                }
            }
            playerInput = player.GetComponentInChildren<StarterAssetsInputs>();
            if (playerInput != null)
            {
                previousCursorLocked = playerInput.cursorLocked;
                previousLookInput = playerInput.cursorInputForLook;
                playerInput.cursorLocked = false;
                playerInput.cursorInputForLook = false;
                playerInput.look = Vector2.zero;
                playerInput.move = Vector2.zero;
                playerInput.jump = false;
            }
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void EndConversation()
    {
        if (!talking)
            return;
        talking = false;
        if (npcDialogue != null)
            npcDialogue.onDialogueComplete.RemoveListener(EndConversation);
        foreach (Behaviour controller in pausedControllers)
        {
            if (controller != null)
                controller.enabled = true;
        }
        pausedControllers.Clear();
        if (playerInput != null)
        {
            playerInput.cursorLocked = previousCursorLocked;
            playerInput.cursorInputForLook = previousLookInput;
            playerInput.look = Vector2.zero;
            playerInput.move = Vector2.zero;
            playerInput.jump = false;
        }
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
        if (activeContact == this)
            activeContact = null;
        setText(canInteract ? interactText : "");
        if (storyData != null && storyData.currentPhase == NPCProfile.dialoguePhase.departed)
            gameObject.SetActive(false);
    }

    [YarnCommand("setPhase")]
    public void setStoryPhase()
    {
        if (npcDialogue != null && storyData != null)
            npcDialogue.VariableStorage.SetValue("$dialogPhase", storyData.currentPhase.ToString());
    }

    [YarnCommand("getPhase")]
    public void getStoryPhase(string phase)
    {
        if (storyData == null)
            return;

        if (phase == "begin")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.begin;
        }
        else if (phase == "comeback")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.comeback;
        }
        else if (phase == "quest_started")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.quest_started;
        }
        else if (phase == "quest_completed")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.quest_completed;
        }
        else if (phase == "reward_given")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.reward_given;
        }
        else if (phase == "departed")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.departed;
        }
        else
        {
            Debug.LogError("Unknown NPC dialogue phase: " + phase, this);
        }
    }

    [YarnCommand("startGlassesQuest")]
    public void StartGlassesQuest()
    {
        if (InvestigationProgress.instance != null)
            InvestigationProgress.instance.glassesQuestStarted = true;
    }

    [YarnCommand("checkGlasses")]
    public void CheckGlasses()
    {
        if (npcDialogue != null)
            npcDialogue.VariableStorage.SetValue("$hasLuisGlasses", InventoryManager.instance != null && InventoryManager.instance.HasItem("Luis glasses"));
    }

    [YarnCommand("finishPhone")]
    public void FinishPhone(bool inviteBob)
    {
        if (InvestigationProgress.instance == null)
            return;

        InvestigationProgress.instance.phoneConversationFinished = true;
        if (inviteBob && !InvestigationProgress.instance.bobVisitFinished)
            InvestigationProgress.instance.bobAtDoor = true;
    }

    [YarnCommand("returnGlasses")]
    public void ReturnGlasses()
    {
        if (InventoryManager.instance == null || InvestigationProgress.instance == null || !InventoryManager.instance.HasItem("Luis glasses"))
            return;

        InventoryManager.instance.RemoveItem("Luis glasses");
        InvestigationProgress.instance.exitUnlocked = true;
        getStoryPhase("reward_given");
    }

    [YarnCommand("gameOver")]
    public void GameOver()
    {
        EndConversation();
        SceneManager.LoadScene("GameOver");
    }

    private void OnDisable()
    {
        EndConversation();
        playersInRange.Clear();
        canInteract = false;
        setText("");
    }

    public void OnApplicationQuit()
    {
        if (storyData != null)
            storyData.dataReset();
    }
}
