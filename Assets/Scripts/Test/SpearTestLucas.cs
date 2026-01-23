using UnityEngine;

public class SpearTestLucas : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Aquí llamas a la función de daño del enemigo
            collision.GetComponent<EnemigoController>().Daño(5f);
        }
    }
}
