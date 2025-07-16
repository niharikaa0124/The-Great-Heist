using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;
    public AudioSource backgroundMusic;


    void Start()
    {
        if (backgroundMusic == null)
        {
            Debug.LogError("BackgroundMusic is NULL!");
            return;
        }

        backgroundMusic.Play();
        Debug.Log("🎵 Trying to play BG Music");

        if (backgroundMusic.isPlaying)
            Debug.Log("✅ BG Music is playing");
        else
            Debug.Log("❌ BG Music failed to play");
    }

    void Awake()
    {
        // Prevent duplicate managers
        if (FindObjectsOfType<BackgroundMusicManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Singleton logic
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 🟢 Keeps this alive on restart

            if (backgroundMusic == null)
                backgroundMusic = GetComponent<AudioSource>();

            bool isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
            backgroundMusic.volume = isMuted ? 0f : 1f;

            if (!isMuted && !backgroundMusic.isPlaying)
            {
                backgroundMusic.Play();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }




    public void PlayMusic()
    {
        if (backgroundMusic != null && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void StopMusic()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }
    }

    public void PauseMusic()
    {
        if (backgroundMusic != null && backgroundMusic.isPlaying)
        {
            backgroundMusic.Pause();
        }
    }

    public void SetVolume(bool isMuted)
    {
        backgroundMusic.volume = isMuted ? 0f : 1f;

        if (isMuted)
            backgroundMusic.Pause();
        else
            backgroundMusic.Play();

        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
//using UnityEngine;

//public class BackgroundMusicManager : MonoBehaviour
//{
//    public static BackgroundMusicManager Instance;
//    public AudioSource backgroundMusic;

//    void Awake()
//    {
//        // Prevent duplicates
//        if (FindObjectsOfType<BackgroundMusicManager>().Length > 1)
//        {
//            Destroy(gameObject);
//            return;
//        }

//        // Singleton setup
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);

//            if (backgroundMusic == null)
//                backgroundMusic = GetComponent<AudioSource>();

//            // Play only (volume will be handled by ToggleVolume)
//            if (!backgroundMusic.isPlaying)
//            {
//                backgroundMusic.Play();
//            }
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void PlayMusic()
//    {
//        if (backgroundMusic != null && !backgroundMusic.isPlaying)
//        {
//            backgroundMusic.Play();
//        }
//    }

//    public void StopMusic()
//    {
//        if (backgroundMusic != null && backgroundMusic.isPlaying)
//        {
//            backgroundMusic.Stop();
//        }
//    }

//    public void PauseMusic()
//    {
//        if (backgroundMusic != null && backgroundMusic.isPlaying)
//        {
//            backgroundMusic.Pause();
//        }
//    }
//}
