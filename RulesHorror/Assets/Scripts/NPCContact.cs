using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class NPCContact : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public string interactText;
    public bool canInteract;
    public DialogueRunner npcDialogue;
    public NPCCamera npc_cam_rig;
    public NPCProfile storyData;

    public void Start()
    {
        //setting this variable looks a bit static for our system design tastes
        //but we can practice a bit of deliberate design because we know were creating 
        //a systematized npc rig and interaction for multiple instances
        npc_cam_rig = transform.GetComponentInChildren<NPCCamera>();
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
                Debug.Log("Start NPC dialogue");
                setText("");
                npc_cam_rig.movePlayer();
                npcDialogue.StartDialogue(storyData.startingNode);
                canInteract = false;
            }
        }
    }

    //this yarn command/method sets the dialogue phase variable in my yarnspinner script
    //we use this in our loadphase node to route the yarnspinner script to its correct storypath
    [YarnCommand("setPhase")]
    public void setStoryPhase()
    {
        InMemoryVariableStorage vStore = GameObject.FindAnyObjectByType<InMemoryVariableStorage>();
        vStore.SetValue("$dialogPhase", storyData.currentPhase.ToString());
    }

    //this yarncommand/method gets a string from yarnspinner and converts that to the case were on in our npcdata scriptableobject
    //it also contains a nice lil error reporting in case we pass a string that does not correspond to any dialoguephase
    [YarnCommand("getPhase")]
    public void getStoryPhase(string phase)
    {
        if (phase == "comeback")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.comeback;
        }
        else if (phase == "quest_started")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.quest_started;
        }
        else if (phase == "quest_completed")
        {
            storyData.currentPhase = NPCProfile.dialoguePhase.quest_completed;
        }
        else
        {
            Debug.LogError("the string u passed in the yarnscipt does not match any current dialogue phase in the NPCProfile scriptable object");
        }


    }

    public void OnApplicationQuit()
    {
        storyData.dataReset();
    }
}
