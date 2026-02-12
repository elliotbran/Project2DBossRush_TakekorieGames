using UnityEngine;
using UnityEngine.AI;

public class EnemyAITest : MonoBehaviour
{
    [SerializeField] Transform target;

    NavMeshAgent agent;

    Animator animator;


    private enum BossState
    {
        Idle,
        Chase,
    }

    private BossState currentState;
           
    private void Start()
    {
        currentState = BossState.Idle;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {

        switch(currentState)
        {
            case BossState.Idle:    
                break;
            case BossState.Chase:
                if (agent.speed > 0)
                {
                    animator.SetFloat("Speed", Mathf.Abs(agent.speed));
                }
                break;
        }      

        agent.SetDestination(target.position);
    }  
}
