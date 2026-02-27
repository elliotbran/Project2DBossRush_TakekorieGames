using System.Collections;
using UnityEngine;

public class AutoDestroyDialogue : MonoBehaviour
{

    PlayerController _playerController;

    private void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        _playerController.canMove = false;
        Debug.Log("TriggerEnter");
        StartCoroutine(DestroyAfterDelay());
    }
}
