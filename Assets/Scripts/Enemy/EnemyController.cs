using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;



public class EnemyController : MonoBehaviour
{
    [Header("Health")] // Header for health related variables 
    public float damage = 25f;
    public float currentHealth;
    public float maxHealth = 100f;
    public enum BossState // Different states for the boss
    {
        Idle,
        Chase,
        Attack,
    }

    [SerializeField] Transform player; // Get the player's position to chase and attack the player

    public BossState currentState;
    public LayerMask whatIsPlayer;

    // Attacking
    public float timeBetweenAttacks = 2f;
    bool _alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // Components
    NavMeshAgent _agent;
    Animator _animator;

    public GameObject CameraGroup;
    public GameObject CameraPlayer;
    public GameObject bossHealthbar;

    public Transform playerPosition;
    public SpriteRenderer spriteRenderer;
    public PlayerController playerController;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component attached to the boss
        _animator = GetComponent<Animator>(); // Get the Animator component attached to the boss
    }

    private void Start()
    {
        currentHealth = maxHealth; // Initialize the boss's health to the maximum health at the start of the game
        currentState = BossState.Idle; // Start the boss in the Idle state (doesn't matter right now because he detects the player right away and changes to Chase)
        _agent.updateRotation = false;  
        _agent.updateUpAxis = false; 
    }

    private void Update()
    {
        UpdateStates();
        playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, whatIsPlayer);
        playerInSightRange = Physics2D.OverlapCircle(transform.position, sightRange, whatIsPlayer);

        // Flip the boss's sprite based on the player's position relative to the boss
        spriteRenderer.flipX = playerPosition.transform.position.x > spriteRenderer.transform.position.x;
    }

    void UpdateStates() // Update the boss's state based on the player's position and the boss's current state
    {      
        if (!playerInSightRange)
        {
            currentState = BossState.Idle;
            UpdateIdle();
        }

        if (playerController.health <= 0) // If the player is in attack range but the player's health is less than or equal to 0, the boss will go back to the Idle state
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
        _agent.SetDestination(player.position);
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

        Debug.Log("Vida restante" + currentHealth);
        if (currentHealth <= 0)
        {
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
                player.ReceiveDamage(damage);
                Debug.Log("Daño realizado. Vida restante: " + player.health);
            }
        }
    }

    void Die() // This function is called when the boss's health is less than or equal to 0. It plays the death animation and disables the boss's colliders and this script to prevent the boss from moving or attacking after it has died.
    {
        Debug.Log("El enemigo ha muerto");

        _animator.SetBool("IsDead", true);

        bossHealthbar.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = false;
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
}