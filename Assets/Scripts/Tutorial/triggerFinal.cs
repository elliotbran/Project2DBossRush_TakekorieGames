using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class triggerFinal : MonoBehaviour
{
    PlayerController player;
    public GameObject fade;
    private void Start()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(BomboBOmbastico());
        }
    }
    IEnumerator BomboBOmbastico()
    {
        player.canMove = false;
        yield return new WaitForSeconds(.1f);
        fade.SetActive(true);
        fade.GetComponent<Animator>().Play("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Game");
    }
}
