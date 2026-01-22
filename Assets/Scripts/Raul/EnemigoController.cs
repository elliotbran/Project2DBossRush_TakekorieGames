using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class EnemigoController : MonoBehaviour
{
    public float daño = 1f;
    public float vida = 3f;
    public float MaxVida = 3f;

    private void OnMouseDown()
    {
        Daño(1f);
    }
    public void Daño(float cantidad)
    {
        vida -= cantidad;
        Debug.Log("Vida restante" + vida);
        if (vida <= 0)
        {
            vida = 0;
            Debug.Log("El enemigo ha muerto");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_ControllerRaul player = collision.GetComponent<Player_ControllerRaul>();

            if (player != null)
            {
                player.TomarDaño(daño);
                Debug.Log("Daño realizado. Vida restante: " + player.Vida);
            }
        }
    }
}