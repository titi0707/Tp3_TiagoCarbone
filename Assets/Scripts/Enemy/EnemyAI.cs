using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    enum State { Patrol, Chase, Attack }

    [SerializeField] EnemyData data;
    [SerializeField] Vector3[] patrolPoints;
    [SerializeField] LayerMask visionMask = ~0;

    NavMeshAgent agent;
    Transform player;
    Health playerHealth;

    State state = State.Patrol;
    int currentPoint;
    float waitTimer;
    float lastSeenTime = -999f;
    Vector3 lastKnownPosition;
    float nextAttackTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        Health health = GetComponent<Health>();
        if (health != null) health.Initialize(data.maxHealth);
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<Health>();
        }

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0]);
    }

    void Update()
    {
        if (player == null) return;

        bool playerAlive = playerHealth == null || !playerHealth.IsDead;
        bool canSee = playerAlive && CanSeePlayer();

        if (canSee)
        {
            lastSeenTime = Time.time;
            lastKnownPosition = player.position;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (canSee) state = State.Chase;
                break;

            case State.Chase:
                Chase();
                if (!playerAlive || Time.time - lastSeenTime > data.loseSightTime) ReturnToPatrol();
                else if (canSee && distance <= data.attackRange) state = State.Attack;
                break;

            case State.Attack:
                Attack();
                if (!playerAlive) ReturnToPatrol();
                else if (!canSee || distance > data.attackRange) state = State.Chase;
                break;
        }
    }

    void Patrol()
    {
        agent.speed = data.patrolSpeed;
        if (patrolPoints.Length == 0) return;

        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.1f) return;

        waitTimer += Time.deltaTime;
        if (waitTimer >= data.waitTimeAtPoint)
        {
            waitTimer = 0f;
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPoint]);
        }
    }

    void Chase()
    {
        agent.isStopped = false;
        agent.speed = data.chaseSpeed;
        agent.SetDestination(lastKnownPosition);
    }

    void Attack()
    {
        agent.isStopped = true;
        FaceTarget(player.position);

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + data.attackCooldown;
            if (playerHealth != null) playerHealth.TakeDamage(data.attackDamage);
        }
    }

    void ReturnToPatrol()
    {
        state = State.Patrol;
        agent.isStopped = false;
        waitTimer = 0f;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPoint]);
    }

    bool CanSeePlayer()
    {
        Vector3 eyes = transform.position + Vector3.up * data.eyeHeight;
        Vector3 toPlayer = player.position - eyes;

        if (toPlayer.magnitude > data.detectionRange) return false;

        Vector3 flatDirection = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (Vector3.Angle(transform.forward, flatDirection) > data.viewAngle * 0.5f) return false;

        if (Physics.Raycast(eyes, toPlayer.normalized, out RaycastHit hit, data.detectionRange, visionMask, QueryTriggerInteraction.Ignore))
            return hit.transform == player || hit.transform.IsChildOf(player);

        return false;
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (data != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, data.detectionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, data.attackRange);
        }

        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            Gizmos.DrawSphere(patrolPoints[i], 0.3f);
            Vector3 next = patrolPoints[(i + 1) % patrolPoints.Length];
            Gizmos.DrawLine(patrolPoints[i], next);
        }
    }
}