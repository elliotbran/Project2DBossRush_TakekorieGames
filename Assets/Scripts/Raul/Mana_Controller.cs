using UnityEngine;
using UnityEngine.UI;

public class Mana_Controller : MonoBehaviour
{
    public Image Mana;
    private float ManaActual = 0;
    private float ManaMaxima = 5;


    void Start()
    {
        ActualizarEstado();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RellenarMana(1f);
           
        }
    }
    void RellenarMana(float cantidad)
    {
        ManaActual = Mathf.Clamp(ManaActual + cantidad, 0f, ManaMaxima);
        ActualizarEstado();
    }

    void ActualizarEstado()
    {
        Mana.fillAmount = ManaActual / ManaMaxima;
    }
}
