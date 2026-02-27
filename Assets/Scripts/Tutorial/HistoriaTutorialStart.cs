using System.Collections;
using UnityEngine;

public class HistoriaTutorialStart : MonoBehaviour
{

    public PlayerController _playerController;
    private Animator _playerAnimator;

    [SerializeField] private GameObject dialogue1;
    [SerializeField] private GameObject dialogue2;

    private void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        _playerAnimator = _playerController.GetComponent<Animator>();

        StartCoroutine(StartTutorial());
    }

    public IEnumerator StartTutorial()
    {
        _playerController.canMove = false;

        if (_playerAnimator != null)
            _playerAnimator.Play("Player_WakeUp");


        yield return new WaitForSeconds(2.5f);

        dialogue1.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        _playerController.autoTrigger = true;
        //Debug.Log(_playerController.autoTrigger);
        yield return new WaitForSeconds(0.3f);
        _playerController.canMove = true;

        dialogue1.SetActive(false);

        //dialogue2.SetActive(true);
    }
}
