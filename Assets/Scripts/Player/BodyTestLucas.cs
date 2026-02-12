using UnityEngine;

public class BodyTestLucas : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            playerController.ReceiveDamage(1f);
        }
    }
}
