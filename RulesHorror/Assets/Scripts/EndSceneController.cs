using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    public NPCProfile[] npcProfiles;

    private void Start()
    {
        if (GameManager.instance != null)
            Destroy(GameManager.instance.gameObject);
        if (InventoryManager.instance != null)
            Destroy(InventoryManager.instance.gameObject);
        if (InvestigationProgress.instance != null)
            Destroy(InvestigationProgress.instance.gameObject);
        if (QuestManager.instance != null)
            Destroy(QuestManager.instance.gameObject);

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(player.transform.root.gameObject);
        }

        foreach (NPCProfile profile in npcProfiles)
        {
            if (profile != null)
                profile.dataReset();
        }

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene("StartScreen");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
