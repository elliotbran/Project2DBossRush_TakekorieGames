using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class barraenemigovida : MonoBehaviour
{
    public Image vidaenemigo;
    private EnemigoController enemigocontroller;

    void Start()
    {
        enemigocontroller = GameObject.Find("Enemigo").GetComponent<EnemigoController>();
    }

    void Update()
    {
        vidaenemigo.fillAmount = enemigocontroller.vida / enemigocontroller.MaxVida;
    }
}
