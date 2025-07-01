using UnityEngine;
using System.Collections;

public class InstructionManager : MonoBehaviour
{
    public GameObject instructionPanel;
    public AudioSource clickAudioSource;  // 🎵 Assign this in Inspector

    // 📜 Call this from Start Button or Instruction Button
    public void ShowInstructions()
    {
        StartCoroutine(PlayClickAndShow());
    }

    public void HideInstructions()
    {
        StartCoroutine(PlayClickAndHide());
    }

    IEnumerator PlayClickAndShow()
    {
        if (clickAudioSource != null)
            clickAudioSource.Play();

        yield return new WaitForSeconds(0.05f); // Small delay

        instructionPanel.SetActive(true);
    }

    IEnumerator PlayClickAndHide()
    {
        if (clickAudioSource != null)
            clickAudioSource.Play();

        yield return new WaitForSeconds(0.05f); // Small delay

        instructionPanel.SetActive(false);
    }
}
