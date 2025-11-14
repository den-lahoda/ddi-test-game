using UnityEngine;

public class ChasingBoss : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float spawnDistance = 10f; // от игрока
    [SerializeField] private float moveSpeed = 3f;      // скорость преследования
    [SerializeField] private float rotateSpeed = 5f;    // плавное вращение к игроку

    private bool isActive = false;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("ChasingBoss: Player not assigned!");
            return;
        }

        // Спавним на случайной позиции от игрока
        Vector2 randomCircle = Random.insideUnitCircle.normalized * spawnDistance;
        Vector3 spawnPos = player.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        transform.position = spawnPos;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        // Плавное вращение к игроку
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        // Двигаемся к игроку
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

}
