using UnityEngine;

public class gameInitializer : MonoBehaviour
{
    public GameObject thief;
    public GameObject detective;

    void Awake()
    {
        Debug.Log("GameInitializer Awake");
    }


    void Start()
    {
        string mode = PlayerPrefs.GetString("GameMode", "");
        Debug.Log("Selected Game Mode: " + mode);

        // Disable all control scripts first
        thief.GetComponent<ThiefController>().enabled = false;
        thief.GetComponent<Mode1ThiefController>().enabled = false;
        detective.GetComponent<DetectivePlayerController>().enabled = false;
        detective.GetComponent<AIDetectiveController>().enabled = false;

        if (mode == "ThiefVsAIDetective")
        {
            thief.GetComponent<Mode1ThiefController>().enabled = true;
            detective.GetComponent<AIDetectiveController>().enabled = true;
        }
        else if (mode == "DetectiveVsAIThief")
        {
            thief.GetComponent<ThiefController>().enabled = true;
            detective.GetComponent<DetectivePlayerController>().enabled = true;
        }
        else if (mode == "TwoPlayers")
        {
            thief.GetComponent<Mode3PlayerThief>().enabled = true;
            detective.GetComponent<Mode3PlayerDetective>().enabled = true;
        }
        else
        {
            Debug.LogError("No game mode selected!");
        }
    }
}
