using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBossHealthBar : MonoBehaviour
{
    public Image bossHealth;
    public EnemyTutorialController _enemyController;

    void Start()
    {
        //_enemyController = GameObject.Find("Boss").GetComponent<BossController>();
    }

    void Update()
    {
        bossHealth.fillAmount = _enemyController.currentHealth / _enemyController.maxHealth;
    }
}
