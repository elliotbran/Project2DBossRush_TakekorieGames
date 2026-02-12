using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class EnemyController : MonoBehaviour
{
    [Header("Health")]
    public float damage = 1f;
    public float life = 3f;
    public float maximumLife = 3f;
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

    public void Damage(float amount)
    {
        life -= amount;
        Debug.Log("Vida restante" + life);
        if (life <= 0)
        {
            life = 0;
            Debug.Log("El enemigo ha muerto");
            Destroy(gameObject);
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
                Debug.Log("Daño realizado. Vida restante: " + player.life);
            }
        }
    }
}