using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public string interactText;
    public bool canInteract;

    public void Start()
    {
       
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            setText(interactText);
            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            setText("");
            canInteract = false;
        }
    }

    public void setText(string txt)
    {
        promptText.text = txt;
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Debug.Log("Start Interaction");
                
                canInteract = false;
            }
        }
    }
}
