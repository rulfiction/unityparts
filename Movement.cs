using UnityEngine;

public class PlayerMovement3D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;
    private float xRotation = 0f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Автоматически создаем точку для проверки земли
        if (groundCheck == null)
        {
            GameObject checkPoint = new GameObject("GroundCheck");
            checkPoint.transform.parent = transform;
            checkPoint.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = checkPoint.transform;
        }
        
        // Если камера не назначена - ищем её
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        
        // Блокируем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        // Проверка земли
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        // --- УПРАВЛЕНИЕ МЫШЬЮ ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Поворот персонажа по горизонтали (влево-вправо)
        transform.Rotate(Vector3.up * mouseX);
        
        // Поворот камеры по вертикали (вверх-вниз)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // --- ДВИЖЕНИЕ ---
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Движение ВПЕРЕД/НАЗАД/ВЛЕВО/ВПРАВО относительно направления камеры
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        // Убираем наклон по вертикали для движения по горизонтали
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        Vector3 move = forward * vertical + right * horizontal;
        
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
        
        // Спринт
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && move.magnitude > 0.1f;
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        
        // Движение
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // --- ПРЫЖОК ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        
        // Гравитация
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        // Отключение курсора по Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Возврат курсора по клику
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
