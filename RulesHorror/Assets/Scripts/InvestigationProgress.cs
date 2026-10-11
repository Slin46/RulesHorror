
using UnityEngine;

public class InvestigationProgress : MonoBehaviour
{
    public static InvestigationProgress instance;

    public bool notebookRead;
    public bool boxOpened;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasAllInformation()
    {
        return notebookRead && boxOpened;
    }
}
