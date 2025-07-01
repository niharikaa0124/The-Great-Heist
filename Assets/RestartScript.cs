using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    public GameObject gameElementsRoot;
    public GameObject pauseButton;
    public GameObject restartButton;
    public GameObject startMenuUI;
    public TMPro.TMP_Text countdownText;
    public StartGameManager startGameManager;

    public void RestartGame()
    {
        // ✅ Force restart music state
        if (BackgroundMusicManager.Instance != null)
        {
            var bgAudio = BackgroundMusicManager.Instance.backgroundMusic;

            if (bgAudio != null)
            {
                if (bgAudio.isPlaying)
                    bgAudio.Stop();  // stop kar do agar chal raha ho

                bgAudio.Play();  // fresh se play hoga
            }
        }

        // Reset game UI
        gameElementsRoot.SetActive(false);
        pauseButton.SetActive(false);
        restartButton.SetActive(false);
        startMenuUI.SetActive(true);
        countdownText.text = "";

        Time.timeScale = 1f;

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
