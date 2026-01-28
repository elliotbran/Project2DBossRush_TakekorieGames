using UnityEngine;
using UnityEngine.UI;

public class Manna_Controller : MonoBehaviour
{
    public Image Manna;
    private float CurrentMana = 0;
    private float MaximalMana = 5;


    void Start()
    {
        UpdateStatus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReffilMana(1f);
           
        }
    }
    void ReffilMana(float amount)
    {
        CurrentMana = Mathf.Clamp(CurrentMana + amount, 0f, MaximalMana);
        UpdateStatus();
    }
    public bool ConsumeMana(float amount)
    {
        if (CurrentMana >= amount)
        {
            CurrentMana -= amount;
            UpdateStatus();
            return true; 
        }
        return false; 
    }
    void UpdateStatus()
    {
        Manna.fillAmount = CurrentMana / MaximalMana;
    }
}
