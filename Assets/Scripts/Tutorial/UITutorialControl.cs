using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class UITutorialControl : MonoBehaviour
{
    [Header("Ints to count")]
    public int hitsTutorial;

    [Header("Hud")]
    public GameObject tutorialHud1;
    public TMP_Text textoTuto1;


    [Header ("Chapuza")]
    [SerializeField] EnemyTutorialController enemyTutorialController;
    [SerializeField] GameObject EnemyTutorial1;
    [SerializeField] GameObject izel3Shadow;

    bool corrutineRunning;
    public CapsuleCollider2D miCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Esto busca el collider en el objeto o en cualquiera de sus hijos
        miCollider = EnemyTutorial1.GetComponentInChildren<CapsuleCollider2D>();
        //miCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //enemyTutorialController = FindAnyObjectByType<EnemyTutorialController>();
        //enemyTutorialController = GetComponent<EnemyTutorialController>();
        if (enemyTutorialController.isGetting5Attacks == true && hitsTutorial<= 4)
        {
            textoTuto1.text = "Ataques hechos " + hitsTutorial + "/5";
        }
        else if(hitsTutorial >= 5 && corrutineRunning == false)
        {
            textoTuto1.text = "Ataques hechos " + hitsTutorial + "/5";
            enemyTutorialController = FindAnyObjectByType<EnemyTutorialController>();
            StartCoroutine(DeactivateEvent1());
        }
    }
    public IEnumerator DeactivateEvent1()
    {
        corrutineRunning = true;
        enemyTutorialController.isGetting5Attacks = false;

        Destroy(miCollider);
        //miCollider.enabled = false;
        //enemyTutorialController.Die();

        yield return new WaitForSeconds(1);
        enemyTutorialController.GetComponent<Animator>().Play("Enemy_Disappear");
        yield return new WaitForSeconds(1);
        EnemyTutorial1.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        izel3Shadow.SetActive(true);
    }
}
