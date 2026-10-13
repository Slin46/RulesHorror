
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PasswordBoxController : MonoBehaviour
{
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;
    public PagePanelController boxPages;
    public PagePanelController passwordControls;

    private bool isCheckingPassword = false;

    public void OpenPasswordPanel()
    {
        if (PagePanelController.IsReading || NPCContact.IsTalking)
            return;
        if (InvestigationProgress.instance != null && InvestigationProgress.instance.boxOpened)
        {
            OpenBoxPages();
            return;
        }
        if (passwordControls == null || passwordInput == null || messageText == null)
            return;
        passwordControls.OpenPanel();
        if (!passwordControls.IsOpen)
            return;

        isCheckingPassword = false;
        passwordInput.text = "";
        messageText.text = "Please enter the password.";

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        passwordInput.ActivateInputField();
        passwordInput.Select();
    }

    private void Update()
    {
        if (passwordPanel != null &&
            passwordPanel.activeInHierarchy &&
            Keyboard.current != null &&
            (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame))
        {
            SubmitPassword();
        }
    }

    public void SubmitPassword()
    {
        if (isCheckingPassword || passwordControls == null || !passwordControls.IsOpen)
            return;

        if (passwordInput == null || messageText == null)
            return;

        string enteredPassword = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(enteredPassword))
        {
            messageText.text = "Please enter the password.";
            passwordInput.ActivateInputField();
            return;
        }

        if (enteredPassword == "1211")
        {
            if (boxPages == null)
            {
                Debug.LogError("Box Pages is not assigned!");
                return;
            }
            isCheckingPassword = true;
            messageText.text = "Password correct!";

            OpenBoxPages();

            if (boxPages.IsOpen && InvestigationProgress.instance != null)
            {
                InvestigationProgress.instance.boxOpened = true;
            }
        }
        else
        {
            messageText.text = "Incorrect password.";
            passwordInput.text = "";
            passwordInput.ActivateInputField();
        }
    }

    private void OpenBoxPages()
    {
        if (boxPages == null || boxPages.panel == null)
            return;
        if (passwordPanel != null && boxPages.panel.transform.IsChildOf(passwordPanel.transform))
        {
            Canvas.ForceUpdateCanvases();
            boxPages.panel.transform.SetParent(passwordPanel.transform.parent, true);
        }
        if (passwordControls != null)
            passwordControls.Close();
        boxPages.Open();
    }

    public void ClosePasswordPanel()
    {
        if (passwordControls != null)
            passwordControls.Close();

        isCheckingPassword = false;
    }
}
