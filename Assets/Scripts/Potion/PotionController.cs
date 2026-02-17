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
        UpdateStatus(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _isFull)
        {
            UsePotion();
        }
    }

    public void UpdateStatus(bool full)
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

    void UsePotion()
    {
        playerController.Cure(25f);
        UpdateStatus(false);
        Debug.Log("Pocima usada, salud restaurada");
    }
}
