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

    [SerializeField] EnemyTutorialController enemyTutorialController;
    [SerializeField] GameObject EnemyTutorial1;

    bool corrutineRunning;
    public CapsuleCollider2D miCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Esto busca el collider en el objeto o en cualquiera de sus hijos
        miCollider = EnemyTutorial1.GetComponentInChildren<CapsuleCollider2D>();
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
            enemyTutorialController.isGetting5Attacks = false;
            UnityEngine.Debug.Log("hola 5 hits");
            enemyTutorialController = FindAnyObjectByType<EnemyTutorialController>();
            StartCoroutine(DeactivateEvent1());
        }
    }
    public IEnumerator DeactivateEvent1()
    {
        corrutineRunning = true;

        if (miCollider != null)
        {
            miCollider.enabled = false;
            UnityEngine.Debug.Log("Collider desactivado en: " + miCollider.gameObject.name);
        }
        else
        {
            UnityEngine.Debug.LogError("No se encontró miCollider para desactivar");
        }

        yield return new WaitForSeconds(1);
        //enemyTutorialController.GetComponent<Animator>().Play("Enemy_Death");
        yield return new WaitForSeconds(1);
        EnemyTutorial1.SetActive(false);
    }
}
