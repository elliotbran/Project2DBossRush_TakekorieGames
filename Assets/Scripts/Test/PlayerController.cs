using UnityEngine;
using UnityEngine.InputSystem;  

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private InputActionReference moveActionReference;

    public Rigidbody2D rb;
    public Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();        
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movementInput.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        movementInput = Vector2.zero;
    }

    private void OnEnable()
    {
        moveActionReference.action.Enable();
        moveActionReference.action.performed += OnMovePerformed;
        moveActionReference.action.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        moveActionReference.action.performed -= OnMovePerformed;
        moveActionReference.action.canceled -= OnMoveCanceled;
        moveActionReference.action.Disable();
    }
}
