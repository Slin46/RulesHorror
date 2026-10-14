
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public Transform mainStartingTransform;
    public GameObject ThirdPersonRig;

    public Vector3 loadPos;
    public Quaternion loadRot;

    private string targetRoomID = "";
    private bool useDirectPosition = false;
    private bool hasPlacedInitialPlayer = false;

    private Coroutine placementCoroutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoad;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            DontDestroyOnLoad(player);
        }
    }

    private void Start()
    {
        if (!hasPlacedInitialPlayer)
        {
            placementCoroutine = StartCoroutine(
                PlacePlayerAfterSceneLoads(SceneManager.GetActiveScene())
            );
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
            instance = null;
        }
    }

    public void SetTargetRoom(string roomID)
    {
        targetRoomID = roomID;
        useDirectPosition = false;

        Debug.Log("Target room: " + roomID);
    }

    public void loadLocationData(Vector3 pos, Quaternion rot)
    {
        loadPos = pos;
        loadRot = rot;

        useDirectPosition = true;
        targetRoomID = "";
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (placementCoroutine != null)
        {
            StopCoroutine(placementCoroutine);
        }

        placementCoroutine = StartCoroutine(
            PlacePlayerAfterSceneLoads(scene)
        );
    }

    private IEnumerator PlacePlayerAfterSceneLoads(Scene scene)
    {
        yield return null;
        yield return new WaitForEndOfFrame();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError(
                "GameManager: Cannot find a GameObject tagged Player. " +
                "Make sure your existing player persists between scenes."
            );
            yield break;
        }

        if (useDirectPosition)
        {
            MovePlayer(player, loadPos, loadRot);

            useDirectPosition = false;
            hasPlacedInitialPlayer = true;
            yield break;
        }

        if (!string.IsNullOrEmpty(targetRoomID))
        {
            RoomEnter[] entrances =
                FindObjectsByType<RoomEnter>(
                    FindObjectsSortMode.None
                );

            foreach (RoomEnter entrance in entrances)
            {
                if (entrance.gameObject.scene != scene)
                    continue;

                if (entrance.roomID == targetRoomID)
                {
                    MovePlayer(
                        player,
                        entrance.transform.position,
                        entrance.transform.rotation
                    );

                    Debug.Log("Player placed at room: " + targetRoomID);

                    targetRoomID = "";
                    hasPlacedInitialPlayer = true;
                    yield break;
                }
            }

            Debug.LogError(
                "No RoomEnter found with ID: " + targetRoomID +
                " in scene: " + scene.name
            );

            yield break;
        }

        if (!hasPlacedInitialPlayer && mainStartingTransform != null)
        {
            MovePlayer(
                player,
                mainStartingTransform.position,
                mainStartingTransform.rotation
            );

            hasPlacedInitialPlayer = true;
        }
    }

    private void MovePlayer(
        GameObject player,
        Vector3 position,
        Quaternion rotation)
    {
        CharacterController cc =
            player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.transform.SetPositionAndRotation(position, rotation);

        if (cc != null)
            cc.enabled = true;
    }

    public void SpawnThirdPersonPrefab(Vector3 pos, Quaternion rot)
    {
        Debug.Log("Using the existing player instead of spawning a new one.");
    }

    public void sceneChangeCheck()
    {
        Debug.Log("You've Changed Scenes!");
    }

    public void setPlayerMovement(bool b)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        ThirdPersonController tpc =
            player.GetComponent<ThirdPersonController>();

        if (tpc != null)
            tpc.enabled = !b;
    }

    public void movePlayerOnNPC(Transform newPos)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && newPos != null)
        {
            MovePlayer(
                player,
                newPos.position,
                newPos.rotation
            );
        }
    }
}
