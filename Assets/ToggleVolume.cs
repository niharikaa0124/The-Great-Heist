using UnityEngine;
using UnityEngine.UI;

public class ToggleVolume : MonoBehaviour
{
    public Sprite volumeUnmute;         // 🎵 Unmuted icon
    public Sprite volumeMute;           // 🔇 Muted icon
    public Image soundIcon;             // UI icon
    public AudioSource[] allAudioSources; // 🔊 All SFX (exclude backgroundMusic)
    public AudioSource backgroundMusic; // 🎵 Only background music source

    private bool isMuted;

    void Awake()
    {
        // Load mute setting
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
    }

    void Start()
    {
        // 🔁 Re-link background music reference if missing
        if (backgroundMusic == null && BackgroundMusicManager.Instance != null)
        {
            backgroundMusic = BackgroundMusicManager.Instance.backgroundMusic;

            if (backgroundMusic == null)
                Debug.LogWarning("BackgroundMusicManager.Instance is found but backgroundMusic is still null!");
            else
                Debug.Log("BG Music re-linked from BackgroundMusicManager.");
        }

        ApplyVolume();
    }




    public void Toggle()
    {
        isMuted = !isMuted;
        ApplyVolume();

        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ApplyVolume()
    {
        Debug.Log("ApplyVolume called | Muted: " + isMuted);
        Debug.Log("BG Music Null? " + (backgroundMusic == null));

        // SFX
        foreach (AudioSource audio in allAudioSources)
        {
            if (audio != null)
            {
                audio.volume = isMuted ? 0f : 1f;
            }
        }

        // Background music
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = isMuted ? 0f : 1f;

            if (isMuted && backgroundMusic.isPlaying)
            {
                backgroundMusic.Pause();
            }
            else if (!isMuted && !backgroundMusic.isPlaying)
            {
                backgroundMusic.Play();
            }
        }

        // Icon
        if (soundIcon != null)
        {
            soundIcon.sprite = isMuted ? volumeMute : volumeUnmute;
        }
    }
}
