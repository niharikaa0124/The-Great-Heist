using UnityEngine;
using UnityEngine.EventSystems;

public class PlayClickSound : MonoBehaviour, IPointerClickHandler
{
    public AudioSource clickSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            clickSound.Play();
        }
    }
}
