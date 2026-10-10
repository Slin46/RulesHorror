
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Item")]
    public string itemName = "Key to Tom's Room";

    [Header("Collection Settings")]
    public bool collectOnTrigger = true;
    public bool destroyAfterCollecting = true;

    private bool collected = false;

    //on triggercollision destroy the object and add the item to the inventory
    private void OnTriggerEnter(Collider other)
    {
        if (!collectOnTrigger || collected)
            return;

        if (!other.CompareTag("Player"))
            return;

        Collect();
    }

    public void Collect()
    {
        if (collected)
            return;

        if (InventoryManager.instance == null)
        {
            Debug.LogError("InventoryManager not found!");
            return;
        }

        InventoryManager.instance.AddItem(itemName);

        collected = true;

        if (destroyAfterCollecting)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
