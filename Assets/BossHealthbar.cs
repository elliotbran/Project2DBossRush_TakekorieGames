using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthbar : MonoBehaviour
{
    public Image bossHealth;
    private BossController _enemyController;

    void Start()
    {
        _enemyController = GameObject.Find("Boss").GetComponent<BossController>();
    }

    void Update()
    {
        bossHealth.fillAmount = _enemyController.currentHealth / _enemyController.maxHealth;
    }
}
