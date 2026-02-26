using System.Collections;
using UnityEngine;

public class HistoriaTutorialStart : MonoBehaviour
{

    public PlayerController _playerController;
    private Animator _playerAnimator;

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

        yield return new WaitForSeconds(2f);
        _playerController.canMove = true;
    }
}
