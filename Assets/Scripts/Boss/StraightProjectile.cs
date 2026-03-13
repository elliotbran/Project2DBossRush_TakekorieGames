using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightProjectile : MonoBehaviour
{
    [SerializeField] float _speed;
    Transform _player; // Assign the player in the Inspector

    BossController _bossController; // Reference to the boss controller to manage projectile behavior

    Vector2 _direction; // Fixed direction computed at spawn
    Vector2 _targetPosition; // Player position sampled at spawn

    // Start is called before the first frame update
    void Start()
    {
        var playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _targetPosition = _player.position; // sample player's position once
            _direction = (_targetPosition - (Vector2)transform.position).normalized;

            // Optional: rotate projectile to face travel direction
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            _direction = Vector2.right; // fallback direction
        }

        var bossObj = GameObject.Find("Boss");
        if (bossObj != null)
            _bossController = bossObj.GetComponent<BossController>();

        // Start lifetime countdown once
        StartCoroutine(Destroy());
    }

    void Update()
    {
        if (_bossController != null && _bossController.currentHealth <= _bossController.maxHealth / 2)
        {
            _speed = 20f;
        }

        // Move in a straight line using the precomputed direction
        transform.position = (Vector2)transform.position + _direction * _speed * Time.deltaTime;

        // Destroy if reached close to the sampled target position
        if (Vector2.Distance(transform.position, _targetPosition) <= 0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.currentState == PlayerController.PlayerState.Parrying)
            {
                return;
            }
            Destroy(gameObject); // Destroy the projectile on impact
            player.TakeDamage(15f); // Apply damage to the player
            if (_bossController != null)
                _bossController.StartCoroutine(_bossController.AttackHitStop()); // Trigger hit stop effect in the boss controller
        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2.5f); // Destroy the projectile after 5 seconds if it doesn't hit the player
        Destroy(gameObject);
    }
}