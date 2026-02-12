using UnityEngine;

public class BodyTestLucas : MonoBehaviour
{
    public PlayerControllerElliot controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            controller.TomarDaño(1f);
        }
    }
}
