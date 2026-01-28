using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;



public class EnemigoController : MonoBehaviour
{
    public float daño = 1f;
    public float vida = 3f;
    public float MaxVida = 3f;
    
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
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spear"))
        { 
            Daño(1f);
        }
    }*/
}