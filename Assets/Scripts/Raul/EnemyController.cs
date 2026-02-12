using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class EnemyController : MonoBehaviour
{
    public float damage = 1f;
    public float life = 3f;
    public float Maximumlife = 3f;

    /*private void OnMouseDown()
    {
        Damage(1f);
    }*/
    public void Damage(float amount)
    {
        life -= amount;
        Debug.Log("Vida restante" + life);
        if (life <= 0)
        {
            life = 0;
            Debug.Log("El enemigo ha muerto");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerControllerElliot player = collision.GetComponent<PlayerControllerElliot>();

            if (player != null)
            {
                player.TomarDaño(damage);
                Debug.Log("Daño realizado. Vida restante: " + player.Life);
            }
        }
    }
}