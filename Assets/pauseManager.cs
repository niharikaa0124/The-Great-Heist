using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseButton;
    public Sprite pauseIcon;
    public Sprite resumeIcon;

    public AudioSource pauseBeatAudio; // Sound played on pause button click
    public AudioSource backgroundMusic; // Background music
    public AudioSource countdownAudio;

    public Button restartButton;
    public Sprite restartIcon;
    private bool wasCountdownPlaying = false;


    private bool isPaused = false;

    void Start()
    {
        // Reassign backgroundMusic from BackgroundMusicManager (in case scene is restarted)
        if (backgroundMusic == null && BackgroundMusicManager.Instance != null)
        {
            backgroundMusic = BackgroundMusicManager.Instance.backgroundMusic;
        }
    }

    public void TogglePause()
    {
        Debug.Log("TogglePause() called");

        isPaused = !isPaused;

        if (isPaused)
        {
            Debug.Log("Game Paused");

            Time.timeScale = 0f;

            if (BackgroundMusicManager.Instance != null)
            {
                BackgroundMusicManager.Instance.PauseMusic();
                Debug.Log("Background music paused");
            }

            if (countdownAudio != null)
            {
                wasCountdownPlaying = countdownAudio.isPlaying;

                if (countdownAudio.isPlaying)
                {
                    countdownAudio.Pause();
                    Debug.Log("Countdown audio stopped");
                }
            }

            // Play pause button audio
            if (pauseBeatAudio != null)
            {
                pauseBeatAudio.Play();
                Debug.Log("Pause audio played");
            }

            if (pauseButton != null && resumeIcon != null)
            {
                pauseButton.GetComponent<Image>().sprite = resumeIcon;
            }
        }
        else
        {

            Time.timeScale = 1f;

            if (BackgroundMusicManager.Instance != null)
            {
                BackgroundMusicManager.Instance.PlayMusic();
            }

            if (countdownAudio != null && wasCountdownPlaying)
            {
                countdownAudio.UnPause();
                wasCountdownPlaying = false;
            }

            if (pauseBeatAudio != null && pauseBeatAudio.isPlaying)
            {
                pauseBeatAudio.Stop();
            }

            if (pauseButton != null && pauseIcon != null)
            {
                pauseButton.GetComponent<Image>().sprite = pauseIcon;
            }
        }
    }

}
