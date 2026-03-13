using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITutorialControl : MonoBehaviour
{
    [Header("Ints to count")]
    public int hitsTutorial;

    [Header("Hud")]
    public GameObject tutorialHud1;
    public TMP_Text textoTuto1;

    [SerializeField] EnemyTutorialController enemyTutorialController;
    [SerializeField] GameObject EnemyTutorial1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //enemyTutorialController = FindAnyObjectByType<EnemyTutorialController>();
    }

    // Update is called once per frame
    void Update()
    {
        //enemyTutorialController = FindAnyObjectByType<EnemyTutorialController>();
        //enemyTutorialController = GetComponent<EnemyTutorialController>();
        if (enemyTutorialController.isGetting5Attacks == true && hitsTutorial<= 5)
        {
            textoTuto1.text = "Ataques hechos " + hitsTutorial + "/5";
        }
        else if(enemyTutorialController.isGetting5Attacks == true && hitsTutorial == 5)
        {
            StartCoroutine(DeactivateEvent1());
            enemyTutorialController = FindAnyObjectByType<EnemyTutorialController>();
        }
    }
    public IEnumerator DeactivateEvent1()
    {
        enemyTutorialController.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(1);
        enemyTutorialController.GetComponent<Animator>().Play("Enemy_Death");
        yield return new WaitForSeconds(1);
        EnemyTutorial1.SetActive(false);
    }
}
