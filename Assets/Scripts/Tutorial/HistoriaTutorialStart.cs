using System.Collections;
using UnityEngine;

public class HistoriaTutorialStart : MonoBehaviour
{

    public PlayerController _playerController;
    private Animator _playerAnimator;

    [SerializeField] private GameObject dialogue1;

    private void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _playerAnimator = _playerController.GetComponent<Animator>();

        StartCoroutine(StartTutorial());
    }

    public IEnumerator StartTutorial()
    {

        if (_playerAnimator != null)
            _playerAnimator.Play("Player_WakeUp");

        _playerController.canMove = false;

        yield return new WaitForSeconds(2.5f);
        _playerController.canMove = true;

        dialogue1.SetActive(true);

        _playerController.autoTrigger = true;
        Debug.Log(_playerController.autoTrigger);
        yield return new WaitForSeconds(0.5f);
        _playerController.autoTrigger = false;

        dialogue1.SetActive(false);

        //dialogue2.SetActive(true);
    }
}
