
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class DormEnter : MonoBehaviour
{
    public string roomID;
    public bool requiresPhoneConversation;
    public TextMeshProUGUI promptText;
    public GameObject bob;
    public GameObject blackoutPanel;
    public Transform wakeUpPoint;
    public Renderer doorRenderer;

    private bool isLoading = false;

    private void Start()
    {
        if (!requiresPhoneConversation)
            return;
        if (bob != null)
            bob.SetActive(false);
        if (blackoutPanel != null)
            blackoutPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isLoading)
            return;

        if (GameManager.instance == null)
            return;

        if (requiresPhoneConversation)
        {
            if (InvestigationProgress.instance == null || !InvestigationProgress.instance.phoneConversationFinished || NPCContact.IsTalking)
            {
                if (promptText != null && !NPCContact.IsTalking)
                    promptText.text = "Finish the phone conversation first.";
                return;
            }

            if (InvestigationProgress.instance.bobAtDoor)
            {
                isLoading = true;
                StartCoroutine(BobAtDoor());
                return;
            }
        }

        isLoading = true;

        GameManager.instance.SetTargetRoom(roomID);

        SceneManager.LoadScene(2);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && promptText != null && promptText.text == "Finish the phone conversation first.")
            promptText.text = "";
    }

    private IEnumerator BobAtDoor()
    {
        if (promptText != null)
            promptText.text = "";
        GameManager.instance.setPlayerMovement(true);
        if (doorRenderer != null)
            doorRenderer.enabled = false;
        if (bob != null)
            bob.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        if (blackoutPanel != null)
            blackoutPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        if (bob != null)
            bob.SetActive(false);
        InvestigationProgress.instance.bobAtDoor = false;
        InvestigationProgress.instance.bobVisitFinished = true;
        GameManager.instance.movePlayerOnNPC(wakeUpPoint);
        if (doorRenderer != null)
            doorRenderer.enabled = true;
        if (blackoutPanel != null)
            blackoutPanel.SetActive(false);
        GameManager.instance.setPlayerMovement(false);
        isLoading = false;
    }
}
