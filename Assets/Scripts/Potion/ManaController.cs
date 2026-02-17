using UnityEngine;
using UnityEngine.UI;

public class ManaController : MonoBehaviour
{
    public Image mana;
    public PotionController potioncontroller;
    private float _currentMana = 0;
    private float _maxMana = 5;


    void Start()
    {
        UpdateStatus();
    }

    void Update()
    {

    }
    public void RefillMana(float amount)
    {
        _currentMana += amount;
        if (_currentMana >= _maxMana)
        {
            _currentMana = 0; 
            if (potioncontroller != null)
            {
                potioncontroller.UpdateStatus(true);
                Debug.Log("Pocion Recargada");
            }
        }
        _currentMana = Mathf.Clamp(_currentMana, 0f, _maxMana);
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
        mana.fillAmount = _currentMana / _maxMana;
    }
}
