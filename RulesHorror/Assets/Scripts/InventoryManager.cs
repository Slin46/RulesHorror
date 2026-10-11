
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [Header("Inventory UI")]
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

        //preserve the manager and all its children between scenes.
        DontDestroyOnLoad(gameObject);

        //listen for scene changes.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //give the new scene a frame to initialize.
        CancelInvoke(nameof(FindInventoryText));
        Invoke(nameof(FindInventoryText), 0.1f);
    }

    private void FindInventoryText()
    {
        //keep the existing reference if it is still valid.
        if (inventoryText != null)
        {
            UpdateUI();
            return;
        }

        //find a TextMeshPro UI object in the new scene.
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(
            FindObjectsSortMode.None
        );

        foreach (TMP_Text text in texts)
        {
            if (text.gameObject.name == "InventoryText")
            {
                inventoryText = text;
                UpdateUI();
                Debug.Log("Inventory text connected!");
                return;
            }
        }

        Debug.LogWarning(
            "InventoryText not found in scene: " + SceneManager.GetActiveScene().name
        );
    }

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
            return;

        StringBuilder display = new StringBuilder();

        foreach (var item in items)
        {
            display.AppendLine(item.Key + " x" + item.Value);
        }

        inventoryText.text = display.ToString();
    }
}
