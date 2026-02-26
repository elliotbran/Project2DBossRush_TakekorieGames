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
        _playerController.canMove = false;
        yield return new WaitForSeconds(1f);
        if (_playerAnimator != null)
            _playerAnimator.SetTrigger("StandUp");
        yield return new WaitForSeconds(1f);
        _playerController.canMove = true;
    }
}
