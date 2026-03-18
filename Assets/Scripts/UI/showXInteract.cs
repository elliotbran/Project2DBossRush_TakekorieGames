using UnityEngine;

public class showXInteract : MonoBehaviour
{
    public GameObject buttonInteract;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonInteract = transform.Find("Interact")?.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        buttonInteract.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        buttonInteract.SetActive(false);
    }
}
