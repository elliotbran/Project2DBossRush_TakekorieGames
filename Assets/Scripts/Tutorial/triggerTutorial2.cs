using UnityEngine;

public class triggerTutorial2 : MonoBehaviour
{
    public GameObject shadow2;
    PlayerController playerController;
    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("TriggerTutorial2 Iniciado");
            shadow2.SetActive(true);
            playerController.canAttack = true;
            gameObject.SetActive(false);
        }
    }
}
