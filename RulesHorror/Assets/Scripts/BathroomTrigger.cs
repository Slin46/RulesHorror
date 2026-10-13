using UnityEngine;

public class BathroomTrigger : MonoBehaviour
{
    public GameObject raNPC;

    private void Start()
    {
        if (raNPC != null)
            raNPC.SetActive(InvestigationProgress.instance != null && InvestigationProgress.instance.raRevealed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || raNPC == null)
            return;
        if (InvestigationProgress.instance == null || !InvestigationProgress.instance.bathroomDoorOpened)
            return;
        if (InvestigationProgress.instance.raRevealed)
            return;

        InvestigationProgress.instance.raRevealed = true;
        raNPC.SetActive(true);
    }
}
