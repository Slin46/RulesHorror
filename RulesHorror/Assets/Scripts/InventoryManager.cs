
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public TMP_Text inventoryText;

    private Dictionary<string, int> items =
        new Dictionary<string, int>();

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

    //add key to inventory
    public void AddItem(string itemName, int amount = 1)
    {
        if (string.IsNullOrEmpty(itemName))
            return;

        if (items.ContainsKey(itemName))
            items[itemName] += amount;
        else
            items.Add(itemName, amount);

        UpdateUI();

        Debug.Log("Collected " + itemName);
    }

    public bool HasItem(string itemName)
    {
        return items.ContainsKey(itemName) &&
               items[itemName] > 0;
    }

    public void RemoveItem(string itemName, int amount = 1)
    {
        if (!HasItem(itemName))
            return;

        items[itemName] -= amount;

        if (items[itemName] <= 0)
            items.Remove(itemName);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (inventoryText == null)
        {
            Debug.LogError("Inventory Text is NOT assigned!");
            return;
        }

        StringBuilder display = new StringBuilder();

        foreach (var item in items)
        {
            display.AppendLine(item.Key + " x" + item.Value);
        }

        inventoryText.text = display.ToString();

        Debug.Log("CURRENT INVENTORY:\n" + display.ToString());
    }
}
