using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("References")]
    [SerializeField] private Transform groundCheckPoint;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Если точка для проверки земли не назначена, создаем её автоматически
        if (groundCheckPoint == null)
        {
            GameObject checkPoint = new GameObject("GroundCheck");
            checkPoint.transform.parent = transform;
            checkPoint.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheckPoint = checkPoint.transform;
        }
    }
    
    void Update()
    {
        // Получаем ввод с клавиатуры
        moveInput = Input.GetAxisRaw("Horizontal");
        
        // Проверка на земле
        isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckDistance, groundLayer);
        
        // Прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }
    
    void FixedUpdate()
    {
        // Движение по горизонтали
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }
    
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
    
    // Визуализация для отладки
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheckPoint.position, 
                           groundCheckPoint.position + Vector3.down * groundCheckDistance);
        }
    }
}
