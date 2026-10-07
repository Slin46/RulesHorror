using UnityEngine;
using UnityEngine.SceneManagement;

public class DormEnter : MonoBehaviour
{
    public Transform dormReturn;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.instance.loadLocationData(dormReturn.position, dormReturn.rotation);
            //GO TO 'NEXT' SCENE
            SceneManager.LoadScene(1);
        }
    }
}
