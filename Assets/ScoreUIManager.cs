using TMPro;
using UnityEngine;

public class ScoreUIManager : MonoBehaviour
{
    public TextMeshProUGUI moveText;

    void Update()
    {
        if (ScoreManager.instance != null)
        {
            moveText.text = "Moves: " + ScoreManager.instance.moveCount;
        }
    }
}
