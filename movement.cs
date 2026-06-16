using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Автоматически создаем точку для проверки земли, если не назначена
        if (groundCheck == null)
        {
            GameObject checkPoint = new GameObject("GroundCheck");
            checkPoint.transform.parent = transform;
            checkPoint.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = checkPoint.transform;
        }
    }
    
    void Update()
    {
        // ПРОВЕРКА ЗЕМЛИ ПО ТЕГУ
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);
        
        // Сбрасываем вертикальную скорость, если на земле
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        // Получаем ввод с клавиатуры
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Вектор движения относительно поворота персонажа
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        
        // Нормализуем движение, чтобы по диагонали не бежать быстрее
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
        
        // Определяем скорость (спринт или ходьба)
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && move.magnitude > 0.1f;
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        
        // Движение персонажа
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // Поворот персонажа в сторону движения
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // ПРЫЖОК
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        
        // Применяем гравитацию
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    // Визуализация для отладки (видно в редакторе)
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
