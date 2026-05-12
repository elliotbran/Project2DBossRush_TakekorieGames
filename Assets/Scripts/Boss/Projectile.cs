using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    [SerializeField]float _speed;
    [SerializeField] private AudioClip _destroySound;
    Transform _player; // Assign the player in the Inspector
    
    BossController _bossController; // Reference to the boss controller to manage projectile behavior

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.Find("Player").transform; // Get the player's position to chase the player
        _bossController = GameObject.Find("Boss").GetComponent<BossController>(); // Get reference to the boss controller
    }   

    void Update()
    {
        _spriteRenderer.flipX = _player.transform.position.x < _spriteRenderer.transform.position.x;

        if (_bossController.currentHealth == _bossController.maxHealth / 2) // If the boss is at full health, destroy the projectile immediately
        {
            _speed = 20f;
        }
        if (_player != null)
        {
            Vector2 direction = (Vector2)_player.position - (Vector2)transform.position;
            float distance = direction.magnitude;
            StartCoroutine(Destroy());

            if (distance > 0.1f) // Stop if close enough
            {
                direction.Normalize();
                transform.position = Vector2.MoveTowards(transform.position, _player.position, _speed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player == null) return;
            if (player.currentState == PlayerController.PlayerState.Dashing)
            {
                return;
            }
            if (player.currentState == PlayerController.PlayerState.Parrying)
            {                
                if (gameObject.CompareTag("AtaqueAmarillo") && player.manaHandler != null)
                {
                    var mc = player.manaHandler.manaController;
                    if (mc != null && !mc.IsManaFull)
                    {
                        player.manaHandler.SpawnMana(5);
                    }                    
                }                       
            }
            if (_bossController != null)
                _bossController.StartCoroutine(_bossController.AttackHitStop()); // Trigger hit stop effect in the boss controller         
            PlayDestroySound();
            player.TakeDamage(15f);
            Destroy(gameObject);

        }
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2.5f); // Destroy the projectile after 5 seconds if it doesn't hit the player
        Destroy(gameObject);
    }
    private void PlayDestroySound()
    {
        if (_destroySound != null)
        {
            AudioSource.PlayClipAtPoint(_destroySound, transform.position);
        }
    }
}
