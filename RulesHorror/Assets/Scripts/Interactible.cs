
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
    public Collider doorCollider;
    public bool requiresRAQuest;

    private bool closetOpened;
    private bool glassesCollected;
    private bool bathroomActivated;

    public bool BathroomUnlocked => InvestigationProgress.instance != null &&
        (requiresRAQuest ? InvestigationProgress.instance.glassesQuestStarted : InvestigationProgress.instance.boxOpened);
    public bool BathroomOpened => bathroomActivated;

    private void Start()
    {
        if (interactionType != InteractionType.BathroomDoor)
            return;

        bathroomActivated = InvestigationProgress.instance != null &&
            (requiresRAQuest ? InvestigationProgress.instance.glassesQuestStarted : InvestigationProgress.instance.bathroomDoorOpened);
        UpdateBathroomDoor();
    }

    private void Update()
    {
        if (interactionType == InteractionType.BathroomDoor && requiresRAQuest && !bathroomActivated && BathroomUnlocked)
            ActivateBathroom();
    }

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

        Renderer closetRenderer = GetComponent<Renderer>();
        if (closetRenderer != null)
            closetRenderer.enabled = false;

        foreach (Collider closetCollider in GetComponents<Collider>())
        {
            if (!closetCollider.isTrigger)
                closetCollider.enabled = false;
        }

        if (openClosetVisual != null)
            openClosetVisual.SetActive(true);

        if (glassesVisual != null)
            glassesVisual.SetActive(!InventoryManager.instance.HasItem("Luis glasses"));
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

        if (!BathroomUnlocked || doorCollider == null)
            return;

        bathroomActivated = true;
        UpdateBathroomDoor();

        if (!requiresRAQuest)
            InvestigationProgress.instance.bathroomDoorOpened = true;
    }

    private void UpdateBathroomDoor()
    {
        if (doorCollider == null)
            return;

        doorCollider.enabled = !bathroomActivated;

        Renderer doorRenderer = doorCollider.GetComponent<Renderer>();
        if (doorRenderer != null)
            doorRenderer.enabled = !bathroomActivated;
    }
}
