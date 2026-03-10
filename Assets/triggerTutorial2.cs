using UnityEngine;

public class triggerTutorial2 : MonoBehaviour
{
    public GameObject shadow2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("TriggerTutorial2 Iniciado");
            shadow2.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
