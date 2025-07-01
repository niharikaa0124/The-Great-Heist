using UnityEngine;

public class DebugReset : MonoBehaviour
{
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        PlayerPrefs.Save();
        Debug.Log("✅ High score reset!");
    }
}
