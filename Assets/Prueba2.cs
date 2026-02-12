using UnityEngine;

public class Prueba2 : MonoBehaviour
{
    ScriptAtaquePrueba ataquePrueba;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ataquePrueba = FindAnyObjectByType<ScriptAtaquePrueba>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ataquePrueba.atacando)
        {
            ataquePrueba.MiBomboBombez();
        }
    }
}
