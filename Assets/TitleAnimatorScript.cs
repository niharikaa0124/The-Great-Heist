
using UnityEngine;

public class TitleAnimatorScript : MonoBehaviour
{
    public Animator titleAnimator;

    void Start()
    {
        titleAnimator.Play("TitlePopUp");
    }
}
