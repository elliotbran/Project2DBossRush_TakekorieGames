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
    public int parrysTutorial;
    public int dashesTutorial;
    public int curacionesTutorial;

    [Header("Hud")]
    public GameObject tutorialHud1;
    public TMP_Text textoTuto1;


    [Header ("Chapuza")]
    [SerializeField] EnemyTutorialController enemyTutorialController;
    [SerializeField] GameObject EnemyTutorial1;
    [SerializeField] GameObject izel3Shadow;
    [SerializeField] GameObject izelParry;

    public bool isTutorial2Active;

    bool corrutineRunning;
    bool corrutineRunning2;
    bool corrutineRunning3;
    public CapsuleCollider2D miCollider;
    public GameObject cuadradoHUD;
    public GameObject rbHud;
    public GameObject lbhud;
    public GameObject triangulohud;
    public GameObject izelTutorialFinal;
    public GameObject izelHABLARFINAL;
    public EnemyTutorialController enemyTutorialController2;

    [Header("Camaras")]
    public GameObject camaraTutorialPlayer;
    public GameObject camaraTutorialGroup;
    public GameObject triggerFinal;
    bool finDelTutorial1;
    public GameObject barraVidaBoss;

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
            textoTuto1.text = "Ataques realizados: " + hitsTutorial + "/5";
        }
        else if(hitsTutorial >= 5 && corrutineRunning == false)
        {
            textoTuto1.text = "Ataques realizados: " + hitsTutorial + "/5";
            enemyTutorialController = FindAnyObjectByType<EnemyTutorialController>();
            StartCoroutine(DeactivateEvent1());
        }

        if (isTutorial2Active)
        {
            textoTuto1.text = "Parrys realizados: " + parrysTutorial + "/3 \nDashes realizados: " + dashesTutorial + "/3 \nCuraciones realizadas: " + curacionesTutorial + "/1";
            rbHud.SetActive(true);
            lbhud.SetActive(true);
            triangulohud.SetActive(true);
            if (parrysTutorial >= 3 && dashesTutorial >= 3 && curacionesTutorial >= 1 && corrutineRunning2 == false)
            {
                enemyTutorialController = FindAnyObjectByType<EnemyTutorialController>();
                UnityEngine.Debug.Log("Tutorial 2 completado");
                rbHud.SetActive(false);
                lbhud.SetActive(false);
                triangulohud.SetActive(false);
                StartCoroutine(tutorialParryCoroutine2());
            }
        }
        if(finDelTutorial1 == true)
        {
            textoTuto1.text = "Busca una salida";
            rbHud.SetActive(false);
            lbhud.SetActive(false);
            triangulohud.SetActive(false);
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
        cuadradoHUD.GetComponent<Animator>().Play("cuadradoDespawn");
        tutorialHud1.GetComponent<Animator>().Play("hudFadeOut");
        yield return new WaitForSeconds(0.5f);
        izel3Shadow.SetActive(true);
        yield return new WaitForSeconds(1);
        cuadradoHUD.SetActive(false);
        tutorialHud1.SetActive(false);
    }

    public IEnumerator tutorialParryCoroutine()
    {

        isTutorial2Active= true;
        yield return new WaitForSeconds(1);
        tutorialHud1.SetActive(true);
    }

    public IEnumerator tutorialParryCoroutine2()
    {
        UnityEngine.Debug.Log("Corrutina tutorial 2 comenzada");

        corrutineRunning2 = true;

        UnityEngine.Debug.Log("Paso 1");
        enemyTutorialController.tutorial2 = false;
        enemyTutorialController.rangeAttackRange = 0;

        UnityEngine.Debug.Log("Paso 2");
        yield return new WaitForSeconds(.5f);

        UnityEngine.Debug.Log("Paso 3");
        izelParry.GetComponent<Animator>().Play("Enemy_Disappear");

        UnityEngine.Debug.Log("Paso 4");
        yield return new WaitForSeconds(1);
        izelParry.SetActive(false);

        UnityEngine.Debug.Log("Paso 5");
        tutorialHud1.GetComponent<Animator>().Play("hudFadeOut");
        rbHud.SetActive(false);
        lbhud.SetActive(false);
        triangulohud.SetActive(false);  

        UnityEngine.Debug.Log("Paso 6");
        yield return new WaitForSeconds(0.5f);

        UnityEngine.Debug.Log("Paso 7");
        tutorialHud1.SetActive(false);

        izelHABLARFINAL.SetActive(true);
    }

    public void bossFightFinalTuto()
    {
        if (corrutineRunning3 == false)
        {
            enemyTutorialController2.sightRange = 0;
            StartCoroutine(bossfightFinalTUtocorr());
        }
    }
    public IEnumerator bossfightFinalTUtocorr()
    {
        corrutineRunning3 = true;
        izelHABLARFINAL.GetComponent<CircleCollider2D>().enabled = false;
        izelHABLARFINAL.GetComponent<Animator>().Play("Disappear");
        yield return new WaitForSeconds(1);
        izelHABLARFINAL.SetActive(false);
        izelTutorialFinal.SetActive(true);
        yield return new WaitForSeconds(1);
        camaraTutorialGroup.SetActive(true);
        camaraTutorialPlayer.SetActive(false);
        yield return new WaitForSeconds(2f);
        enemyTutorialController2.sightRange = 32;
        barraVidaBoss.SetActive(true);
        yield return null;
    }

    public void finDelTutorial()
    {
        finDelTutorial1 = true;
        UnityEngine.Debug.Log("Fin del tutorial");
        tutorialHud1.SetActive(true);
        textoTuto1.text = "Busca una salida";
        triggerFinal.SetActive(true);
    }
}
