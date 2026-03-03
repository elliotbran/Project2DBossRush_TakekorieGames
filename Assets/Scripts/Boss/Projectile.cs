using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 20f;
    float _speed = 10f;
    Transform _player; // Assign the player in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").transform; // Get the player's position to chase the player
    }   

    void Update()
    {
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
            Destroy(gameObject); // Destroy the projectile on impact
            player.TakeDamage(damage); // Apply damage to the player
        }        
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5f); // Destroy the projectile after 5 seconds if it doesn't hit the player
        Destroy(gameObject);
    }
}
