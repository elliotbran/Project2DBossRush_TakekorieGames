using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemylifeBar : MonoBehaviour
{
    public Image enemylife;
    private EnemyController enemycontroller;

    void Start()
    {
        enemycontroller = GameObject.Find("Enemigo prueba").GetComponent<EnemyController>();
    }

    void Update()
    {
        enemylife.fillAmount = enemycontroller.life / enemycontroller.Maximumlife;
    }
}
