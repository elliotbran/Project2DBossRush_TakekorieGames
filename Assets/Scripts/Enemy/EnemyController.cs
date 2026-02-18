using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class EnemyController : MonoBehaviour
{
    [Header("Health")]
    public float damage = 25f;
    public float currentHealth;
    public float maxHealth = 100f;
    public enum BossState
    {
        Idle,
        Chase,
        Attack,
    }

    [SerializeField] Transform player;

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

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentState = BossState.Idle;
        _animator = GetComponent<Animator>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    private void Update()
    {
        UpdateStates();
        playerInAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, whatIsPlayer);
        playerInSightRange = Physics2D.OverlapCircle(transform.position, sightRange, whatIsPlayer);

        
    }

    void UpdateStates()
    {
        /*switch (currentState)
        {
            case BossState.Idle:
                currentState = BossState.Idle;
                break;
            case BossState.Chase:
                currentState = BossState.Chase;
                break;
            case BossState.Attack:
                currentState = BossState.Attack;
                break;
        }*/

        if (!playerInSightRange)
        {
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
    void UpdateIdle()
    {
        _agent.SetDestination(transform.position);
        _animator.SetFloat("Speed", 0);              
    }

    void UpdateChase()
    {
        _agent.SetDestination(player.position);
        _animator.SetFloat("Speed", Mathf.Abs(_agent.speed));
    }

    void UpdateAttack()
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

    private void ResetAttack()
    {
        _alreadyAttacked = false;
    }
        

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        _animator.SetTrigger("Hurt");

        Debug.Log("Vida restante" + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
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
    }*/

    void Die()
    {
        Debug.Log("El enemigo ha muerto");

        _animator.SetBool("IsDead", true);

        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}