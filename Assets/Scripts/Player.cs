using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Camera playerCamera;               // ссылка на камеру игрока
    public float mouseSensitivity = 2f;       // чувствительность мыши
    public float verticalRotationLimit = 80f; // ограничение взгляда вверх/вниз

    private CharacterController controller;
    private Vector3 velocity;
    private float rotationX = 0f; // вертикальное вращение камеры

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (playerCamera == null)
        {
            Debug.LogError("Player: Assign a Camera to playerCamera!");
        }

        // Заблокировать курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
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

        controller.Move(move * speed * Time.deltaTime);

        // гравитация
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = 0f;
    }

    private void HandleCamera()
    {
        if (playerCamera == null) return;

        // Поворот мышью
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Вращение игрока по горизонтали
        transform.Rotate(Vector3.up * mouseX);

        // Вращение камеры по вертикали
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalRotationLimit, verticalRotationLimit);
        playerCamera.transform.localEulerAngles = new Vector3(rotationX, 0f, 0f);

        // Камера на голове игрока
        playerCamera.transform.position = transform.position + Vector3.up * 1.0f;
    }
}
