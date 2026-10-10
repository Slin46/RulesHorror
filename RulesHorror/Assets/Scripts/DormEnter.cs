
using UnityEngine;
using UnityEngine.SceneManagement;

public class DormEnter : MonoBehaviour
{
    public string roomID;

    private bool isLoading = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isLoading)
            return;

        if (GameManager.instance == null)
            return;

        isLoading = true;

        GameManager.instance.SetTargetRoom(roomID);

        SceneManager.LoadScene(2);
    }
}
