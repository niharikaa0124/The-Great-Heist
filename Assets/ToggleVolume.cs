using UnityEngine;
using UnityEngine.UI;

public class ToggleVolume : MonoBehaviour
{
    public Sprite volumeUnmute;   // 🎵 Unmuted icon
    public Sprite volumeMute;     // 🔇 Muted icon
    public Image soundIcon;       // 🔁 Directly assign in Inspector
    public AudioSource[] allAudioSources;

    private bool isMuted;

    void Awake()
    {
        // Load mute setting
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
    }

    void Start()
    {
        ApplyVolume();
    }

    public void Toggle()
    {
        isMuted = !isMuted;
        ApplyVolume();

        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ApplyVolume()
    {
        foreach (AudioSource audio in allAudioSources)
        {
            if (audio != null)
                audio.volume = isMuted ? 0f : 1f;
        }

        if (soundIcon != null)
        {
            soundIcon.sprite = isMuted ? volumeMute : volumeUnmute;
        }
    }
}
