using UnityEngine;
using UnityEngine.AI;

public class EnemyAITest : MonoBehaviour
{
    [SerializeField] Transform player;

    // Components
    NavMeshAgent _agent;        
    Animator _animator;

    // Change from Idle to Chase

    // If the player is whitin a certain range, Attack
    private enum BossState
    {
        Idle,
        Chase,
        Attack,
    }

    private BossState currentState = BossState.Idle;
           
    private void Start()
    {
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

    void SetNewState (BossState newState)
    {
        if(newState == BossState.Attack) 
        {

        }
        currentState = newState;
    }

}
