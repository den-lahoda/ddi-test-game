using UnityEngine;

public class BossDamage : MonoBehaviour
{
    [Header("Attack")]
    public float damagePerSecond = 10f;
    public float attackDistance = 1.4f;

    Transform player;
    PlayerHealth playerHealth;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerHealth = p.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackDistance)
        {
            playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}
