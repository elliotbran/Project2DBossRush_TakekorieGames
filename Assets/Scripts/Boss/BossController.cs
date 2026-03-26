using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
public class BossController : MonoBehaviour
{
    public BossState currentState;

    [Header("Health")] // Header for health related variables 
    public float damage = 25f;
    public float currentHealth;
    public float maxHealth = 100f;
    public bool isDead = false;
    public bool secondPhase = false;

    [Header("Combat")] // Header for combat related variables
    // Attacking
    [SerializeField] int _meleeAttackType = 0; // 1 for normal melee attack, 2 for golden melee attack
    [SerializeField] int _rangeAttackType = 0; // 1 for normal range attack, 2 for golden range attack

    [Range(0, 5f)]
    public float meleeAttackRange;
    [Header("Range distances")]
    [Range(0, 30f)]
    public float rangeAttackMinRange = 15f; // minimum distance to allow ranged attack
    [Range(0, 50f)]
    public float rangeAttackRange = 20f; // maximum distance to allow ranged attack
    [Range(0, 10f)]
    public float timeBetweenMeleeAttacks;
    [Range(0, 10f)]
    public float timeBetweenRangeAttacks;
    public GameObject normalProjectilePrefab;
    public GameObject goldenProjectilePrefab;
    public Transform projectileSpawnPoint;
    bool _alreadyMeleeAttacked;
    bool _alreadyRangeAttacked;

    [Range(0, 50f)]
    public float sightRange;
    public bool playerInMeleeAttackRange, playerInRangeAttackRange, playerInSightRange;

    [Header("Sounds")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _bloodSound;
    [SerializeField] private AudioClip _bossHurtSound;
    public AudioClip stepSound;       // Sonido de pasos
    public float stepInterval = 0.5f; // Intervalo entre cada paso
    private float nextStepTime = 0f;  // Control del tiempo entre pasos


    public enum BossState // Different states for the boss
    {
        Idle,
        Chase,
        MeleeAttack,
        RangeAttack,
    }

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
        _sprite_renderer_assign();
        _bloodParticles = GetComponentInChildren<ParticleSystem>(); // Get the ParticleSystem component attached to the boss for the blood effect when the boss takes damage
    }

    // helper to keep assignment consistent with original code style
    void _sprite_renderer_assign()
    {
        _spriteRenderer = _originalRenderer;
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
        // Ensure ranges are up-to-date before deciding states
        UpdateRanges();
        UpdateStates();
        SecondPhase();
    }

    void UpdateRanges()
    {
        playerInSightRange = Physics2D.OverlapCircle(transform.position, sightRange, whatIsPlayer);
        playerInMeleeAttackRange = Physics2D.OverlapCircle(transform.position, meleeAttackRange, whatIsPlayer);

        // Compute distance to player and use min/max window for ranged-attack eligibility
        if (_playerPosition != null)
        {
            float dist = Vector2.Distance(transform.position, _playerPosition.position);
            // true only when between min and max (inclusive)
            playerInRangeAttackRange = dist <= rangeAttackRange && dist >= rangeAttackMinRange;
        }
        else
        {
            playerInRangeAttackRange = false;
        }
    }

    void UpdateStates() // Update the boss's state based on the player's position and the boss's current state
    {
        // Flip the boss's sprite based on the player's position relative to the boss
        if (_playerPosition != null)
            _sprite_renderer_flip();

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
            _meleeAttackType = Random.Range(1, 5); // Randomly choose between the normal melee attack and the golden melee attack
            currentState = BossState.MeleeAttack;
            UpdateMeleeAttack();
        }

        if (playerInRangeAttackRange && !playerInMeleeAttackRange && playerInSightRange)
        {
            _rangeAttackType = Random.Range(1, 3); // Randomly choose between the normal range attack and the golden range attack
            currentState = BossState.RangeAttack;
            UpdateRangeAttack();
        }

        if (_playerController != null && _playerController.health <= 0) // If the player is dead, go Idle
        {
            sightRange = 0;
            meleeAttackRange = 0;
            rangeAttackRange = 0;
            currentState = BossState.Idle;            
            UpdateIdle();
        }
    }

    // helper to keep assignment consistent with original code style
    void _sprite_renderer_flip()
    {
        _spriteRenderer.flipX = _playerPosition.transform.position.x < _spriteRenderer.transform.position.x;
    }

    void SecondPhase()
    {
        if (!secondPhase && currentHealth <= maxHealth / 2)
        {
            secondPhase = true; // enter second phase
            _agent.speed = 8;
            _agent.acceleration = 14;
            damage = 35;
            timeBetweenMeleeAttacks = 1.25f;
            timeBetweenRangeAttacks = 3;
            _spriteRenderer.color = Color.green;
        }

        if (secondPhase)
        {
            return;
        }
    }

    void UpdateIdle()
    {
        _agent.SetDestination(transform.position);
        _animator.SetFloat("Speed", 0);              
    }

    void UpdateChase()
    {
        _agent.SetDestination(_playerPosition.position);
        _animator.SetFloat("Speed", Mathf.Abs(_agent.speed));
    }

    void UpdateMeleeAttack()
    {
        _agent.SetDestination(transform.position);
        _animator.SetFloat("Speed", 0);

        if (!_alreadyMeleeAttacked && _meleeAttackType == 1)
        {
            this.gameObject.tag = "AtaqueNormal";
            _animator.SetTrigger("NormalMeleeAttack");

            _alreadyMeleeAttacked = true;
            Debug.Log(_meleeAttackType);
            Invoke(nameof(ResetMeleeAttack), timeBetweenMeleeAttacks);
        }

        if (!_alreadyMeleeAttacked && _meleeAttackType == 2)
        {
            _animator.SetTrigger("GoldenMeleeAttack");

            _alreadyMeleeAttacked = true;
            Debug.Log(_meleeAttackType);
            Invoke(nameof(ResetMeleeAttack), timeBetweenMeleeAttacks);
        }

        if (!_alreadyMeleeAttacked && _meleeAttackType == 3)
        {
            _animator.SetTrigger("NormalSplashAttack");

            _alreadyMeleeAttacked = true;
            Debug.Log(_meleeAttackType);
            Invoke(nameof(ResetMeleeAttack), timeBetweenMeleeAttacks);
        }

        if (!_alreadyMeleeAttacked && _meleeAttackType == 4)
        {
            _animator.SetTrigger("GoldenSplashAttack");

            _alreadyMeleeAttacked = true;
            Debug.Log(_meleeAttackType);
            Invoke(nameof(ResetMeleeAttack), timeBetweenMeleeAttacks);
        }        
    }

    void UpdateRangeAttack()
    {
        _agent.SetDestination(transform.position);
        _animator.SetFloat("Speed", 0);

        if (!_alreadyRangeAttacked && _rangeAttackType == 1)
        {
            rangeAttackRange = 0f;
            _animator.SetTrigger("RangeAttack");
            Instantiate(normalProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            _alreadyRangeAttacked = true;
            Invoke(nameof(ResetRangeAttack), timeBetweenRangeAttacks);
        }

        if (!_alreadyRangeAttacked && _rangeAttackType == 2)
        {
            rangeAttackRange = 0f;
            this.gameObject.tag = "AtaqueMelee";
            _animator.SetTrigger("RangeAttack");
            Instantiate(goldenProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            _alreadyRangeAttacked = true;
            Invoke(nameof(ResetRangeAttack), timeBetweenRangeAttacks);
        }
    }

    private void ResetMeleeAttack()
    {
        _alreadyMeleeAttacked = false;
        this.gameObject.tag = "Untagged";
    }

    private void ResetRangeAttack()
    {
        _alreadyRangeAttacked = false;
        rangeAttackRange = 20f;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        StartCoroutine(HurtAnimation());

        if (_bloodParticles != null) _bloodParticles.Play();

        if (_audioSource != null)
        {

            if (_bloodSound != null)
            {
                _audioSource.PlayOneShot(_bloodSound, 1.0f);
            }


            if (_bossHurtSound != null)
            {
                _audioSource.pitch = Random.Range(0.8f, 1.0f); 
                _audioSource.PlayOneShot(_bossHurtSound, 0.9f);
            }
        }

        Debug.Log("Vida restante" + currentHealth);

        if (currentHealth <= 0)
        {
            _spriteRenderer.color = Color.white;
            isDead = true;
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if (player.currentState == PlayerController.PlayerState.Dashing)
            {
                return; // Skip damage and knockback if the player is dashing
            }
            if (player != null)
            {
                Vector2 knockDir = (collision.transform.position - transform.position).normalized;
                player.moveDir = new Vector3(knockDir.x, knockDir.y, 0f);
                player.knockbackCounter = player.knockbackTotalTime;

                StartCoroutine(AttackHitStop());
                player.TakeDamage(damage);
                Debug.Log("Da�o realizado. Vida restante: " + player.health);
            }
        }
    }    

    void Die()
    {
        Debug.Log("El boss ha muerto");
        Time.timeScale = 1f;

        _animator.SetBool("IsDead", true);

        bossHealthbar.SetActive(false);
        GetComponent<CapsuleCollider2D>().enabled = false;

        CameraGroup.SetActive(false);
        CameraPlayer.SetActive(true);

        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange); 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeAttackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, rangeAttackMinRange);
    }
    IEnumerator HurtAnimation()
    {
        _spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = Color.white;
    }
    #region HitStop
    public IEnumerator AttackHitStop()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
    }
    #endregion
}