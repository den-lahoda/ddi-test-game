using UnityEngine;
using UnityEngine.AI;

public class Boss2Movement : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Distances")]
    public float viewDistance = 10f;
    public float loseDistance = 15f;
    public float attackDistance = 1.4f;

    [Header("Search")]
    public float searchRadius = 8f;
    public float searchTime = 6f;

    [Header("Patrol")]
    public float patrolRadius = 12f;

    Vector3 patrolPoint;
    Vector3 lastKnownPlayerPos;
    bool hasPatrolPoint;

    float searchTimer;

    enum State { Patrol, Chase, Search }
    State state = State.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // ВАЖНО
        agent.stoppingDistance = 0f;
        agent.isStopped = false;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                Patrol();

                if (dist <= viewDistance && CanSeePlayer())
                {
                    state = State.Chase;
                }
                break;

            case State.Chase:
                lastKnownPlayerPos = player.position;

                if (dist > attackDistance)
                {
                    agent.isStopped = false;
                    agent.SetDestination(player.position);
                }
                else
                {
                    agent.isStopped = true; // ⬅ ПОЛНАЯ ОСТАНОВКА
                }

                if (dist > loseDistance)
                {
                    searchTimer = searchTime;
                    agent.isStopped = false;
                    agent.SetDestination(lastKnownPlayerPos);
                    state = State.Search;
                }
                break;

            case State.Search:
                Search();

                if (dist <= viewDistance && CanSeePlayer())
                {
                    state = State.Chase;
                }
                break;
        }
    }

    // ================= STATES =================

    void Patrol()
    {
        agent.isStopped = false;

        if (!hasPatrolPoint)
        {
            if (TryGetRandomNavMeshPoint(transform.position, patrolRadius, out patrolPoint))
            {
                hasPatrolPoint = true;
                agent.SetDestination(patrolPoint);
            }
        }

        if (hasPatrolPoint && agent.remainingDistance < 1f)
        {
            hasPatrolPoint = false;
        }
    }

    void Search()
    {
        agent.isStopped = false;
        searchTimer -= Time.deltaTime;

        if (agent.remainingDistance < 1f)
        {
            if (TryGetRandomNavMeshPoint(lastKnownPlayerPos, searchRadius, out Vector3 searchPoint))
            {
                agent.SetDestination(searchPoint);
            }
        }

        if (searchTimer <= 0f)
        {
            hasPatrolPoint = false;
            state = State.Patrol;
        }
    }

    // ================= HELPERS =================

    bool TryGetRandomNavMeshPoint(Vector3 center, float radius, out Vector3 result)
    {
        Vector3 randomPos = center + Random.insideUnitSphere * radius;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, radius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = center;
        return false;
    }

    bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 dir = (player.position - origin).normalized;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, viewDistance))
        {
            return hit.transform.CompareTag("Player");
        }

        return false;
    }
}
