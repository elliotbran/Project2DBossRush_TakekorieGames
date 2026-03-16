using UnityEngine;

public class FlipSprites : MonoBehaviour
{
    SpriteRenderer _spriteRenderer;
    public Transform _playerPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Flip the boss's sprite based on the player's position relative to the boss
        _spriteRenderer.flipX = _playerPosition.transform.position.x > _spriteRenderer.transform.position.x;
    }
}
