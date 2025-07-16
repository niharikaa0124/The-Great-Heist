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

    public void SetModeThiefVsAIDetective()
    {
        PlayerPrefs.SetString("GameMode", "ThiefVsAIDetective");
        PlayerPrefs.Save();
    }

    public void SetModeDetectiveVsAIThief()
    {
        PlayerPrefs.SetString("GameMode", "DetectiveVsAIThief");
        PlayerPrefs.Save();
    }

    public void SetModeTwoPlayer()
    {
        PlayerPrefs.SetString("GameMode", "TwoPlayers");   // ✅ Spelling fix: "TwoPlayer"
        PlayerPrefs.Save();
    }

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

        string mode = PlayerPrefs.GetString("GameMode", "");

        if (mode == "ThiefVsAIDetective")
        {
            EnableThiefPlayer();        // Player controls Thief
            EnableDetectiveAI();        // AI controls Detective
        }
        else if (mode == "DetectiveVsAIThief")
        {
            EnableDetectivePlayer();    // Player controls Detective
            EnableThiefAI();            // AI controls Thief
        }
        else if (mode == "TwoPlayers")
        {
            EnableThiefPlayer();
            EnableDetectivePlayer();
        }

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

    void EnableThiefPlayer()
    {
        GameObject thief = GameObject.FindWithTag("Thief");
        if (thief != null)
        {
            var t1 = thief.GetComponent<Mode1ThiefController>();
            var t2 = thief.GetComponent<ThiefController>();
            var t3 = thief.GetComponent<Mode3PlayerThief>();

            if (t1 != null) t1.enabled = false;
            if (t2 != null) t2.enabled = false;
            if (t3 != null) t3.enabled = false;

            string mode = PlayerPrefs.GetString("GameMode");
            if (mode == "TwoPlayers")
            {
                if (t3 != null)
                {
                    Debug.Log("✅ Enabling Mode3PlayerThief");
                    t3.enabled = true;
                }
            }
            else
            {
                if (t1 != null)
                {
                    Debug.Log("✅ Enabling Mode1ThiefController");
                    t1.enabled = true;
                }
            }
        }
    }


    void EnableDetectivePlayer()
    {
        GameObject detective = GameObject.FindWithTag("Detective");
        if (detective != null)
        {
            var d1 = detective.GetComponent<DetectivePlayerController>();
            var d2 = detective.GetComponent<AIDetectiveController>();
            var d3 = detective.GetComponent<Mode3PlayerDetective>();

            if (d1 != null) d1.enabled = false;
            if (d2 != null) d2.enabled = false;
            if (d3 != null) d3.enabled = false;

            string mode = PlayerPrefs.GetString("GameMode");
            if (mode == "TwoPlayers")
            {
                if (d3 != null)
                {
                    Debug.Log("✅ Enabling Mode3PlayerDetective");
                    d3.enabled = true;
                }
            }
            else
            {
                if (d1 != null)
                {
                    Debug.Log("✅ Enabling DetectivePlayerController");
                    d1.enabled = true;
                }
            }
        }
    }



    void EnableThiefAI()
    {
        GameObject thief = GameObject.FindWithTag("Thief");
        if (thief != null)
            thief.GetComponent<ThiefController>().enabled = true;
    }

    void EnableDetectiveAI()
    {
        GameObject detective = GameObject.FindWithTag("Detective");
        if (detective != null)
            detective.GetComponent<AIDetectiveController>().enabled = true;
    }

}
