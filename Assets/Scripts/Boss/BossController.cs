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
    public bool secondPhase = false;

    [Header("Combat")] // Header for combat related variables
    // Attacking
    [SerializeField] int _attackType = 0; // 1 for normal melee attack, 2 for golden melee attack

    [Range(0, 5f)]
    public float meleeAttackRange;
    [Range(0, 30f)]
    public float rangeAttackRange;
    [Range(0, 10f)]
    public float timeBetweenMeleeAttacks;
    [Range(0, 10f)]
    public float timeBetweenRangeAttacks;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    bool _alreadyMeleeAttacked;
    bool _alreadyRangeAttacked;

    [Range(0, 50f)]
    public float sightRange;
    public bool playerInMeleeAttackRange, playerInRangeAttackRange, playerInSightRange;
   
    public enum BossState // Different states for the boss
    {
        Idle,
        Chase,
        MeleeAttack,
        RangeAttack,
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
    private SpriteRenderer _originalRenderer;

    private PlayerController _playerController;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component attached to the boss
        _animator = GetComponent<Animator>(); // Get the Animator component attached to the boss
        _playerPosition = GameObject.Find("Player").transform; // Get the player's position to chase and attack the player
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>(); // Get the PlayerController component attached to the player
        _originalRenderer = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer component attached to the boss body
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer component attached to the boss body
        _spriteRenderer = _originalRenderer;
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
        UpdateRanges();      
        SecondPhase();
    }

    void UpdateRanges()
    {
        playerInSightRange = Physics2D.OverlapCircle(transform.position, sightRange, whatIsPlayer);
        playerInMeleeAttackRange = Physics2D.OverlapCircle(transform.position, meleeAttackRange, whatIsPlayer);
        playerInRangeAttackRange = Physics2D.OverlapCircle(transform.position, rangeAttackRange, whatIsPlayer);
    }

    void UpdateStates() // Update the boss's state based on the player's position and the boss's current state
    {
        // Flip the boss's sprite based on the player's position relative to the boss
        _spriteRenderer.flipX = _playerPosition.transform.position.x < _spriteRenderer.transform.position.x;

        if (!playerInSightRange)
        {
            currentState = BossState.Idle;
            UpdateIdle();
        }

        if (!playerInMeleeAttackRange && !playerInRangeAttackRange && playerInSightRange)
        {
            currentState = BossState.Chase;
            UpdateChase();
        }

        if (playerInMeleeAttackRange && playerInSightRange)
        {
            _attackType = Random.Range(1, 3); // Randomly choose between the normal melee attack and the golden melee attack
            currentState = BossState.MeleeAttack;
            UpdateMeleeAttack();
        }

        if (playerInRangeAttackRange && !playerInMeleeAttackRange && playerInSightRange)
        {
            currentState = BossState.RangeAttack;
            UpdateRangeAttack();
        }

        if (_playerController.health <= 0) // If the player is in attack range but the player's health is less than or equal to 0, the boss will go back to the Idle state
        {
            sightRange = 0;
            meleeAttackRange = 0;
            rangeAttackRange = 0;
            currentState = BossState.Idle;            
            UpdateIdle();
        }
    }
    void SecondPhase()
    {
        if (!secondPhase && currentHealth == maxHealth / 2)
        {
            secondPhase = true; // If the boss's health is less than or equal to half of its maximum health, it will enter the second phase of the fight where it will become more aggressive and use different attacks
        }

        if (secondPhase)
        {
            _agent.speed = _agent.speed * 2f; // Increase the boss's speed when its health is less than or equal to half of its maximum health to make the fight more challenging for the player
            damage = 35; // Increase the boss's damage when its health is less than or equal to half of its maximum health to make the fight more challenging for the player
            timeBetweenMeleeAttacks = 1.25f;
            timeBetweenRangeAttacks = 5;
            _spriteRenderer.color = Color.green; // Change the boss's sprite color to yellow to indicate that it is in the second phase of the fight when its health is less than or equal to half of its maximum health
            secondPhase = false;
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

    void UpdateMeleeAttack() // In the Attack state, the boss will stop moving and play the attack animation. If the boss is already attacking, it will wait for the time between attacks before it can attack again.
    {
        _agent.SetDestination(transform.position);
        _animator.SetFloat("Speed", 0);


        if (!_alreadyMeleeAttacked && _attackType == 1)
        {
            _animator.SetTrigger("NormalMeleeAttack");

            _alreadyMeleeAttacked = true;
            Debug.Log(_attackType
    );
            Invoke(nameof(ResetMeleeAttack), timeBetweenMeleeAttacks);
        }

        if (!_alreadyMeleeAttacked && _attackType == 2)
        {
            _animator.SetTrigger("GoldenMeleeAttack");

            _alreadyMeleeAttacked = true;
            Debug.Log(_attackType
    );
            Invoke(nameof(ResetMeleeAttack), timeBetweenMeleeAttacks);
        }
    }

    void UpdateRangeAttack() // In the Attack state, the boss will stop moving and play the attack animation. If the boss is already attacking, it will wait for the time between attacks before it can attack again.
    {
        _agent.SetDestination(transform.position);
        _animator.SetFloat("Speed", 0);

        if (!_alreadyRangeAttacked)
        {
            _animator.SetTrigger("RangeAttack");

            // Instantiate the projectile prefab
            Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity); // Instantiate the projectile prefab at the projectile spawn point position with no rotation
            _alreadyRangeAttacked = true;
            rangeAttackRange = 0; // Set range to 0 to prevent the boss from attacking again while the projectile is still in the air
            Invoke(nameof(ResetRangeAttack), timeBetweenRangeAttacks);
        }        
    }

    private void ResetMeleeAttack() // Reset the attack so the boss can attack again after the time between attacks has passed
    {
        _alreadyMeleeAttacked = false;
        _attackType = 0; // Randomly choose between the normal melee attack and the golden melee attack
    }

    private void ResetRangeAttack() // Reset the attack so the boss can attack again after the time between attacks has passed
    {
        _alreadyRangeAttacked = false;
        rangeAttackRange = 20f; // Reset sight range to its original value after the time between range attacks has passed
    }

    public void TakeDamage(int damage) // This function is called when the boss takes damage. It reduces the boss's health by the amount of damage taken and checks if the boss's health is less than or equal to 0. If it is, the boss dies.
    {
        currentHealth -= damage;
        
        //_animator.SetTrigger("Hurt");
        StartCoroutine(HurtAnimation());
        _bloodParticles.Play();
        
        Debug.Log("Vida restante" + currentHealth);

        if (currentHealth <= 0)
        {
            _spriteRenderer.color = Color.white; // Original sprite color
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
                player.TakeDamage(damage);
                Debug.Log("Da�o realizado. Vida restante: " + player.health);
            }
        }
    }    

    void Die() // This function is called when the boss's health is less than or equal to 0. It plays the death animation and disables the boss's colliders and this script to prevent the boss from moving or attacking after it has died.
    {
        Debug.Log("El boss ha muerto");
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
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange); 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeAttackRange);
    }

    IEnumerator HurtAnimation()
    {
        _spriteRenderer.color = Color.red; // Change the boss's sprite color to red to indicate that it has taken damage
        yield return new WaitForSeconds(0.1f); // Wait for the hurt animation to finish before changing the boss's sprite color back to normal
        _spriteRenderer.color = Color.white; // Change the boss's sprite color back to normal after the hurt animation has finished
    }
    #region HitStop
    public IEnumerator AttackHitStop()
    {
        Time.timeScale = 0.2f; // Slow down time to create hit stop effect
        yield return new WaitForSecondsRealtime(0.3f); // Wait for a short duration in real time
        Time.timeScale = 1; // Restore original time scale      
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