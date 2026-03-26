using UnityEngine;

public class TriggerCameraBossPlayer : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject bossCamera;
    public GameObject colliderStairs;

    private void Start()
    {
        playerCamera.SetActive(true);
        bossCamera.SetActive(false);
        colliderStairs.SetActive(false);    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            colliderStairs.SetActive(true);
            playerCamera.SetActive(false);
            bossCamera.SetActive(true);
            Destroy(gameObject); // Optional: destroy trigger after use
        }
    }

}
