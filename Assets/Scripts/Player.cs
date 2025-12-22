using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]

    [Tooltip("Базовая скорость передвижения игрока")]
    public float speed = 5f;

    [Tooltip("Множитель скорости при зажатом Shift")]
    public float sprintMultiplier = 1.5f;

    [Tooltip("Высота прыжка в юнитах Unity")]
    public float jumpHeight = 2f;

    [Tooltip("Сила гравитации (должна быть отрицательной)")]
    public float gravity = -9.81f;


    [Header("Camera Settings")]

    [Tooltip("Камера игрока (обязательно назначить)")]
    public Camera playerCamera;

    [Tooltip("Чувствительность мыши")]
    public float mouseSensitivity = 2f;

    [Tooltip("Максимальный угол поворота камеры вверх и вниз")]
    public float verticalRotationLimit = 80f;


    [Header("Ground Check")]

    [Tooltip("Точка проверки касания земли (обычно у ног игрока)")]
    public Transform groundCheck;

    [Tooltip("Радиус проверки земли")]
    public float groundDistance = 0.2f;

    [Tooltip("Слои, которые считаются землёй")]
    public LayerMask groundMask;


    [Header("Jump Buffer")]

    [Tooltip("Время, в течение которого прыжок запоминается")]
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
        // Буфер прыжка
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

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= sprintMultiplier;

        controller.Move(move * currentSpeed * Time.deltaTime);

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        if (jumpBufferCounter > 0f && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferCounter = 0f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCamera()
    {
        if (playerCamera == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalRotationLimit, verticalRotationLimit);

        playerCamera.transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);
        playerCamera.transform.position = transform.position + Vector3.up * 1.0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
