using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossAI : MonoBehaviour
{
    enum State { Idle, Move, Aim, Shoot, Reload }

    [SerializeField] BossData data;
    [SerializeField] Vector3[] coverPoints;
    [SerializeField] Transform firePoint;
    [SerializeField] BossBullet bulletPrefab;
    [SerializeField] Transform arenaCenter;
    [SerializeField] Vector2 arenaHalfSize = new Vector2(10f, 10f);
    [SerializeField] LayerMask obstacleMask = ~0;

    [SerializeField] AudioClip aimSound;
    [SerializeField] AudioClip shotSound;
    [SerializeField] AudioClip reloadSound;

    NavMeshAgent agent;
    Health health;
    Transform player;
    Health playerHealth;

    State state = State.Idle;
    int currentCover = -1;
    int shotsLeft;
    int burstLeft;
    float stateTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        if (health != null) health.Initialize(data.maxHealth);
        shotsLeft = data.cylinderSize;
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<Health>();
        }
    }

    public void StartFight()
    {
        if (state != State.Idle) return;
        GoToNextCover();
    }

    void Update()
    {
        if (state == State.Idle || player == null) return;

        if (playerHealth != null && playerHealth.IsDead)
        {
            agent.isStopped = true;
            return;
        }

        bool phase2 = health != null && health.CurrentHealth <= health.MaxHealth * 0.5f;
        float speedMultiplier = phase2 ? data.phase2SpeedMultiplier : 1f;

        switch (state)
        {
            case State.Move:
                agent.speed = data.moveSpeed * speedMultiplier;
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
                    EnterAim(speedMultiplier);
                break;

            case State.Aim:
                FaceTarget(player.position);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    state = State.Shoot;
                    burstLeft = Random.Range(data.minBurst, data.maxBurst + 1);
                    stateTimer = 0f;
                }
                break;

            case State.Shoot:
                FaceTarget(player.position);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    FireBullet(phase2);
                    shotsLeft--;
                    burstLeft--;
                    stateTimer = data.timeBetweenShots / speedMultiplier;

                    if (shotsLeft <= 0) EnterReload();
                    else if (burstLeft <= 0) GoToNextCover();
                }
                break;

            case State.Reload:
                FaceTarget(player.position);
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    shotsLeft = data.cylinderSize;
                    GoToNextCover();
                }
                break;
        }
    }

    void GoToNextCover()
    {
        state = State.Move;
        agent.isStopped = false;

        if (coverPoints.Length == 0) return;

        int next = currentCover;
        if (coverPoints.Length > 1)
            while (next == currentCover) next = Random.Range(0, coverPoints.Length);
        else
            next = 0;

        currentCover = next;
        agent.SetDestination(coverPoints[currentCover]);
    }

    void EnterAim(float speedMultiplier)
    {
        state = State.Aim;
        agent.isStopped = true;
        stateTimer = data.aimTime / speedMultiplier;
        PlaySound(aimSound);
    }

    void EnterReload()
    {
        state = State.Reload;
        agent.isStopped = true;
        stateTimer = data.reloadTime;
        PlaySound(reloadSound);
    }

    void FireBullet(bool phase2)
    {
        Vector3 origin = firePoint.position;
        Vector3 target = player.position;
        Vector3 direction = (target - origin).normalized;

        bool canSee = !IsBlocked(origin, target);
        float ricochetChance = phase2 ? data.phase2RicochetChance : data.ricochetChance;

        if (!canSee || Random.value < ricochetChance)
        {
            if (TryGetRicochetDirection(origin, target, out Vector3 ricochet))
                direction = ricochet;
        }

        BossBullet bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(direction));
        bullet.Launch(direction, data.bulletSpeed, data.bulletDamage, data.maxBounces, transform);
        PlaySound(shotSound);
    }

    bool TryGetRicochetDirection(Vector3 origin, Vector3 target, out Vector3 direction)
    {
        Vector3 c = arenaCenter.position;

        Vector3[] mirrored =
        {
            new Vector3(2f * (c.x + arenaHalfSize.x) - target.x, target.y, target.z),
            new Vector3(2f * (c.x - arenaHalfSize.x) - target.x, target.y, target.z),
            new Vector3(target.x, target.y, 2f * (c.z + arenaHalfSize.y) - target.z),
            new Vector3(target.x, target.y, 2f * (c.z - arenaHalfSize.y) - target.z)
        };

        int start = Random.Range(0, mirrored.Length);
        for (int i = 0; i < mirrored.Length; i++)
        {
            Vector3 point = mirrored[(start + i) % mirrored.Length];
            Vector3 dir = (point - origin).normalized;

            if (!Physics.Raycast(origin, dir, out RaycastHit wallHit, Vector3.Distance(origin, point), obstacleMask, QueryTriggerInteraction.Ignore))
                continue;

            Vector3 reflected = Vector3.Reflect(dir, wallHit.normal);
            Vector3 toTarget = (target - wallHit.point).normalized;
            Vector3 bounceStart = wallHit.point + wallHit.normal * 0.1f;

            if (Vector3.Dot(reflected, toTarget) > 0.98f && !IsBlocked(bounceStart, target))
            {
                direction = dir;
                return true;
            }
        }

        direction = Vector3.zero;
        return false;
    }

    bool IsBlocked(Vector3 from, Vector3 to)
    {
        if (Physics.Linecast(from, to, out RaycastHit hit, obstacleMask, QueryTriggerInteraction.Ignore))
            return hit.transform != player && !hit.transform.IsChildOf(player);

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

    void PlaySound(AudioClip clip)
    {
        if (clip != null) AudioSource.PlayClipAtPoint(clip, transform.position);
    }

    void OnDrawGizmosSelected()
    {
        if (arenaCenter != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(arenaCenter.position, new Vector3(arenaHalfSize.x * 2f, 0.1f, arenaHalfSize.y * 2f));
        }

        if (coverPoints == null) return;

        Gizmos.color = Color.cyan;
        foreach (Vector3 point in coverPoints)
            Gizmos.DrawSphere(point, 0.3f);
    }
}