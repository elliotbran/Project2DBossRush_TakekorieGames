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

    [Header("Player Speed")]
    [SerializeField] float _speed;
    private float _maxSpeed = 10f;

    [Header("Parry")]
    private Collider2D Object;
    private bool canParry = false;
    public enum PlayerState
    {
        Normal,        
        Rolling,
        Attacking,
        Parry,
    }
    public bool attacking;

    public Vector3 moveDir;

    private Vector3 _rollDir;
    private Vector3 _lastMoveDir;

    [SerializeField] private float _rollCooldown = 1f; // cooldown in seconds
    private float _rollSpeed = 20f;

    public PlayerState playerState;

    private float _rollCooldownTimer = 0f;

    public Camera mainCamera;
    private PlayerController _playerController;
    private ManaController _maanaController;
    private Animator _playerAnimator;
    private Rigidbody2D _rb;
    private Animator _animator;

    public GameObject target;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); // get the Rigidbody2D component
        playerState = PlayerState.Normal; // start in Normal state
        _animator = GetComponent<Animator>(); // get the Animator component
    }
    void Start()
    {
        _speed = _maxSpeed;
        health = maxHealth;
        _playerController = GetComponent<PlayerController>();
        _maanaController = GameObject.FindAnyObjectByType<ManaController>();
        _playerAnimator = GetComponent<Animator>();
        target.SetActive(false);
    }

    private void FixedUpdate()
    {
        // movement based on state
        switch (playerState)
        {
            case PlayerState.Normal:
                _rb.linearVelocity = moveDir * _speed;
                break;
            case PlayerState.Rolling:
                _rb.linearVelocity = _rollDir * _rollSpeed;
                break;
            case PlayerState.Parry:
                _rb.linearVelocity = Vector2.zero; 
                break;
        }
    } 
    void Update()
    {
        // cooldown timer 
        if (_rollCooldownTimer > 0f)
        {
            _rollCooldownTimer -= Time.deltaTime;
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                attacking = true;
                Debug.Log("ATAQUE INICIADO");
                StartCoroutine(AttackTiming());
                nextAttackTime = Time.time + 1f / attackRate;
                Debug.Log("ATAQUE FINALIZADO");
            }
        }
        if (Input.GetMouseButtonDown(1) && playerState == PlayerState.Normal)
        {
            StartCoroutine(ParryWindowRoutine());
        }
       
        UpdateStates();

        Ray ray = new Ray(transform.position, _playerController.moveDir);
        Debug.DrawRay(ray.origin, ray.direction*5f, Color.red);

        /////////------------NO TOCAR------------/////////
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
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
                HandleMovement();               
                break;

            case PlayerState.Rolling:
                HandleRolling();
                break;

            case PlayerState.Attacking:
                HandleAttack();
                break;

            case PlayerState.Parry:
                HandleParry();
                break;

        }
    }
    IEnumerator ParryWindowRoutine()
    {
        playerState = PlayerState.Parry;
        Debug.Log("ParryActivado");
        yield return new WaitForSeconds(0.25f);
        playerState = PlayerState.Normal;
    }
    void HandleParry()
    {
        if (canParry && Object != null)
        {
            if (Object.CompareTag("AtaqueAmarillo"))
            {
                if (_maanaController != null) _maanaController.RefillMana(1f);
                Debug.Log("parreado");
            }
            else if (Object.CompareTag("AtaqueNormal"))
            {
                ReceiveDamage(25f);
                Debug.Log("No parreado Daño recibido");
            }
            canParry = false;
            Object = null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AtaqueAmarillo") || collision.CompareTag("AtaqueNormal"))
        {
            Object = collision;
            canParry = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == Object)
        {
            Object = null;
            canParry = false;
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

    #region Movement
    void HandleMovement()
    {
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
        _animator.SetFloat("Speed", Mathf.Abs(moveX) + Mathf.Abs(moveY));


        if (moveX != 0 || moveY != 0)
        {
            // Not Idle
            _lastMoveDir = moveDir;


            // Swap direction of sprite depending on walk direction
            if (moveX > 0)
                transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else if (moveX < 0)
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        }

        // Only allow roll if cooldown has expired
        if (Input.GetKeyDown(KeyCode.Space) && _rollCooldownTimer <= 0f)
        {
            // fallback direction if player hasn't moved yet
            if (_lastMoveDir == Vector3.zero)
            {
                _lastMoveDir = Vector3.right;
            }

            _rollDir = _lastMoveDir;
            _rollSpeed = 30f;
            playerState = PlayerState.Rolling;

            // start cooldown
            _rollCooldownTimer = _rollCooldown;
        }
    }

    void HandleRolling()
    {
        float rollSpeedDropMultiplier = 5f;
        _rollSpeed -= _rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

        float minRollSpeed = 15f;
        if (_rollSpeed < minRollSpeed)
        {
            playerState = PlayerState.Normal;
        }
    }
    #endregion


    #region Combat

    public UnityEngine.Transform attackPoint;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;

    private float nextAttackTime = 0f;

    public LayerMask enemyLayers;

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

    void HandleAttack()
    {
        StartCoroutine(AttackTiming());
    }
    void Attack()
    {
        // Restrict player movement 
        _speed = _speed / 4;

        // Play attack animation
        _playerAnimator.SetTrigger("Attack");

        // Detect enemies in range of attack
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        foreach (Collider2D enemy in hitEnemies)
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
    IEnumerator AttackTiming()
    { 
        target.SetActive(true);//Esto activa HitRange que es la zona de daño, se pondra en el animator
        attacking = true;
        Attack();
        yield return new WaitForSeconds(.25f);//Cuanto dura el ataque
        target.SetActive(false);
        attacking = false;
        _speed = _maxSpeed;

    }


    #endregion
}
