using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
public class BossController : MonoBehaviour
{
    [Header("Health")] // Header for health related variables 
    public float damage = 25f;
    public float currentHealth;
    public float maxHealth = 100f;
    public bool isDead = false;

    [Header("Combat")] // Header for combat related variables
    // Attacking
    public float attackRange;
    public float timeBetweenAttacks = 2f;
    bool _alreadyAttacked;

    public float sightRange;
    public bool playerInAttackRange, playerInSightRange;
   
    public enum BossState // Different states for the boss
    {
        Idle,
        Chase,
        Attack,
    }

    public BossState currentState;
    public LayerMask whatIsPlayer;     

    // Components
    NavMeshAgent _agent;
    Animator _animator;
    ParticleSystem _bloodParticles;
    Transform _playerPosition; // Get the player's position to chase and attack the player

    public GameObject CameraGroup;
    public GameObject CameraPlayer;
    public GameObject bossHealthbar;
    private SpriteRenderer _spriteRenderer;

    private PlayerController _playerController;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component attached to the boss
        _animator = GetComponent<Animator>(); // Get the Animator component attached to the boss
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer component attached to the boss body
        _playerPosition = GameObject.Find("Player").transform; // Get the player's position to chase and attack the player
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>(); // Get the PlayerController component attached to the player
        _bloodParticles = GetComponentInChildren<ParticleSystem>(); // Get the ParticleSystem component attached to the boss for the blood effect when the boss takes damage
    }

    private void Start()
    {
        currentHealth = maxHealth; // Initialize the boss's health to the maximum health at the start of the game
        currentState = BossState.Idle; // Start the boss in the Idle state (doesn't matter right now because he detects the player right away and changes to Chase)
        _agent.updateRotation = false;  
        _agent.updateUpAxis = false;
        _bloodParticles.Stop();
    }

    private void Update()
    {
        UpdateStates();
        playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, whatIsPlayer);
        playerInSightRange = Physics2D.OverlapCircle(transform.position, sightRange, whatIsPlayer);

        // Flip the boss's sprite based on the player's position relative to the boss
        _spriteRenderer.flipX = _playerPosition.transform.position.x < _spriteRenderer.transform.position.x;
    }

    void UpdateStates() // Update the boss's state based on the player's position and the boss's current state
    {      
        if (!playerInSightRange)
        {
            currentState = BossState.Idle;
            UpdateIdle();
        }

        if (_playerController.health <= 0) // If the player is in attack range but the player's health is less than or equal to 0, the boss will go back to the Idle state
        {
            sightRange = 0;
            attackRange = 0;
            currentState = BossState.Idle;
            UpdateIdle();
        }

        if (!playerInAttackRange && playerInSightRange)
        {
            currentState = BossState.Chase;
            UpdateChase();
        }

        if (playerInAttackRange && playerInSightRange)
        {
            currentState = BossState.Attack;
            UpdateAttack();
        }

    }
    void UpdateIdle() // In the Idle state, the boss will stop moving and play the idle animation
    {
        _agent.SetDestination(transform.position);
        _animator.SetFloat("Speed", 0);              
    }

    void UpdateChase() // In the Chase state, the boss will move towards the player and play the running animation
    {
        _agent.SetDestination(_playerPosition.position);
        _animator.SetFloat("Speed", Mathf.Abs(_agent.speed));
    }

    void UpdateAttack() // In the Attack state, the boss will stop moving and play the attack animation. If the boss is already attacking, it will wait for the time between attacks before it can attack again.
    {
        _agent.SetDestination(transform.position);
        _animator.SetFloat("Speed", 0);


        if (!_alreadyAttacked)
        {
            _animator.SetTrigger("Attack");

            _alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }        
    }

    private void ResetAttack() // Reset the attack so the boss can attack again after the time between attacks has passed
    {
        _alreadyAttacked = false;
    }
        

    public void TakeDamage(int damage) // This function is called when the boss takes damage. It reduces the boss's health by the amount of damage taken and checks if the boss's health is less than or equal to 0. If it is, the boss dies.
    {
        currentHealth -= damage;
        
        _animator.SetTrigger("Hurt");
        _bloodParticles.Play();

        Debug.Log("Vida restante" + currentHealth);
        if (currentHealth <= 0)
        {
            isDead = true;
            //StartCoroutine(DeathHitStop()); // Start the hit stop effect when the boss dies
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) // This function is called when the boss's attack hitbox collides with the player. It checks if the collided object is the player and if it is, it calls the player's ReceiveDamage function to deal damage to the player.
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player != null)
            {
                StartCoroutine(AttackHitStop()); // Start the hit stop effect when the boss attacks the player
                player.ReceiveDamage(damage);
                Debug.Log("Daño realizado. Vida restante: " + player.health);
            }
        }
    }

    void Die() // This function is called when the boss's health is less than or equal to 0. It plays the death animation and disables the boss's colliders and this script to prevent the boss from moving or attacking after it has died.
    {
        Debug.Log("El enemigo ha muerto");
        Time.timeScale = 1f; // Ensure that time scale is reset to normal after the hit stop effect

        _animator.SetBool("IsDead", true);

        bossHealthbar.SetActive(false);
        GetComponent<CapsuleCollider2D>().enabled = false;

        CameraGroup.SetActive(false);
        CameraPlayer.SetActive(true);

        this.enabled = false;
    }

    private void OnDrawGizmos() // Show the attack range and sight range of the boss in the editor for debugging.
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    #region HitStop
    IEnumerator AttackHitStop()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.2f; // Slow down time to create hit stop effect
        yield return new WaitForSecondsRealtime(0.3f); // Wait for a short duration in real time
        Time.timeScale = originalTimeScale; // Restore original time scale      
    }
    /*IEnumerator DeathHitStop()
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.2f; // Slow down time to create hit stop effect
        yield return new WaitForSeconds(0.3f); // Wait for a short duration in real time
        Time.timeScale = originalTimeScale; // Restore original time scale        
    }*/
    #endregion
}