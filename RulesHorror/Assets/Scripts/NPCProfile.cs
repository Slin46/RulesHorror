using UnityEngine;

[CreateAssetMenu(fileName = "NPCProfile", menuName = "NPCData")]
public class NPCProfile : ScriptableObject
{
    public string npcName;
    public string startingNode;
    public enum dialoguePhase { comeback, begin, quest_started, quest_completed, reward_given }
    public dialoguePhase currentPhase;

    public void dataReset()
    {
        currentPhase = dialoguePhase.begin;
    }
}
