using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SegundoTutorial : MonoBehaviour
{
    public GameObject izelComoMil;
    public GameObject izelTuto2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void segundoTutorial()
    {
        StartCoroutine(segundoTuto());
    }
    public IEnumerator segundoTuto()
    {
        yield return new WaitForSeconds(1);
        izelComoMil.GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(1);
        izelComoMil.SetActive(false);
        izelTuto2.SetActive(true);
    }
}
