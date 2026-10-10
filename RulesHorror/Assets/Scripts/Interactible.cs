
using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        CollectItem,
        OpenNotebook,
        PasswordBox,
        SummonNPC,
        Closet
    }

    //all of the items that can be interacted 
    [Header("Interaction")]
    public InteractionType interactionType;
    public string interactionMessage = "Press E";

    [Header("Collect Item")]
    public Collectible collectible;

    [Header("Notebook")]
    public GameObject notebookPanel;

    [Header("Password Box")]
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public string correctPassword = "1211";
    public string passwordRewardItem = "";

    [Header("Summon NPC")]
    public GameObject npcPrefab;
    public Transform npcSpawnPoint;
    public string[] requiredQuestIDs;

    [Header("Closet")]
    public string requiredQuestID = "RaQuest";
    public string closetItemName = "Glasses";

    private bool alreadyUsed = false;

    public void Interact()
    {
        switch (interactionType)
        {
            case InteractionType.CollectItem:
                if (collectible != null)
                    collectible.Collect();
                break;

            case InteractionType.OpenNotebook:
                if (notebookPanel != null)
                    notebookPanel.SetActive(true);
                break;

            case InteractionType.PasswordBox:
                if (passwordPanel != null)
                    passwordPanel.SetActive(true);
                break;

            case InteractionType.SummonNPC:
                SummonNPC();
                break;

            case InteractionType.Closet:
                OpenCloset();
                break;
        }
    }

    public void CheckPassword()
    {
        if (passwordInput == null)
            return;

        if (passwordInput.text == correctPassword)
        {
            Debug.Log("Correct password!");

            if (!string.IsNullOrEmpty(passwordRewardItem))
            {
                InventoryManager.instance.AddItem(
                    passwordRewardItem
                );
            }

            if (passwordPanel != null)
                passwordPanel.SetActive(false);

            alreadyUsed = true;
        }
        else
        {
            Debug.Log("Incorrect password!");
            passwordInput.text = "";
        }
    }

    private void SummonNPC()
    {
        if (alreadyUsed)
            return;

        if (!AllRequiredQuestsComplete())
        {
            Debug.Log("You haven't completed all the required quests.");
            return;
        }

        if (npcPrefab == null || npcSpawnPoint == null)
        {
            Debug.LogWarning("Assign the NPC prefab and spawn point.");
            return;
        }

        Instantiate(
            npcPrefab,
            npcSpawnPoint.position,
            npcSpawnPoint.rotation
        );

        alreadyUsed = true;
    }

    private void OpenCloset()
    {
        if (alreadyUsed)
            return;

        if (!QuestManager.instance.IsQuestComplete(requiredQuestID))
        {
            Debug.Log("You need to finish Ra's quest first.");
            return;
        }

        InventoryManager.instance.AddItem(closetItemName);

        alreadyUsed = true;
        Debug.Log("Collected " + closetItemName);
    }

    private bool AllRequiredQuestsComplete()
    {
        if (requiredQuestIDs == null ||
            requiredQuestIDs.Length == 0)
        {
            return true;
        }

        foreach (string questID in requiredQuestIDs)
        {
            if (!QuestManager.instance.IsQuestComplete(questID))
                return false;
        }

        return true;
    }
}
