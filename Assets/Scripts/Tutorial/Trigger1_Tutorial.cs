using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Trigger1_Tutorial : MonoBehaviour
{
    [SerializeField] GameObject shadowIzel;
    [SerializeField] GameObject dialogueTrigger;
    [SerializeField] GameObject shadowStairsAbajo;
    [SerializeField] GameObject youDied;
    private Animator anim;
    private Animator _playerAnimator;
    [SerializeField] private CircleCollider2D dialogueActivator;
    PlayerController _playerController;
    public void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        anim = shadowIzel.GetComponent<Animator>();
        _playerAnimator = _playerController.GetComponent<Animator>();
        dialogueActivator = shadowIzel.GetComponent<CircleCollider2D>();
        Debug.Log(dialogueActivator);
    }
    public void TriggerStairs()
    {
        Debug.Log("TriggerStairs Iniciado");
        StartCoroutine(TriggerStairsCoroutine());
    }
    public IEnumerator TriggerStairsCoroutine()
    {
        dialogueActivator.enabled = false;
        dialogueTrigger.SetActive(true);
        yield return new WaitForSeconds(1f);
        anim.Play("Disappear");
        shadowStairsAbajo.SetActive(true);
        yield return new WaitForSeconds(1f);
        shadowIzel.SetActive(false);
    }

    public void irseTutorial()
    {
        StartCoroutine(abandonarAnim());
    }
    public IEnumerator abandonarAnim()
    {
        _playerController.canMove = false;
        yield return new WaitForSeconds(0.7f);
        _playerAnimator.Play("Player_Death");
        yield return new WaitForSeconds(1.5f);
        youDied.SetActive(true);
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }
}
