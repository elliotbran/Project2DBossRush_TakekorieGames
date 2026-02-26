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
    public DialogueUI DialogueUI => dialogueUI; //Lets other scritps access the dialogueUI without allowing them to change it
    public IInteractable interactable { get; set; } //Gets the "IInteractable" interface from the object the player is interacting with


    [Header("Player Health")]
    public float health;
    public float maxHealth = 100f;

    [Header("Player Speed")]
    [SerializeField] float _speed;
    private float _maxSpeed = 10f;

    [Header("Parry system")]
    [SerializeField] private float _parrycooldown = 1f;
    private float _parrycooldowntime = 0;
    public enum PlayerState //State machine for the player
    {
        Normal,        
        Rolling,
        Attacking,
        Parry,
        Dead,
    }
    public bool isAttacking;

    private bool canParry = false;   

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
    private Collider2D _object;
    private Animator _animator;

    public GameObject target;
    public BoxCollider2D playerHitbox;

    [Header("Combat")]
    [SerializeField] private float attackDuration = 0.25f; // how long the attack lasts (seconds)

    public UnityEngine.Transform attackPoint;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float attackRate = 2f;

    private float nextAttackTime = 0f;

    public LayerMask enemyLayers; //Its used by the boss to detect our player

    public bool canMove = true;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        _animator = GetComponent<Animator>(); // Get the Animator component
        currentState = PlayerState.Normal; // Start in Normal state
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
        // Movement based on state
        switch (currentState)
        {
            case PlayerState.Normal:
                _rb.linearVelocity = moveDir * _speed;
                break;
            case PlayerState.Rolling:
                _rb.linearVelocity = _rollDir * _rollSpeed;
                break;
            case PlayerState.Attacking:
                // While attacking, movement is restricted by reduced _speed set in Attack()
                _rb.linearVelocity = moveDir * _speed;
                break;
            case PlayerState.Parry:
                _rb.linearVelocity = Vector2.zero; // While parryign the player cannot move
                break;
            case PlayerState.Dead:
                _rb.linearVelocity = Vector2.zero; // When the player is dead, it cannot move
                break;
        }
    } 
    void Update()
    {
        if (canMove == false) return; //If canMove is false, the player cannot move or do any action
        if (dialogueUI.IsOpen) return; //Tracks if the dialogue is already open so the player doesn't open it again while it's already open
        if (_rollCooldownTimer > 0f) _rollCooldownTimer -= Time.deltaTime; 
        if (_parrycooldowntime > 0f) _parrycooldowntime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Submit")) //In case is not open, this activates it if the player is in range of an interactable object and presses the interact button
        {
            interactable?.Interact(this);
        }
        // Cooldown timer 
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
        if (Input.GetMouseButtonDown(1) && currentState == PlayerState.Normal && _parrycooldowntime <= 0f) 
        {
            _parrycooldowntime = _parrycooldown; //Inicia el Cooldown del parry
            StartCoroutine(ParryWindowRoutine()); //Llama a la corrutina ParryWindowRoutine()
        }
        UpdateStates();

        Ray ray = new Ray(transform.position, _playerController.moveDir); //Raycast that shows the direction the player is moving
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
        switch (currentState) // Change behavior based on state
        {
            case PlayerState.Normal:
                HandleMovement();               
                break;
            case PlayerState.Rolling:
                HandleRolling();
                break;

            case PlayerState.Attacking:
                break;
               
            case PlayerState.Parry:
                if (canParry)
                {
                    HandleParry(); // Calls "handleParry" when the player is parryng and canParry is true
                }
                break;
            case PlayerState.Dead:
                break;
        }
    }

    public void Cure(float quantity) // Heals the player with the potion
    {
        health += quantity;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    

    #region Movement
    void HandleMovement() // Normal movement and roll initiation
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
        _animator.SetFloat("MoveX", moveX);
        _animator.SetFloat("MoveY", moveY);
        _animator.SetFloat("MoveMagnitude", moveDir.magnitude);
        _animator.SetFloat("LastMoveX", _lastMoveDir.x);
        _animator.SetFloat("LastMoveY", _lastMoveDir.y);


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
        
    void HandleRolling() // Rolling behavior and cooldown management
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

    public void ReceiveDamage(float quantity) // Damage player
    {
        health -= quantity;
        _animator.SetTrigger("Hurt"); // Trigger hurt animation

        if (health <= 0)
        {
            health = 0;
            currentState = PlayerState.Dead;
            Debug.Log("El jugador ha muerto");
            // Trigger death animation, disable player controls, etc.
            _animator.SetBool("IsDead", true);
            playerHitbox.enabled = false; // Disable hitbox to prevent further damage
            this.enabled = false; // Disable this script to stop player movement and actions
        }
    }

    void Attack() // Initiate attack sequence
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
    IEnumerator ParryWindowRoutine()
    {
        currentState = PlayerState.Parry; //cambia el estado al estado del parry
        Debug.Log("Parry Activado");
        _playerAnimator.SetTrigger("Parry"); //activa la animacion del parry
        canParry = false;
        yield return new WaitForSeconds(0.40f); //tiempo del parry
        canParry = true;
        currentState = PlayerState.Normal; //vuelve al estado nromal
    }
    void HandleParry()
    {
        if (canParry && _object != null) //detecta el objeto y mira que tag le corresponde
        {
            if (_object.CompareTag("AtaqueAmarillo")) //Objeto con el tag AtaqueAmarillo rellena 1 de mana y destrulle el objeto
            {
                if (_manacontroller != null) _manacontroller.RefillMana(1f);
                Debug.Log("parreando");
                Destroy(_object.gameObject);
                Debug.Log("destruido");
            }
            else if (_object.CompareTag("AtaqueNormal")) //Objeto con el tag AtaqueNormal no parrea hace 25 de daño y se destruye el objeto
            {
                ReceiveDamage(25f);
                Destroy(_object.gameObject);
                Debug.Log("No parreando Daño recibido");
            }
            canParry = false;
            _object = null;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) //Si hay un objeto con el tag AtaqueAmarillo o AtaqueNormal, guarda el objeto y activa el parry
    {
        if (collision.CompareTag("AtaqueAmarillo") || collision.CompareTag("AtaqueNormal"))
        {
            _object = collision;
            canParry = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision) //si el parry no llega al objeto se cancela el parry
    {
        if (collision == _object)
        {
            _object = null;
            canParry = false;
        }
    }
    private IEnumerator AttackRoutine() // Manages the attack lifecycle and resets state after attackDuration
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

    private void OnDrawGizmosSelected() // Visualize attack range in editor
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    #endregion

}
