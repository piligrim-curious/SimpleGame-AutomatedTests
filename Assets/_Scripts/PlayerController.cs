using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private Rigidbody2D rb;
    private bool isGrounded;
    
    // Переменные для ввода
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Получаем существующий компонент PlayerInput
        playerInput = GetComponent<PlayerInput>();
        
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component is missing! Please add PlayerInput component to the player.");
            return;
        }
        
        // Получаем действия из Input Actions
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        
        if (moveAction == null || jumpAction == null)
        {
            Debug.LogError("Move or Jump actions are not found in Input Actions! Check your Input Actions asset.");
        }
    }

    void Update()
    {
        // Если moveAction не найден, не обновляем движение
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }

        // Если jumpAction не найден, не обрабатываем прыжок
        if (jumpAction != null && jumpAction.WasPressedThisFrame() && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}