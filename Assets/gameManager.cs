using UnityEngine;

public class ModeSelector : MonoBehaviour
{
    public MonoBehaviour mode1Detective;   // e.g., AI script
    public MonoBehaviour mode2Thief;       // e.g., AI script
    public MonoBehaviour mode3Detective;   // 2P
    public MonoBehaviour mode3Thief;       // 2P

    void Start()
    {
        string mode = PlayerPrefs.GetString("GameMode", "DetectiveVsAIThief");

        if (mode == "TwoPlayers")
        {
            mode1Detective.enabled = false;
            mode2Thief.enabled = false;

            mode3Detective.enabled = true;
            mode3Thief.enabled = true;
        }
        else if (mode == "DetectiveVsAIThief")
        {
            mode1Detective.enabled = true;
            mode2Thief.enabled = false;
            mode3Detective.enabled = false;
            mode3Thief.enabled = false;
        }
        else if (mode == "ThiefVsAIDetective")
        {
            mode1Detective.enabled = false;
            mode2Thief.enabled = true;
            mode3Detective.enabled = false;
            mode3Thief.enabled = false;
        }
    }
}
