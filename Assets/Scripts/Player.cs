using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Camera playerCamera;
    public float mouseSensitivity = 2f;
    public float verticalRotationLimit = 80f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private float rotationX = 0f;
    private bool isGrounded;
    private float jumpBufferCounter;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
            Debug.LogError("Player: Assign a Camera to playerCamera!");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Обновляем буфер прыжка
        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        HandleMovement();
        HandleCamera();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0f, v);
        if (move.magnitude > 1f) move.Normalize();
        move = transform.TransformDirection(move);

        // ускорение при беге
        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= sprintMultiplier;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // проверка на землю через Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f; // небольшое отрицательное значение для устойчивости

        // прыжок с буфером
        if (jumpBufferCounter > 0f && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0f;
        }

        // гравитация
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCamera()
    {
        if (playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // горизонтальный поворот игрока
        transform.Rotate(Vector3.up * mouseX);

        // вертикальный поворот камеры
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalRotationLimit, verticalRotationLimit);
        playerCamera.transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);

        // камера на голове игрока
        playerCamera.transform.position = transform.position + Vector3.up * 1.0f;
    }

    // Визуализация Ground Check в сцене (не обязательно)
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
