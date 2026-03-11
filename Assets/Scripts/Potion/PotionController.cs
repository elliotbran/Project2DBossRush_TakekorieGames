using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PotionController : MonoBehaviour
{
    public Image potion; 
    public bool IsFull => _isFull;
    public PlayerController playerController;
    public ManaController manaController;

    private bool _isFull = false;

    private Animator _playerAnimator;

    void Start()
    {
        UpdateStatus(false); //empieza la pocion vacia
        _playerAnimator = playerController.GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isFull || Input.GetButtonDown("Heal") && _isFull) //Al darle a la tecla F llama a la funcion UsePotion
        {
            UsePotion();
            _playerAnimator.SetTrigger("Heal");
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
        playerController.Heal(25f);
        playerController.currentState = PlayerController.PlayerState.Healing;
        UpdateStatus(false);
        Debug.Log("Pocima usada, salud restaurada");
        StartCoroutine(HealAnimation());
    }

    IEnumerator HealAnimation () //la animacion de curacion dura 1 segundo y luego vuelve al estado normal del player
    {
        yield return new WaitForSeconds(1.25f);
        playerController.currentState = PlayerController.PlayerState.Normal;
    }
}
