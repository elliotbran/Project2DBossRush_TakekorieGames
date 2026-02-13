using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    [Header("Player Health")]
    public float health;
    public float maxHealth = 100f;

    public Vector3 moveDir;

    [Header("Player Speed")]
    [SerializeField] private float moveSpeed;

    public enum PlayerState
    {
        Normal,        
        Rolling,
        Attacking,
    }

    private Rigidbody2D rb;
    private Animator animator;

    private Vector3 rollDir;
    private Vector3 lastMoveDir;

    private float rollSpeed = 20f;
    [SerializeField] private float rollCooldown = 1f; // cooldown in seconds

    public PlayerState playerState;

    private float rollCooldownTimer = 0f;

    public Camera maincamera;
    public bool attacking;
    private PlayerController _playerController;
    private Animator _playerAnimator;

    public GameObject target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // get the Rigidbody2D component
        playerState = PlayerState.Normal; // start in Normal state
        animator = GetComponent<Animator>(); // get the Animator component
    }
    void Start()
    {
        health = maxHealth;
        _playerController = GetComponent<PlayerController>();
        _playerAnimator = GetComponent<Animator>();
        target.SetActive(false);
    }

    void Update()
    {
        // cooldown timer 
        if (rollCooldownTimer > 0f)
        {
            rollCooldownTimer -= Time.deltaTime;
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("ATAQUE INICIADO");
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
                Debug.Log("ATAQUE FINALIZADO");
            }
        }
       

        UpdateStates();

        Ray ray = new Ray(transform.position, _playerController.moveDir);
        Debug.DrawRay(ray.origin, ray.direction*5f, Color.red);

        /////////------------NO TOCAR------------/////////
        Vector3 mouseWorldPos = maincamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        target.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        /////////------------NO TOCAR------------/////////
        
        
    }

    void UpdateStates()
    {
        switch (playerState) // change behavior based on state
        {
            case PlayerState.Normal:
                float moveX = 0f;
                float moveY = 0f;

                if (Input.GetKey(KeyCode.W))
                {
                    moveY = 1f;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveY = -1f;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    moveX = -1f;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    moveX = 1f;
                }

                moveDir = new Vector3(moveX, moveY).normalized;
                animator.SetFloat("Speed", Mathf.Abs(moveX) + Mathf.Abs(moveY));


                if (moveX != 0 || moveY != 0)
                {
                    // Not Idle
                    lastMoveDir = moveDir;


                    // Swap direction of sprite depending on walk direction
                    if (moveX > 0)
                        transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    else if (moveX < 0)
                        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                }

                // Only allow roll if cooldown has expired
                if (Input.GetKeyDown(KeyCode.Space) && rollCooldownTimer <= 0f)
                {
                    // fallback direction if player hasn't moved yet
                    if (lastMoveDir == Vector3.zero)
                    {
                        lastMoveDir = Vector3.right;
                    }

                    rollDir = lastMoveDir;
                    rollSpeed = 30f;
                    playerState = PlayerState.Rolling;

                    // start cooldown
                    rollCooldownTimer = rollCooldown;
                }
                break;


            case PlayerState.Rolling:
                float rollSpeedDropMultiplier = 5f;
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float minRollSpeed = 15f;
                if (rollSpeed < minRollSpeed)
                {
                    playerState = PlayerState.Normal;
                }
                break;

            case PlayerState.Attacking:
                
                break;
        }
    }
    private void FixedUpdate()
    {
        // movement based on state
        switch (playerState)
        {
            case PlayerState.Normal:
                rb.linearVelocity = moveDir * moveSpeed;
                break;
            case PlayerState.Rolling:
                rb.linearVelocity = rollDir * rollSpeed;
                break;
        }
    }

    void UpdateIdle()
    {
        //Setear el animator
        //Recuperar stamina
    }

    void UpdateAttack()
    {

    }
        

    IEnumerator Attacktimeing()
    { 
        target.SetActive(true);//Esto activa HitRange que es la zona de daño, se pondra en el animator
        attacking = true;
        Attack();
        yield return new WaitForSeconds(1f);//Cuanto dura el ataque
        target.SetActive(false);
        attacking = false;

    }
    public void ReceiveDamage(float quantity) // damage player
    {
        health -= quantity;
        if (health <= 0)
        {
            health = 0;
            Debug.Log("El jugador ha muerto");
            Destroy(gameObject);
        }
    }
    public void Cure(float quantity) // cure player
    {
        health += quantity;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    #region Combat

    public UnityEngine.Transform attackPoint;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;

    private float nextAttackTime = 0f;


    public LayerMask enemyLayers;


    void Attack()
    {
        // Play attack animation
        _playerAnimator.SetTrigger("Attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
            Debug.Log("We hit " + enemy.name);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion
}
