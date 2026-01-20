using UnityEngine;
using UnityEngine.UI;

public class Pocima_Controller : MonoBehaviour
{
    public Image Pocima; 
    public Player_ControllerRaul player;
    public Mana_Controller manaController;
    private bool estaLlena = false;

    void Start()
    {
        ActualizarEstado(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !estaLlena)
        {
            if (manaController.ConsumirMana(1f))
            {
                ActualizarEstado(true);
                Debug.Log("Pócima recargada con maná");
            }
            else
            {
                Debug.Log("No tienes suficiente maná");
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && estaLlena)
        {
            UsarPocima();
        }
    }

    void ActualizarEstado(bool llena)
    {
        estaLlena = llena;
        if (estaLlena)
        {
            Pocima.color = Color.green;
            Pocima.fillAmount = 1f;
        }
        else
        {
            Pocima.color = Color.red;
            Pocima.fillAmount = 0.2f;
        }
    }

    void UsarPocima()
    {
        player.Curar(1f);
        ActualizarEstado(false);
        Debug.Log("Pocima usada, salud restaurada");
    }
}
