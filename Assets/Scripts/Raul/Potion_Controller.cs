using UnityEngine;
using UnityEngine.UI;

public class Potion_Controller : MonoBehaviour
{
    public Image Potion; 
    public PlayerControllerElliot player;
    public Manna_Controller mannacontroller;
    private bool full = false;

    void Start()
    {
        UpdateStatus(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !full)
        {
            if (mannacontroller.ConsumeMana(1f))
            {
                UpdateStatus(true);
                Debug.Log("Pócima recargada con mana");
            }
            else
            {
                Debug.Log("No tienes suficiente mana");
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && full)
        {
            UsePocima();
        }
    }

    void UpdateStatus(bool llena)
    {
        full = llena;
        if (full)
        {
            Potion.color = Color.green;
            Potion.fillAmount = 1f;
        }
        else
        {
            Potion.color = Color.red;
            Potion.fillAmount = 0.2f;
        }
    }

    void UsePocima()
    {
        player.Cure(1f);
        UpdateStatus(false);
        Debug.Log("Pocima usada, salud restaurada");
    }
}
