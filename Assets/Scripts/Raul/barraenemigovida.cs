using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class barraenemigovida : MonoBehaviour
{
    public Image vidaenemigo;
    private EnemyController enemigocontroller;

    void Start()
    {
        enemigocontroller = GameObject.Find("Enemigo prueba").GetComponent<EnemyController>();
    }

    void Update()
    {
        vidaenemigo.fillAmount = enemigocontroller.life / enemigocontroller.Maximumlife;
    }
}
