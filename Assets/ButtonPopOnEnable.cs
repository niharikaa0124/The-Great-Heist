using UnityEngine;

public class ButtonPopOnEnable : MonoBehaviour
{
    private Animator animator;

    void OnEnable()
    {
        if (animator == null) animator = GetComponent<Animator>();
        animator.Play("StartButtonBounce", -1, 0f); // Restart the animation
    }
}
