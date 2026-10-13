
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using StarterAssets;
using System.Collections.Generic;

public class PagePanelController : MonoBehaviour
{
    [Header("Page Text")]
    [TextArea(3, 10)]
    public string[] leftPages;

    [TextArea(3, 10)]
    public string[] rightPages;

    public TMP_Text leftPageText;
    public TMP_Text rightPageText;
    public TMP_Text pageNumberText;

    public GameObject panel;
    public Button nextButton;
    public Button previousButton;

    public static bool IsReading { get; private set; }
    private bool isOpen;
    public bool IsOpen => isOpen;
    private readonly List<Behaviour> pausedControllers = new List<Behaviour>();
    private StarterAssetsInputs playerInput;
    private bool previousCursorLocked;
    private bool previousLookInput;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLock;

    private int currentPage = 0;

    public void Open()
    {
        if (IsReading || NPCContact.IsTalking || panel == null || leftPageText == null || leftPages == null || leftPages.Length == 0)
            return;

        OpenPanel();
        currentPage = 0;
        ShowPage();
    }

    public void OpenPanel()
    {
        if (IsReading || NPCContact.IsTalking || panel == null)
            return;
        isOpen = true;
        IsReading = true;
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
                playerInput.move = Vector2.zero;
                playerInput.look = Vector2.zero;
                playerInput.jump = false;
            }
        }
        panel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void NextPage()
    {
        if (isOpen && currentPage < leftPages.Length - 1)
        {
            currentPage++;
            ShowPage();
        }
    }

    public void PreviousPage()
    {
        if (isOpen && currentPage > 0)
        {
            currentPage--;
            ShowPage();
        }
    }

    private void ShowPage()
    {
        leftPageText.text = leftPages[currentPage];

        if (rightPageText != null)
            rightPageText.text = rightPages != null && currentPage < rightPages.Length ? rightPages[currentPage] : "";

        if (nextButton != null)
            nextButton.interactable = currentPage < leftPages.Length - 1;
        if (previousButton != null)
            previousButton.interactable = currentPage > 0;

        if (pageNumberText != null)
            pageNumberText.text =
                (currentPage + 1) + " / " + leftPages.Length;
    }

    public void Close()
    {
        if (!isOpen)
            return;
        isOpen = false;
        IsReading = false;
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
            playerInput.move = Vector2.zero;
            playerInput.look = Vector2.zero;
            playerInput.jump = false;
        }
        Cursor.visible = previousCursorVisible;
        Cursor.lockState = previousCursorLock;
        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
        if (isOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            Close();
    }

    private void OnDisable()
    {
        Close();
    }
}
