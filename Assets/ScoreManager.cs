using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int moveCount = 0;
    public TextMeshProUGUI moveText;
    public TextMeshProUGUI highScoreText; // Assign this in Inspector
    public TextMeshProUGUI highScoreMessageText;


    private const string HighScoreKey = "HighScore";

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateMoveUI();

        //  Display saved high score at game start
        if (highScoreText != null && PlayerPrefs.HasKey(HighScoreKey))
        {
            int best = PlayerPrefs.GetInt(HighScoreKey);
            highScoreText.text = "High Score: " + best + " Moves";
        }
    }

    public void IncreaseMove()
    {
        moveCount++;
        Debug.Log("Moves: " + moveCount);
        UpdateMoveUI();
    }

    public void UpdateMoveUI()
    {
        if (moveText != null)
            moveText.text = "Moves: " + moveCount;
    }


    public void TryUpdateHighScore()
    {
        int current = moveCount;
        int high = PlayerPrefs.GetInt("HighScore", int.MaxValue);

        if (current < high)
        {
            PlayerPrefs.SetInt("HighScore", current);
            PlayerPrefs.Save();

            if (highScoreMessageText != null)
            {
                highScoreMessageText.text = "New High Score!!!";
                //highScoreMessageText.color = new Color32(255, 215, 0, 255);
                highScoreMessageText.gameObject.SetActive(true);  // ✅ make it visible
            }

            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + current;

                //highScoreMessageText.color = new Color32(255, 215, 0, 255);
            }

            Debug.Log("🎯 New High Score set: " + current);
        }
        else
        {
            if (highScoreMessageText != null)
            {
                highScoreMessageText.text = "";
                highScoreMessageText.gameObject.SetActive(false); // ✅ hide if not high score
            }

            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + high;
            }
        }
    }


    IEnumerator HideHighScoreMessage()
    {
        yield return new WaitForSeconds(3f);
        if (highScoreMessageText != null)
            highScoreMessageText.gameObject.SetActive(false);
    }

}
