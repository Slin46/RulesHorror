
using TMPro;
using UnityEngine;

public class PasswordBoxController : MonoBehaviour
{
    public GameObject passwordPanel;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;

    public PagePanelController boxPages;

    public void OpenPasswordPanel()
    {
        passwordPanel.SetActive(true);
        passwordInput.text = "";
        messageText.text = "";

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        passwordInput.ActivateInputField();
    }

    public void SubmitPassword()
    {
        if (passwordInput.text == "1211")
        {
            passwordPanel.SetActive(false);

            boxPages.Open();

            if (InvestigationProgress.instance != null)
                InvestigationProgress.instance.boxOpened = true;
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
    }
}
