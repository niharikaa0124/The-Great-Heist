using UnityEngine;

public class TitleAnimationTrigger : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("TitlePopUp");
        }
        else
        {
            Debug.LogWarning("Animator not found on GameTitle object.");
        }
    }
}
