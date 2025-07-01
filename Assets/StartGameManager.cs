using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StartGameManager : MonoBehaviour
{
    public GameObject startMenuUI;
    public TMP_Text countdownText;
    public GameObject gameElementsRoot;
    public GameObject pauseButton;
    public GameObject restartButton;

    public AudioSource countdownAudioSource;
    public AudioSource clickAudioSource;

    public float countdownDelay = 1f;

    public void OnStartButtonClick()
    {
        if (clickAudioSource != null)
            clickAudioSource.Play();  // 🔊 Play click sound

        StartCoroutine(DelayedCountdownStart());
    }

    IEnumerator DelayedCountdownStart()
    {
        yield return new WaitForSeconds(0.25f); // let click sound finish

        // 🎵 Stop background music
        //if (BackgroundMusicManager.Instance != null)
        //    BackgroundMusicManager.Instance.StopMusic();

        startMenuUI.SetActive(false);
        StartCoroutine(PlayCountdown());

        pauseButton.SetActive(true);
        restartButton.SetActive(true);
    }

    public IEnumerator PlayCountdown()
    {
        countdownText.gameObject.SetActive(true);

        if (countdownAudioSource != null)
            countdownAudioSource.Play();

        string[] count = { "3", "2", "1" };

        foreach (string num in count)
        {
            countdownText.text = num;
            countdownText.transform.localScale = Vector3.zero;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                float scale = Mathf.Lerp(0.8f, 1f, t);
                countdownText.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }

            yield return new WaitForSeconds(countdownDelay);
        }

        // ✅ GO!
        countdownText.text = "GO!";
        countdownText.transform.localScale = Vector3.zero;

        float goT = 0f;
        while (goT < 1f)
        {
            goT += Time.deltaTime * 4f;
            float scale = Mathf.Lerp(0.5f, 1.2f, goT);
            countdownText.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        countdownText.text = "";
        countdownText.gameObject.SetActive(false);

        gameElementsRoot.SetActive(true);
        pauseButton.SetActive(true);
        restartButton.SetActive(true);
    }

    public void StartCountdownExternally()
    {
        StartCoroutine(PlayCountdown());
    }
}
