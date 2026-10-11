
using TMPro;
using UnityEngine;

public class PagePanelController : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] pages;

    public TMP_Text pageText;
    public TMP_Text pageNumberText;
    public GameObject panel;

    private int currentPage = 0;

    public void Open()
    {
        if (pages == null || pages.Length == 0)
            return;

        currentPage = 0;
        panel.SetActive(true);
        ShowPage();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowPage();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage();
        }
    }

    private void ShowPage()
    {
        pageText.text = pages[currentPage];

        if (pageNumberText != null)
            pageNumberText.text =
                (currentPage + 1) + " / " + pages.Length;
    }

    public void Close()
    {
        panel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
