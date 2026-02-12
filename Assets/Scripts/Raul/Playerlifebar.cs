using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playerlifebar : MonoBehaviour
{
    public Image PlayerlifeBar;
    private PlayerControllerElliot player_controller;

    void Start()
    {
        player_controller = GameObject.Find("Player").GetComponent<PlayerControllerElliot>();
    }

    void Update()
    {
<<<<<<<< HEAD:Assets/Scripts/Raul/Playerlifebar.cs
        PlayerlifeBar.fillAmount = player_controller.Life / player_controller.MaximumLife;
========
        rellenarbarradevida.fillAmount = player_controller.Life / player_controller.MaximumLife;
>>>>>>>> main:Assets/Scripts/Raul/barradevida2.cs
    }
}
