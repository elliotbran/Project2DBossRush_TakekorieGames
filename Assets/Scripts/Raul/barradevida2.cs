using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class barradevida2 : MonoBehaviour
{
    public Image rellenarbarradevida;
    private PlayerControllerElliot player_controller;

    void Start()
    {
        player_controller = GameObject.Find("Player").GetComponent<PlayerControllerElliot>();
    }

    void Update()
    {
        rellenarbarradevida.fillAmount = player_controller.Life / player_controller.MaximumLife;
    }
}
