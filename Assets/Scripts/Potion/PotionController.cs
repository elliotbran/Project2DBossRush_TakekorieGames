using UnityEngine;
using UnityEngine.UI;

public class PotionController : MonoBehaviour
{
    public Image potion; 
    public PlayerController playerController;
    public ManaController manaController;

    private bool _isFull = false;

    void Start()
    {
        UpdateStatus(false); //empieza la pocion vacia
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _isFull) //Al darle a la tecla F llama a la funcion UsePotion
        {
            UsePotion();
        }
    }

    public void UpdateStatus(bool full) //mira si la potion si se llena se pone de color verde y si no esta llena se mantiene en color rojo
    {
        _isFull = full;
        if (_isFull)
        {
            potion.color = Color.green;
            potion.fillAmount = 1f;
        }
        else
        {
            potion.color = Color.red;
            potion.fillAmount = 0.2f;
        }
    }

    void UsePotion() //la funcion llama al playercontroller y cura al player con 25 de vida y la potion se queda vacia
    {
        playerController.Cure(25f);
        UpdateStatus(false);
        Debug.Log("Pocima usada, salud restaurada");
    }
}
