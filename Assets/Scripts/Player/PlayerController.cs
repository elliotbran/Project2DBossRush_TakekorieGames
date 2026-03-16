using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI; //Lets other scritps access the dialogueUI without allowing them to change it
    public IInteractable interactable { get; set; } //Gets the "IInteractable" interface from the object the player is interacting with

    [Header("Player Health")]
    public float health;
    public float maxHealth = 100f;

    [Header("Player Movement")]
    [SerializeField] float _speed;
    public Vector3 moveDir;
    private float _maxSpeed = 10f;
    public bool canMove = true;
    public bool canAttack;

    [Header("Player Dashing")]
    [SerializeField] private float _dashCooldown = 1f; // cooldown in seconds
    private float _dashSpeed = 20f;

    [Header("Player Combat")]
    [SerializeField] private float attackDuration = 0.25f; // how long the attack lasts (seconds)
    public float attackRange = 0.5f;
    public float attackRate = 2f;
    private float nextAttackTime = 0f;
    public int attackDamage = 40;
    public UnityEngine.Transform attackPoint;
    public bool isAttacking;
    public LayerMask enemyLayers; //Its used by the boss to detect our player

    [Header("Parry System")]
    [SerializeField] private float _parryCooldown = 1f;
    private float _parryCooldownTime = 0;

    public enum PlayerState //State machine for the player
    {
        Normal,        
        Dashing,
        Attacking,
        Parrying,
        Healing,
        Dead,
    }
    private bool canParry = false;   
    private bool isParrying = false;

    private Vector3 _rollDir;
    private Vector3 _lastMoveDir;       

    public PlayerState currentState;

    private float _dashCooldownTimer = 0f;

    public Camera mainCamera;
    public GameObject target;
    [SerializeField] CapsuleCollider2D _playerHitbox;

    private Animator _playerAnimator;
    [SerializeField] ParticleSystem _bloodParticlesPlayer;
    private Rigidbody2D _rb;
    private Collider2D _object;
    private Animator _animator;

    //Scripts
    private PlayerController _playerController;
    private PlayerParryShake _playerParryShake;            
    public ManaParticleHandler manaHandler;

    public GameObject youDiedPanel;


    public bool autoTrigger = false;
    void Awake()
    {
        youDiedPanel.SetActive(false); // Ensure "You Died" panel is hidden at start
        _rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        _animator = GetComponent<Animator>(); // Get the Animator component
        _playerHitbox = GetComponent<CapsuleCollider2D>(); // Get the BoxCollider2D component
        currentState = PlayerState.Normal; // Start in Normal state
    }
    void Start()
    {
        _speed = _maxSpeed;
        health = maxHealth;
        _playerController = GetComponent<PlayerController>();
        _playerParryShake = GetComponent<PlayerParryShake>();
        _playerAnimator = GetComponent<Animator>();
        _bloodParticlesPlayer.Stop();
        if (target != null)
        {
            target.SetActive(false);
        }
    }
    void FixedUpdate()
{
    if (!canMove)
    {
        _rb.linearVelocity = Vector2.zero;
        return;
    }

        // Movement based on state
        switch (currentState)
        {
            case PlayerState.Normal:
                _rb.linearVelocity = moveDir * _speed;
                _playerHitbox.enabled = true; // Ensure hitbox is enabled during normal movement
                break;
            case PlayerState.Dashing:
                _rb.linearVelocity = _rollDir * _dashSpeed;
                _playerHitbox.enabled = false; // Disable hitbox to prevent damage while dashing
                break;
            case PlayerState.Attacking:
                // While attacking, movement is restricted by reduced _speed set in Attack()
                _rb.linearVelocity = moveDir * _speed;
                break;
            case PlayerState.Healing:
                _rb.linearVelocity = Vector2.zero; // While healing the player cannot move
                break;
            case PlayerState.Parrying:
                _rb.linearVelocity = Vector2.zero; // While parryign the player cannot move
                break;
            case PlayerState.Dead:
                canMove = false;
                // Detener completamente al jugador
                moveDir = Vector3.zero;
                _rb.linearVelocity = Vector2.zero;

                // Parar animación de movimiento
                _animator.SetFloat("MoveMagnitude", 0);
                _rb.linearVelocity = Vector2.zero; // When the player is dead, it cannot move
                break;
        }
    } 
    void Update()
    {
        if (dialogueUI.IsOpen)
        {
            canMove = false;

            // Detener completamente al jugador
            moveDir = Vector3.zero;
            _rb.linearVelocity = Vector2.zero;

            // Parar animación de movimiento
            _animator.SetFloat("MoveMagnitude", 0);

            return;
        }
 //Tracks if the dialogue is already open so the player doesn't open it again while it's already open
            
        if (_dashCooldownTimer > 0f) _dashCooldownTimer -= Time.deltaTime; 
        if (_parryCooldownTime > 0f) _parryCooldownTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Submit")) //In case is not open, this activates it if the player is in range of an interactable object and presses the interact button
        {
            interactable?.Interact(this);
        }

        if (autoTrigger) //This is used for the boss fight, it automatically triggers the dialogue when the player enters the trigger area
        {
            interactable?.Interact(this);
            autoTrigger = false; //Reset autoTrigger to prevent multiple triggers
            //Debug.Log("AutoTrigger activated");
        }

        // Cooldown timer 
        if (_dashCooldownTimer > 0f)
        {
            _dashCooldownTimer -= Time.deltaTime;
        }

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetButton("Attack")) 
            {
                Debug.Log("ATAQUE INICIADO");
                // set next attack time (cooldown)
                nextAttackTime = Time.time + 1f / attackRate;
                Attack(); // Attack will handle isAttacking and its reset
            }
        }

        float _leftTrigger;
        _leftTrigger = Input.GetAxis("ParryL2"); // Get the value of the right trigger for dashing
        //Debug.Log("Left Trigger Value: " + _leftTrigger); // Log the value of the left trigger for debugging
        if (Input.GetMouseButtonDown(1) && currentState == PlayerState.Normal && _parryCooldownTime <= 0f || Input.GetButtonDown("Parry") && currentState == PlayerState.Normal && _parryCooldownTime <= 0f || _leftTrigger > 0.5f && currentState == PlayerState.Normal && _parryCooldownTime <= 0f) 
        {
            _parryCooldownTime = _parryCooldown; //Inicia el Cooldown del parry
            StartCoroutine(ParryWindowRoutine()); //Llama a la corrutina ParryWindowRoutine()
        }

        UpdateStates();

        Ray ray = new Ray(transform.position, _playerController.moveDir); //Raycast that shows the direction the player is moving
        Debug.DrawRay(ray.origin, ray.direction*5f, Color.red);

        /////////------------NO TOCAR------------/////////
        /*Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        target.transform.rotation = Quaternion.Euler(0, 0, angle - 90);*/
        /////////------------NO TOCAR------------/////////             
    }

    #region Movement
    void UpdateStates()
    {
        switch (currentState) // Change behavior based on state
        {
            case PlayerState.Normal:
                HandleMovement();               
                break;
            case PlayerState.Dashing:
                HandleDashing();
                break;

            case PlayerState.Attacking:
                break;
               
            case PlayerState.Parrying:
                if (canParry)
                {
                    HandleParry(); // Calls "handleParry" when the player is parrying and canParry is true
                }
                break;
            case PlayerState.Healing:
                break;
            case PlayerState.Dead:
                break;
        }
    }    
    void HandleMovement() // Normal movement and roll initiation
    {
        _playerHitbox.enabled = true; // Ensure hitbox is enabled during normal movement
        if (canMove == false) return;
        _speed = _maxSpeed;

        float moveX = 0f;
        moveX = Input.GetAxisRaw("Horizontal");
        float moveY = 0f;
        moveY = Input.GetAxisRaw("Vertical");

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

        // Seamlessly blend between idle and movement animations by setting MoveX, MoveY, and MoveMagnitude parameters
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
        float _rightTrigger;
        _rightTrigger = Input.GetAxis("DashR2"); // Get the value of the right trigger for dashing
        

        if (Input.GetKeyDown(KeyCode.Space) && _dashCooldownTimer <= 0f || Input.GetButtonDown("Dash") && _dashCooldownTimer <= 0f || _rightTrigger >0.7f && _dashCooldownTimer <= 0f)
        {

            // fallback direction if player hasn't moved yet
            if (_lastMoveDir == Vector3.zero)
            {
                _lastMoveDir = Vector3.right;
            }

            _rollDir = _lastMoveDir;
            _dashSpeed = 30f;
            currentState = PlayerState.Dashing;

            // start cooldown
            _dashCooldownTimer = _dashCooldown;
        }
    }        
    void HandleDashing() // Rolling behavior and cooldown management
    {
        float rollSpeedDropMultiplier = 5f;
        _dashSpeed -= _dashSpeed * rollSpeedDropMultiplier * Time.deltaTime;
        Shadows.me.Sombras_Skill();
        _playerHitbox.enabled = false; // Disable hitbox to prevent damage while dashing

        float minRollSpeed = 15f;
        if (_dashSpeed < minRollSpeed)
        {
            currentState = PlayerState.Normal;
        }
    }
    #endregion

    #region Health and Healing
    public void TakeDamage(float quantity) // Damage player
    {
        _bloodParticlesPlayer.Play();
        if (isParrying) // If the player can parry, they will parry instead of taking damage
        {
            return;
        }

        else
        {
            health -= quantity;
            _animator.SetTrigger("Hurt"); // Trigger hurt animation
        }

        if (health <= 0)
        {
            health = 0;
            currentState = PlayerState.Dead;
            canMove = true;
            Debug.Log("El jugador ha muerto");
            // Trigger death animation, disable player controls, etc.
            _animator.SetBool("IsDead", true);
            StartCoroutine(WaitForDisablingScript()); // Start coroutine to disable the script after a short delay
        }
    }

    public void Heal(float quantity) // Heals the player with the potion
    {
        health += quantity;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    IEnumerator WaitForDisablingScript()
    {
        yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds before disabling the script 
        _playerHitbox.enabled = false; // Disable hitbox to prevent further damage
        this.enabled = false; // Disable this script to stop player movement and actions
        yield return new WaitForSeconds(1f); // Wait for 1 second before showing the "You Died" panel
        youDiedPanel.SetActive(true); // Show "You Died" panel
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene(1);
    }
    #endregion

    #region Combat
    void Attack() // Initiate attack sequence
    {
        if(canAttack == false) return;
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
            var enemyController = enemy.GetComponent<BossController>();
            var tutorialController = enemy.GetComponent<EnemyTutorialController>();
            if (enemyController != null)
            {
                StartCoroutine(AttackHitStop()); // Start hit stop effect
                enemyController.TakeDamage(attackDamage);
                Debug.Log("We hit " + enemy.name);
            }
            if (tutorialController != null)
            {
                StartCoroutine(AttackHitStop()); // Start hit stop effect
                tutorialController.TakeDamage(attackDamage);
                Debug.Log("We hit " + enemy.name);
            }
        }

        // Start coroutine to finish the attack after duration
        StartCoroutine(AttackRoutine());
    }
    void HandleParry()
    {
        if (canParry && _object != null) 
        { 
            if (_object.CompareTag("AtaqueMelee")) //Objeto con el tag AtaqueMelee rellena 1 de mana con las particulas de mana, parrea el ataque mana y sacude la camaa
            {
                _bloodParticlesPlayer.Stop(); //Desactiva partículas de sangre
                if (manaHandler != null && !manaHandler.manaController.potioncontroller.IsFull)
                {
                    manaHandler.SpawnMana(5); //Suelta 5 bolas de particulas de mana
                }
                if (_playerParryShake != null) //la camara se sacude 
                {
                    StartCoroutine(ParryHitStop()); //Start hit stop effect
                    _playerParryShake.TriggerShake();
                }
                Debug.Log("Parry Melee");
                _object = null;
                canParry = false;
            }
            else if (_object.CompareTag("AtaqueAmarillo")) //Objeto con el tag AtaqueAmarillo rellena 1 de mana con las particulas de mana, sacude la camara y destrulle el objeto
            {
                _bloodParticlesPlayer.Stop();
                if (!manaHandler.manaController.potioncontroller.IsFull)
                {
                    if (manaHandler != null) //Suelta 5 bolas de particulas de mana 
                    {
                        manaHandler.SpawnMana(5);
                    }
                    if (_playerParryShake != null) //la camara se sacude 
                    {
                        StartCoroutine(ParryHitStop()); //Start hit stop effect
                        _playerParryShake.TriggerShake();
                    }
                }
                Debug.Log("Parreado");
                Destroy(_object.gameObject);
                _object = null;
                canParry = false;
            }
            else if (_object.CompareTag("AtaqueNormal")) //Objeto con el tag AtaqueNormal no pparrea hace25 de daño y se destruye el objeto
            {
                TakeDamage(25f); //25 de daño 
                //Destroy(_object.gameObject);
                _object = null;
                canParry = false;
                Debug.Log("No parreado");
            }
            canParry = false;           
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) //Si hay un objeto con el tag AtaqueAmarillo o AtaqueNormal, guarda el objeto y activa el parry
    {
        if (collision.CompareTag("AtaqueAmarillo") || collision.CompareTag("AtaqueNormal") || collision.CompareTag("AtaqueMelee"))
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
    IEnumerator AttackRoutine() // Manages the attack lifecycle and resets state after attackDuration
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
    IEnumerator ParryWindowRoutine()
    {
        isParrying = true;
        _playerHitbox.enabled = false; // Disable hitbox to prevent further damage
        currentState = PlayerState.Parrying; //cambia el estado al estado del parry
        Debug.Log("Parry Activado");
        _playerAnimator.SetTrigger("Parry"); //activa la animacion del parry

        canParry = false;
        yield return new WaitForSeconds(.2f); //tiempo del parry
        canParry = true;
        isParrying = false;
        _playerHitbox.enabled = true; // Disable hitbox to prevent further damage
        currentState = PlayerState.Normal; //vuelve al estado nromal
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

    #region Hitstop
    IEnumerator AttackHitStop()
    {
        Time.timeScale = 0.1f; // Slow down time to create hit stop effect
        yield return new WaitForSecondsRealtime(0.1f); // Wait for a short duration in real time
        Time.timeScale = 1; // Restore original time scale
    }

    IEnumerator ParryHitStop()
    {
        Time.timeScale = 0.2f; // Slow down time to create hit stop effect
        yield return new WaitForSecondsRealtime(0.3f); // Wait for a short duration in real time
        Time.timeScale = 1; // Restore original time scale
    }    
    #endregion
}
