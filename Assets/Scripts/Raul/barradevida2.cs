using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class barradevida2 : MonoBehaviour
{
    public Image rellenarbarradevida;
    private Player_ControllerRaul player_controller;

    void Start()
    {
        player_controller = GameObject.Find("Player").GetComponent<Player_ControllerRaul>();
    }

    void Update()
    {
        rellenarbarradevida.fillAmount = player_controller.Vida / player_controller.vidaMaxima;
    }
}
