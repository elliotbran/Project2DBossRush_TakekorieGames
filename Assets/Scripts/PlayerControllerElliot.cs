using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerElliot : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public float Life = 3f;
    public float MaximumLife = 3f;

    private enum State
    {
        Normal,
        Rolling,
    }

    private Rigidbody2D rb;
    public Vector3 moveDir;
    private Vector3 rollDir;
    private Vector3 lastMoveDir;
    private float rollSpeed = 20f;
    private State state;

    private Animator animator;

    [SerializeField] private float rollCooldown = 1f; // cooldown in seconds
    private float rollCooldownTimer = 0f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // get the Rigidbody2D component
        state = State.Normal; // start in Normal state
        animator = GetComponent<Animator>(); // get the Animator component
    }
    void Update()
    {
        // cooldown timer 
        if (rollCooldownTimer > 0f)
        {
            rollCooldownTimer -= Time.deltaTime;
        }

        switch (state) // change behavior based on state
        {
            case State.Normal:
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
                    state = State.Rolling;

                    // start cooldown
                    rollCooldownTimer = rollCooldown;
                }
                break;
            case State.Rolling:
                float rollSpeedDropMultiplier = 5f; 
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float minRollSpeed = 15f;
                if (rollSpeed < minRollSpeed)
                {
                    state = State.Normal;
                }
                break;
        }

    }

    private void FixedUpdate()
    {
        // movement based on state
        switch (state) { 
            case State.Normal:
                rb.linearVelocity = moveDir * moveSpeed;
                break;
            case State.Rolling:
                rb.linearVelocity = rollDir * rollSpeed;
                break;
        }
    }   
    public void TomarDaño(float cantidad) // damage player
    {
        Life -= cantidad;
        if (Life <= 0)
        {
            Life = 0;
            Debug.Log("El jugador ha muerto");
            Destroy(gameObject);
        }
    }
    public void Cure(float cantidad) // cure player
    {
        Life += cantidad;
        if (Life > MaximumLife)
        {
            Life = MaximumLife;
        }
    }
}
