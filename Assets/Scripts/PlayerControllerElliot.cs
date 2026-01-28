using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerElliot : MonoBehaviour
{
    private const float moveSpeed = 5f;
    public float Vida = 3f;
    public float vidaMaxima = 3f;

    private enum State
    {
        Normal,
        Rolling,
    }

    private Rigidbody2D rb;
    public Vector3 moveDir;
    private Vector3 rollDir;
    private Vector3 lastMoveDir;
    private float rollSpeed = 30f;
    private State state;

    [SerializeField] private float rollCooldown = 1f; // cooldown in seconds
    private float rollCooldownTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        state = State.Normal;
    }
    void Update()
    {
        // cooldown timer update
        if (rollCooldownTimer > 0f)
        {
            rollCooldownTimer -= Time.deltaTime;
        }

        switch (state)
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
                if(moveX != 0 || moveY != 0)
                {
                    // Not Idle
                    lastMoveDir = moveDir;
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
                float rollSpeedDropMultiplier = 5; 
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
        switch (state) { 
            case State.Normal:
                rb.linearVelocity = moveDir * moveSpeed;
                break;
            case State.Rolling:
                rb.linearVelocity = rollDir * rollSpeed;
                break;
        }
    }
    /*private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.CompareTag("Enemy"))
        {
            TomarDaño(1f);
        }
    }*/
    public void TomarDaño(float damagerecive)
    {
        Vida -= damagerecive;
        if (Vida <= 0)
        {
            Vida = 0;
            Debug.Log("El jugador ha muerto");
            Destroy(gameObject);
        }
    }
    public void Curar(float cantidad)
    {
        Vida += cantidad;
        if (Vida > vidaMaxima)
        {
            Vida = vidaMaxima;
        }
    }

}
