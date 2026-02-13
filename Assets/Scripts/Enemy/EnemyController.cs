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


    // Components
    NavMeshAgent _agent;
    Animator _animator;

    /*private void OnMouseDown()
    {
        Damage(1f);
    }*/

    private void Start()
    {
        currentHealth = maxHealth;
        currentState = BossState.Chase;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    private void Update()
    {
        UpdateStates();
    }

    void UpdateStates()
    {
        switch (currentState)
        {
            case BossState.Idle:
                UpdateIdle();
                break;
            case BossState.Chase:
                UpdateChase();
                break;
            case BossState.Attack:
                UpdateAttack();
                break;
        }
    }
    void UpdateIdle()
    {

    }

    void UpdateChase()
    {
        if (_agent.speed > 0)
        {
            _animator.SetFloat("Speed", Mathf.Abs(_agent.speed));
        }

        _agent.SetDestination(player.position);
    }

    void UpdateAttack()
    {
        _animator.SetTrigger("Attack");
    }

    void SetNewState(BossState newState)
    {
        if (newState == BossState.Attack)
        {

        }
        currentState = newState;
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

    private void OnTriggerEnter2D(Collider2D collision)
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

    void Die()
    {
        Debug.Log("El enemigo ha muerto");

        _animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}