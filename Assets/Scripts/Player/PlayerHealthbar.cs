using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    public Image PlayerlifeBar;
    private PlayerController player_controller;

    void Start()
    {
        player_controller = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        PlayerlifeBar.fillAmount = player_controller.life / player_controller.maximumLife;
    }
}
