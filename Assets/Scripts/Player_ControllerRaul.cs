using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ControllerRaul : MonoBehaviour
{
    public float Life = 3f;
    public float MaximumLife = 3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TomarDaño(float cantidad)
    {
        Life -= cantidad;
        if (Life <= 0)
        {
            Life = 0;
            Debug.Log("El jugador ha muerto");
            Destroy(gameObject);
        }
    }
    public void Curar(float cantidad)
    {
        Life += cantidad;
        if (Life > MaximumLife) 
        {
            Life = MaximumLife;
        }
    }
}
