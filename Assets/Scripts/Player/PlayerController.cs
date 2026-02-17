using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;

    public IInteractable interactable { get; set; }
    [Header("Player Health")]
    public float health;
    public float maxHealth = 100f;

    [Header("Player Speed")]
    [SerializeField] float _speed;
    private float _maxSpeed = 10f;

    [Header("Parry system")]
    private Collider2D Object;
    private bool canParry = false;

    

    public enum PlayerState
    {
        Normal,        
        Rolling,
        Attacking,
        Parry,
    }
    public bool isAttacking;

    public Vector3 moveDir;

    private Vector3 _rollDir;
    private Vector3 _lastMoveDir;

    [SerializeField] private float _rollCooldown = 1f; // cooldown in seconds
    private float _rollSpeed = 20f;

    public PlayerState currentState;

    private float _rollCooldownTimer = 0f;

    public Camera mainCamera;
    private PlayerController _playerController;
    private ManaController _manacontroller;
    private Animator _playerAnimator;
    private Rigidbody2D _rb;
    private Animator _animator;

    public GameObject target;

    [Header("Combat")]
    [SerializeField] private float attackDuration = 0.25f; // how long the attack lasts (seconds)

    public UnityEngine.Transform attackPoint;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;

    private float nextAttackTime = 0f;

    public LayerMask enemyLayers;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); // get the Rigidbody2D component
        currentState = PlayerState.Normal; // start in Normal state
        _animator = GetComponent<Animator>(); // get the Animator component
    }
    void Start()
    {
        _speed = _maxSpeed;
        health = maxHealth;
        _playerController = GetComponent<PlayerController>();
        _manacontroller = GameObject.FindAnyObjectByType<ManaController>();
        _playerAnimator = GetComponent<Animator>();
        if (target != null)
        {
            target.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        // movement based on state
        switch (currentState)
        {
            case PlayerState.Normal:
                _rb.linearVelocity = moveDir * _speed;
                break;
            case PlayerState.Rolling:
                _rb.linearVelocity = _rollDir * _rollSpeed;
                break;
            case PlayerState.Attacking:
                // while attacking, movement is restricted by reduced _speed set in Attack()
                _rb.linearVelocity = moveDir * _speed;
                break;
            case PlayerState.Parry:
                _rb.linearVelocity = Vector2.zero;
                break;
        }
    } 
    void Update()
    {
        if (dialogueUI.IsOpen) return;
        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Submit"))
        {
            interactable?.Interact(this);
        }
        // cooldown timer 
        if (_rollCooldownTimer > 0f)
        {
            _rollCooldownTimer -= Time.deltaTime;
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                Debug.Log("ATAQUE INICIADO");
                // set next attack time (cooldown)
                nextAttackTime = Time.time + 1f / attackRate;
                Attack(); // Attack will handle isAttacking and its reset
            }
        }
        if (Input.GetMouseButtonDown(1) && currentState == PlayerState.Normal) 
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
        switch (currentState) // change behavior based on state
        {
            case PlayerState.Normal:
                HandleMovement();               
                break;


            case PlayerState.Rolling:
                HandleRolling();
                break;

            case PlayerState.Attacking:
                // Do nothing here; Attack() manages the attack lifecycle via coroutine
                break;
               
            case PlayerState.Parry:
                HandleParry();
                break;
        }
    }
    IEnumerator ParryWindowRoutine()
    {
        currentState = PlayerState.Parry;
        Debug.Log("Parry Activado");
        _playerAnimator.SetTrigger("Parry");
        yield return new WaitForSeconds(0.40f);
        currentState = PlayerState.Normal;
    }
    void HandleParry()
    {
        if (canParry && Object != null)
        {
            if (Object.CompareTag("AtaqueAmarillo"))
            {
                if (_manacontroller != null) _manacontroller.RefillMana(1f);
                Debug.Log("parreando");
            }
            else if (Object.CompareTag("AtaqueNormal"))
            {
                ReceiveDamage(25f);
                Debug.Log("No parreando Daño recibido");
            }
            canParry = false;
            Object = null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AtaqueAmarillo")|| collision.CompareTag("AtaqueNormal"))
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
        _speed = _maxSpeed;

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
            currentState = PlayerState.Rolling;

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
            currentState = PlayerState.Normal;
        }
    }
    #endregion


    #region Combat

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

    void Attack()
    {
        // Prevent starting a new attack while one is active
        if (isAttacking)
            return;

        // Activate attack bool
        isAttacking = true;

        // Change state to Attacking
        currentState = PlayerState.Attacking;

        // Restrict player movement 
        _speed = _maxSpeed / 4f;

        // Play attack animation
        if (_playerAnimator != null)
            _playerAnimator.SetTrigger("Attack");

        // Activate target hitbox (if you use it)
        if (target != null)
            target.SetActive(true);

        // Detect enemies in range of attack and damage them immediately
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            var enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(attackDamage);
                Debug.Log("We hit " + enemy.name);
            }
        }

        // Start coroutine to finish the attack after duration
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(attackDuration);

        // Deactivate target hitbox
        if (target != null)
            target.SetActive(false);

        // Reset attack flags and state
        isAttacking = false;
        currentState = PlayerState.Normal;
        _speed = _maxSpeed;

        Debug.Log("ATAQUE FINALIZADO");
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
