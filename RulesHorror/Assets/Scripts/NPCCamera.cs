using Unity.Cinemachine;
using UnityEngine;
using Yarn.Unity;

public class NPCCamera : MonoBehaviour
{
    public Transform npc_loc;
    public Transform pc_loc;
    public Transform pc_move_loc;
    public int pNum;
    public CinemachineCamera npc_cCam;


    [YarnCommand("cam_move")]
    public void cam_move(string locName)
    {
        npc_cCam.Priority = pNum;

        if (locName == "npc")
        {
            npc_cCam.gameObject.transform.position = npc_loc.position;
        }
        else if (locName == "pc")
        {
            npc_cCam.gameObject.transform.position = pc_loc.position;
        }
        else if (locName == "return")
        {
            npc_cCam.Priority = 0;
        }
    }

    public void movePlayer()
    {
        GameManager.instance.movePlayerOnNPC(pc_move_loc);
    }
}
