using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource uiSource;
    public AudioSource countdownSource;

    public AudioClip clickSound;
    public AudioClip countdown3;
    public AudioClip countdown2;
    public AudioClip countdown1;
    public AudioClip countdownGo;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayClick() => uiSource.PlayOneShot(clickSound);
    public void PlayCountdown3() => countdownSource.PlayOneShot(countdown3);
    public void PlayCountdown2() => countdownSource.PlayOneShot(countdown2);
    public void PlayCountdown1() => countdownSource.PlayOneShot(countdown1);
    public void PlayCountdownGo() => countdownSource.PlayOneShot(countdownGo);
}
