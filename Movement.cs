using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonCapsuleController : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;
    public float cameraDistance = 4f;
    public float cameraHeight = 1.6f;
    public float mouseSensitivity = 3f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float rotationSpeed = 12f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -20f;
    public float groundedStickForce = -2f;

    private CharacterController controller;
    private float verticalVelocity;
    private float yaw;
    private float pitch = 20f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedStickForce;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 horizontalMovement = moveDirection * currentSpeed;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMovement = horizontalMovement + Vector3.up * verticalVelocity;

        controller.Move(finalMovement * Time.deltaTime);
    }

    private void UpdateCameraPosition()
    {
        if (cameraTransform == null)
        {
            return;
        }

        Vector3 pivotPosition = transform.position + Vector3.up * cameraHeight;

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 cameraOffset = cameraRotation * new Vector3(0f, 0f, -cameraDistance);

        cameraTransform.position = pivotPosition + cameraOffset;
        cameraTransform.rotation = cameraRotation;
    }
}
