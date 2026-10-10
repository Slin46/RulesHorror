
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    private HashSet<string> completedQuests =
        new HashSet<string>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void CompleteQuest(string questID)
    {
        if (string.IsNullOrEmpty(questID))
            return;

        completedQuests.Add(questID);

        Debug.Log("Quest completed: " + questID);
    }

    public bool IsQuestComplete(string questID)
    {
        return completedQuests.Contains(questID);
    }

    public void RewardItem(string itemName)
    {
        if (InventoryManager.instance != null)
            InventoryManager.instance.AddItem(itemName);
    }
}
