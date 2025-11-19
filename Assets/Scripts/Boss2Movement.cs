using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boss2Movement : MonoBehaviour
{
    public float speed = 3f;                 // скорость движения
    public Transform player;                 // ссылка на игрока
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Убедимся, что Rigidbody настроен корректно
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = true;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Движение только по плоскости XZ
        Vector3 direction = (player.position - transform.position);
        direction.y = 0;
        direction.Normalize();

        // Передвигаем Rigidbody
        Vector3 move = direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        // Поворот в сторону игрока
        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 5f * Time.fixedDeltaTime));
        }
    }
}
