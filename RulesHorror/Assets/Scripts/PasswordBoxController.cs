
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PasswordBoxController : MonoBehaviour
{
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;
    public PagePanelController boxPages;

    private bool isCheckingPassword = false;

    public void OpenPasswordPanel()
    {
        passwordPanel.SetActive(true);

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
            Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SubmitPassword();
        }
    }

    public void SubmitPassword()
    {
        if (isCheckingPassword)
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
            isCheckingPassword = true;
            messageText.text = "Password correct!";

            // Open the newspaper pages immediately.
            passwordPanel.SetActive(false);

            if (boxPages != null)
            {
                boxPages.Open();
            }
            else
            {
                Debug.LogError("Box Pages is not assigned!");
            }

            if (InvestigationProgress.instance != null)
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

    public void ClosePasswordPanel()
    {
        passwordPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        isCheckingPassword = false;
    }
}
