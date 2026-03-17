using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SegundoTutorial : MonoBehaviour
{
    public GameObject izelComoMil;
    public GameObject izelTuto2;
    EnemyTutorialController enemyTutorialController;
    UITutorialControl uiTutorialControl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyTutorialController = izelTuto2.GetComponent<EnemyTutorialController>();
        uiTutorialControl = FindAnyObjectByType<UITutorialControl>();
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
        izelComoMil.GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForSeconds(1);
        izelComoMil.GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(1);
        izelComoMil.SetActive(false);
        izelTuto2.SetActive(true);
        StartCoroutine(uiTutorialControl.tutorialParryCoroutine());
        yield return new WaitForSeconds(3f);
        enemyTutorialController.rangeAttackRange = 10;
    }
}
