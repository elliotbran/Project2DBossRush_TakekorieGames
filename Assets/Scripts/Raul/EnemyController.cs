using UnityEngine;

public class EnemigoController : MonoBehaviour
{
    public float daño = 1f;

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