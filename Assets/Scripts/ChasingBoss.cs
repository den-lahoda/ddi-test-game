using UnityEngine;

public class ChasingBoss : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float spawnDistance = 10f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 5f;

    [Header("Disappear Settings")]
    [SerializeField] private float disappearTime = 10f; // время без контакта до исчезновения

    private bool isActive = false;
    private CharacterController controller;
    private float timeSinceContact = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();

        if (player == null)
        {
            Debug.LogError("ChasingBoss: Player not assigned!");
            return;
        }

        // Спавн на случайной позиции от игрока
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnDistance;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        transform.position = spawnPos;

        isActive = true;
    }

    void Update()
    {
        if (!isActive || player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;

        if (distance > 0.5f)
        {
            // Босс ещё не упёрся в игрока — двигаемся
            direction.Normalize();

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
            }

            controller.Move(transform.forward * moveSpeed * Time.deltaTime);

            // Считаем время без контакта
            timeSinceContact += Time.deltaTime;
            if (timeSinceContact >= disappearTime)
            {
                // исчезаем
                gameObject.SetActive(false);
            }
        }
        else
        {
            // Босс упёрся в игрока
            timeSinceContact = 0f; // сброс таймера
        }
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }
}
