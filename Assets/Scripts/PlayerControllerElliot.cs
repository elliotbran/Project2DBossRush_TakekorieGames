using UnityEngine;

public class PlayerControllerElliot : MonoBehaviour
{
    private const float moveSpeed = 10f;

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


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        state = State.Normal;
    }
    void Update()
    {
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

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rollDir = lastMoveDir;
                    rollSpeed = 100f;
                    state = State.Rolling;
                }
                break;
            case State.Rolling:
                float rollSpeedDropMultiplier = 5f; 
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float minRollSpeed = 50f;
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
    void Start()
    {

    }

}
