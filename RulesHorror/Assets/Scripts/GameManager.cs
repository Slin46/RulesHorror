using StarterAssets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public Transform mainStartingTransform;
    public GameObject ThirdPersonRig;
    public Vector3 loadPos;
    public Quaternion loadRot;


    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            //this is a built in action within the scenemanager that requires a listener method 
            //that takes 2 arguments: scene and loadscenemode
            //this action is called whenever a new scene is loaded
            SceneManager.sceneLoaded += OnSceneLoad;
            DontDestroyOnLoad(this);
        }
    }

    //this is the receiver mthod of the scneeloaded action
    //it checks for what scene has been loaded using the scenes buildindex number
    //that number is set in the build profiles for the project
    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            //the below checks to see if data has been loaded into loadpos
            //if loadpos is blank then the game spawns the player at the mainstartingtransform
            //if loadpos has data then the game spawns the player at the loadpos and loadrot spot
            if (loadPos == Vector3.zero)
            {
                //calls the instantation method using dynamic arguments for the position and rotation
                SpawnThirdPersonPrefab(mainStartingTransform.position, mainStartingTransform.rotation);
            }
            else
            {
                SpawnThirdPersonPrefab(loadPos, loadRot);
            }
           
        }
    }

    //a public method we can call from outside this class to instatiate our thirdpersonrig prefab
    //it takes 2 arguments: position and rotation where we want to spawn that thirdpersonrig prefab

    public void SpawnThirdPersonPrefab(Vector3 pos, Quaternion rot)
    {
        //GameObject player = Instantiate(ThirdPersonRig, pos, rot);

        // Make sure the player is active
        //player.SetActive(true);

        // Find the camera even if it was disabled
       // Camera playerCamera = player.GetComponentInChildren<Camera>(true);

        //if (playerCamera != null)
        {
           // playerCamera.gameObject.SetActive(true);

            // Make sure it is the active camera
           // playerCamera.enabled = true;
        }
    }

    //this method is called by the trigger of our loading zones when we exit the scene
    //so that when we reload the 3d demo scene we use the loadpos and loadrot data
    public void loadLocationData(Vector3 pos, Quaternion rot)
    {
        loadPos = pos;
        loadRot = rot;
    }

   
    public void sceneChangeCheck()
    {
        Debug.Log("You've Changed Scenes!");
    }

    //this is a method that is called by startdialogue event in the dialoguerunner class
    //we will toggle trye/false in the event system to call whether we want to move or not
    public void setPlayerMovement(bool b)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        ThirdPersonController tpc = player.GetComponent<ThirdPersonController>();

        if (b)
        {
            tpc.enabled = false;
        }
        else
        {
            tpc.enabled = true;
        }
    }
    public void movePlayerOnNPC(Transform newPos)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = newPos.position;
    }
}
