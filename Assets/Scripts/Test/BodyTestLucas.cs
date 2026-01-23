using UnityEngine;

public class BodyTestLucas : MonoBehaviour
{
    public PlayerControllerElliot controller; // Arrastra aquí al padre

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // El cuerpo solo reacciona si lo toca un enemigo
        if (collision.CompareTag("Enemy"))
        {
            controller.TomarDaño(1f);
        }
    }
}
