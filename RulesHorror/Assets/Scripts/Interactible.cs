
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        Notebook,
        PasswordBox,
        Closet,
        Glasses,
        BathroomDoor
    }

    public InteractionType interactionType;

    [Header("Notebook")]
    public PagePanelController notebookPages;

    [Header("Password Box")]
    public PasswordBoxController passwordBox;

    [Header("Closet")]
    public GameObject openClosetVisual;
    public GameObject glassesVisual;

    [Header("Bathroom")]
    public GameObject candles;
    public GameObject raNPC;

    private bool closetOpened;
    private bool glassesCollected;
    private bool bathroomActivated;

    public void Interact()
    {
        switch (interactionType)
        {
            case InteractionType.Notebook:
                notebookPages.Open();

                if (InvestigationProgress.instance != null)
                    InvestigationProgress.instance.notebookRead = true;
                break;

            case InteractionType.PasswordBox:
                passwordBox.OpenPasswordPanel();
                break;

            case InteractionType.Closet:
                OpenCloset();
                break;

            case InteractionType.Glasses:
                CollectGlasses();
                break;

            case InteractionType.BathroomDoor:
                ActivateBathroom();
                break;
        }
    }

    private void OpenCloset()
    {
        if (closetOpened)
            return;

        if (InventoryManager.instance == null ||
            !InventoryManager.instance.HasItem("Key to Closet"))
        {
            Debug.Log("You need the key to open this closet.");
            return;
        }

        closetOpened = true;

        if (openClosetVisual != null)
            openClosetVisual.SetActive(true);

        if (glassesVisual != null)
            glassesVisual.SetActive(true);
    }

    private void CollectGlasses()
    {
        if (glassesCollected)
            return;

        if (!closetOpened)
            return;

        if (InventoryManager.instance == null)
            return;

        InventoryManager.instance.AddItem("Glasses");
        glassesCollected = true;

        if (glassesVisual != null)
            glassesVisual.SetActive(false);

        gameObject.SetActive(false);
    }

    private void ActivateBathroom()
    {
        if (bathroomActivated)
            return;

        if (InvestigationProgress.instance == null ||
            !InvestigationProgress.instance.HasAllInformation())
        {
            Debug.Log("You still need to investigate the notebook and box.");
            return;
        }

        bathroomActivated = true;

        if (candles != null)
            candles.SetActive(true);

        if (raNPC != null)
            raNPC.SetActive(true);
    }
}
