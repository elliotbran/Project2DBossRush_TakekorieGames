using UnityEngine;

public class FadeTrigger : MonoBehaviour
{

    public Animator animator;

    void Start()
    {
        //Invoke("FadeIn", 0.5f);
    }

    public void FadeOut()
    {
        animator.Play("FadeOut");
    }
    public void FadeIn()
    {
        animator.Play("FadeIn");
    }
}
