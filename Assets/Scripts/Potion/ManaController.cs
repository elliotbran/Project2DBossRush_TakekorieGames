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
        UpdateStatus();//actualiza la barra del mana de la UI empezando en 0/vacia
    }

    void Update()
    {

    }
    public void RefillMana(float amount) //esta funcion rellena la barra de mana cuando las particulas tocan al player
    {
        _currentMana += amount; //se suma 1 de mana que nos da la primera particula

        if (_currentMana >= _maxMana) //Si el mana se llena se vacia la barra de mana y se rellena la pocima cambiandola de color a verde
        {
            _currentMana = 0; //se vacia la barra de mana y empieza de cero 

            if (potioncontroller != null)
            {
                potioncontroller.UpdateStatus(true); //que cambie la pocion de color cuando se llene 
                Debug.Log("Mana llena Poción lists");
            }
        }

        UpdateStatus(); //actualizamos la barra de mana
    }
    public bool ConsumeMana(float amount)// esta funcion intenta gastar mana si ha gastado mana devuelve el true y si no devuelve el false
    {
        if (_currentMana >= amount)
        {
            _currentMana -= amount;
            UpdateStatus();
            return true; 
        }
        return false; 
    }
    void UpdateStatus() //actualiza la barra del mana de la UI
    {
        mana.fillAmount = _currentMana / _maxMana;
    }
}
