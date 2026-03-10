using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Trigger1_Tutorial : MonoBehaviour
{
    [SerializeField] GameObject shadowIzel;
    private Animator anim;
    public void Start()
    {
        //_playerController = FindAnyObjectByType<PlayerController>();
        anim = shadowIzel.GetComponent<Animator>();
    }
    public void TriggerStairs()
    {
        Debug.Log("TriggerStairs Iniciado");
        StartCoroutine(TriggerStairsCoroutine());
    }
    public IEnumerator TriggerStairsCoroutine()
    {
        yield return new WaitForSeconds(1f);
        anim.Play("Disappear");
        yield return new WaitForSeconds(1f);
        shadowIzel.SetActive(false);
    }
}
