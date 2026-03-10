using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Trigger1_Tutorial : MonoBehaviour
{
    [SerializeField] GameObject shadowIzel;
    private Animator anim;
    [SerializeField] private CircleCollider2D dialogueActivator;
    public void Start()
    {
        //_playerController = FindAnyObjectByType<PlayerController>();
        anim = shadowIzel.GetComponent<Animator>();
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
        yield return new WaitForSeconds(1f);
        anim.Play("Disappear");
        yield return new WaitForSeconds(1f);
        shadowIzel.SetActive(false);
    }
}
