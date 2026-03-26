using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{

    public Animator animator;

    void Start()
    {
        //Invoke("FadeIn", 0.5f);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        animator.Play("FadeText");
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Game");
    }
}
