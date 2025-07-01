
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BreathingEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float scaleAmount = 1.1f;          // How big to scale (breath)
    public float scaleSpeed = 1.5f;           // Speed of breathing
    public float bounceDuration = 0.2f;       // Initial bounce in duration
    public float bounceScale = 1.2f;          // Initial pop scale

    private bool isHovered = false;
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StopAllCoroutines();

        // ✅ Initial bounce
        transform.localScale = originalScale;
        LeanTween.scale(gameObject, originalScale * bounceScale, bounceDuration)
                 .setEaseOutBack()
                 .setOnComplete(() => StartCoroutine(BreatheEffect()));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        StopAllCoroutines();
        LeanTween.scale(gameObject, originalScale, 0.2f).setEaseOutSine();
    }

    IEnumerator BreatheEffect()
    {
        while (isHovered)
        {
            // Scale up (breathe in)
            yield return ScaleTo(originalScale * scaleAmount);
            // Scale down (breathe out)
            yield return ScaleTo(originalScale);
        }
    }

    IEnumerator ScaleTo(Vector3 targetScale)
    {
        float t = 0f;
        Vector3 startScale = transform.localScale;

        while (t < 1f)
        {
            t += Time.deltaTime * scaleSpeed;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
    }
}
