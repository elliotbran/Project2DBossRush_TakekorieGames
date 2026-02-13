using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    public Image enemylife;
    private EnemyController enemycontroller;

    void Start()
    {
        enemycontroller = GameObject.Find("Boss#1").GetComponent<EnemyController>();
    }

    void Update()
    {
        enemylife.fillAmount = enemycontroller.currentHealth / enemycontroller.maxHealth;
    }
}
