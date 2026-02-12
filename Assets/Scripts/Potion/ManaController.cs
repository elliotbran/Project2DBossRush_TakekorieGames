using UnityEngine;
using UnityEngine.UI;

public class ManaController : MonoBehaviour
{
    public Image mana;
    private float _currentMana = 0;
    private float _maximumMana = 5;


    void Start()
    {
        UpdateStatus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RefillMana(1f);
           
        }
    }
    void RefillMana(float amount)
    {
        _currentMana = Mathf.Clamp(_currentMana + amount, 0f, _maximumMana);
        UpdateStatus();
    }
    public bool ConsumeMana(float amount)
    {
        if (_currentMana >= amount)
        {
            _currentMana -= amount;
            UpdateStatus();
            return true; 
        }
        return false; 
    }
    void UpdateStatus()
    {
        mana.fillAmount = _currentMana / _maximumMana;
    }
}
