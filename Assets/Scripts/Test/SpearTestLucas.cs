using UnityEngine;

public class SpearTestLucas : MonoBehaviour
{
    public EnemyController controller;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            controller.Damage(5f);
        }
    }
}
