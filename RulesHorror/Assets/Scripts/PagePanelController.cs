
using TMPro;
using UnityEngine;

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

    private int currentPage = 0;

    public void Open()
    {
        if (leftPages == null || leftPages.Length == 0)
            return;

        currentPage = 0;
        panel.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        ShowPage();
    }

    public void NextPage()
    {
        if (currentPage < leftPages.Length - 1)
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
        leftPageText.text = leftPages[currentPage];

        if (currentPage < rightPages.Length)
            rightPageText.text = rightPages[currentPage];
        else
            rightPageText.text = "";

        if (pageNumberText != null)
            pageNumberText.text =
                (currentPage + 1) + " / " + leftPages.Length;
    }

    public void Close()
    {
        panel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
