using UnityEngine;

public class SpearTestLucas : MonoBehaviour
{
    public EnemigoController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            controller.Daño(1f);
        }
    }
}
