using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelectionManager : MonoBehaviour
{
    public Button btnThiefVsAIDetective;
    public Button btnDetectiveVsAIThief;
    public Button btnTwoPlayers;
    public Button btnStartGame;
    public GameObject modeSelection;

    private string selectedMode = "";

    void Start()
    {
        btnStartGame.interactable = false; // 🔴 Initially disabled

        btnThiefVsAIDetective.onClick.AddListener(() => SelectMode("ThiefVsAIDetective"));
        btnDetectiveVsAIThief.onClick.AddListener(() => SelectMode("DetectiveVsAIThief"));
        btnTwoPlayers.onClick.AddListener(() => SelectMode("TwoPlayers"));
        btnStartGame.onClick.AddListener(StartGame);
    }

    void SelectMode(string mode)
    {
        selectedMode = mode;
        btnStartGame.interactable = true; // 🟢 Enable Start Button

        // Optional: Visual feedback
        ResetButtonColors();
        switch (mode)
        {
            case "ThiefVsAIDetective":
                btnThiefVsAIDetective.image.color = Color.green;
                break;
            case "DetectiveVsAIThief":
                btnDetectiveVsAIThief.image.color = Color.green;
                break;
            case "TwoPlayers":
                btnTwoPlayers.image.color = Color.green;
                break;
        }
    }

    void ResetButtonColors()
    {
        btnThiefVsAIDetective.image.color = Color.white;
        btnDetectiveVsAIThief.image.color = Color.white;
        btnTwoPlayers.image.color = Color.white;
    }

    void StartGame()
    {
        if (string.IsNullOrEmpty(selectedMode)) return;

        PlayerPrefs.SetString("GameMode", selectedMode);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene"); // Replace with actual scene
        modeSelection.SetActive(false);
    }
}
