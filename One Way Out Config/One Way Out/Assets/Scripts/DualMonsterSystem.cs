using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MonsterType2 : MonoBehaviour
{
    [Header("Wander Settings")]
    public float normalSpeed = 3f;
    public float minWanderRadius = 10f;
    public float maxWanderRadius = 25f;
    public float wanderTimer = 3f;

    [Header("Avoidance Settings")]
    public float avoidanceRadius = 10f;
    public float avoidanceStrength = 2f;
    public LayerMask monsterLayers;

    [Header("Target Settings")]
    public Vector3 targetCoordinates;
    public float attractionStrength = 0.1f;

    [Header("Player Detection")]
    public float visionRange = 20f;
    public float closeRange = 5f; // Дистанция для мгновенной реакции
    public float visionAngle = 90f;
    public float rearVisionAngle = 30f; // Угол обзора сзади для близких расстояний
    public float chaseSpeed = 5f;
    public float memoryDuration = 5f;
    public float searchRadius = 15f;
    public float pathUpdateInterval = 0.5f; // Частота обновления пути при погоне

    [Header("Sound Settings")]
    public AudioClip touchSound;

    private NavMeshAgent agent;
    private float timer;
    private float pathUpdateTimer;
    private AudioSource audioSource;
    private const int COLLIDER_CACHE_SIZE = 10;
    private Collider[] nearbyCollidersCache = new Collider[COLLIDER_CACHE_SIZE];
    private Transform player;
    private Vector3 lastKnownPlayerPosition;
    private float memoryTimer;
    private bool isChasing;
    private List<Vector3> visitedPositions = new List<Vector3>();
    private float revisitThreshold = 5f;
    private Vector3 lastWanderPosition;
    private float stuckTimer;
    private float maxStuckTime = 2f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = normalSpeed;
        timer = wanderTimer;
        pathUpdateTimer = pathUpdateInterval;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        memoryTimer = 0f;
        isChasing = false;
        lastWanderPosition = transform.position;
        stuckTimer = 0f;
    }

    void Update()
    {
        if (player == null) return;

        bool canSeePlayer = CanSeePlayer();

        // Мгновенная реакция если игрок очень близко, даже сзади
        if (!canSeePlayer && Vector3.Distance(transform.position, player.position) <= closeRange)
        {
            canSeePlayer = CanSeePlayerRear();
        }

        if (canSeePlayer)
        {
            lastKnownPlayerPosition = player.position;
            memoryTimer = memoryDuration;
            isChasing = true;
            stuckTimer = 0f; // Сброс таймера застревания при обнаружении игрока
        }
        else if (memoryTimer > 0f)
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer <= 0f)
            {
                isChasing = false;
            }
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }

        AvoidOtherMonsters();
        MoveTowardsTarget();
        RememberVisitedLocations();
        CheckIfStuck();
    }

    void Wander()
    {
        agent.speed = normalSpeed;
        timer -= Time.deltaTime;

        if (timer <= 0f || Vector3.Distance(transform.position, lastWanderPosition) < 0.5f)
        {
            Vector3 newPos;
            if (memoryTimer > 0f)
            {
                // Ищем игрока в последнем известном месте
                newPos = GetSearchPositionAround(lastKnownPlayerPosition);
            }
            else
            {
                // Обычное блуждание, избегая посещённых мест и выбирая дальние точки
                newPos = GetStrategicWanderPosition();
            }

            if (agent.isOnNavMesh)
            {
                agent.SetDestination(newPos);
                lastWanderPosition = newPos;
            }
            timer = wanderTimer;
        }
    }

    void ChasePlayer()
    {
        agent.speed = chaseSpeed;
        pathUpdateTimer -= Time.deltaTime;

        // Частое обновление пути при погоне
        if (pathUpdateTimer <= 0f && agent.isOnNavMesh)
        {
            agent.SetDestination(lastKnownPlayerPosition);
            pathUpdateTimer = pathUpdateInterval;
        }
    }

    bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > visionRange)
        {
            return false;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Основной угол обзора спереди
        if (angleToPlayer > visionAngle / 2)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionRange))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    bool CanSeePlayerRear()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > closeRange)
        {
            return false;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Угол обзора сзади для близких расстояний
        if (angleToPlayer < 180 - rearVisionAngle / 2)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, closeRange))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    void AvoidOtherMonsters()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(
            transform.position,
            avoidanceRadius,
            nearbyCollidersCache,
            monsterLayers
        );

        if (numColliders == 0) return;

        Vector3 avoidanceDirection = Vector3.zero;
        int monstersToAvoid = 0;

        for (int i = 0; i < numColliders; i++)
        {
            Collider monster = nearbyCollidersCache[i];
            if (monster.gameObject == gameObject) continue;

            Vector3 dirAwayFromMonster =
                (transform.position - monster.transform.position).normalized;

            float distance = Vector3.Distance(transform.position, monster.transform.position);
            float weight = 1 - (distance / avoidanceRadius);

            avoidanceDirection += dirAwayFromMonster * weight;
            monstersToAvoid++;
        }

        if (monstersToAvoid > 0)
        {
            avoidanceDirection /= monstersToAvoid;
            Vector3 newDirection = agent.velocity.normalized +
                                 avoidanceDirection * avoidanceStrength;
            agent.velocity = newDirection * agent.speed;
        }
    }

    void MoveTowardsTarget()
    {
        if (!isChasing)
        {
            Vector3 direction = (targetCoordinates - transform.position).normalized;
            if (agent.isOnNavMesh)
            {
                agent.destination += direction * attractionStrength * Time.deltaTime;
            }
        }
    }

    Vector3 GetRandomNavPosition(float radius)
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * radius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    Vector3 GetSearchPositionAround(Vector3 center)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * searchRadius;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, searchRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return center;
    }

    Vector3 GetStrategicWanderPosition()
    {
        Vector3 newPosition;
        int attempts = 0;
        float maxDistance = 0f;
        Vector3 bestPosition = transform.position;

        // Пробуем найти несколько позиций и выбираем самую далекую от посещенных
        do
        {
            float radius = Random.Range(minWanderRadius, maxWanderRadius);
            newPosition = GetRandomNavPosition(radius);
            
            // Оцениваем позицию по удаленности от посещенных мест
            float minDistance = float.MaxValue;
            foreach (var visitedPos in visitedPositions)
            {
                float dist = Vector3.Distance(newPosition, visitedPos);
                if (dist < minDistance) minDistance = dist;
            }
            
            if (minDistance > maxDistance)
            {
                maxDistance = minDistance;
                bestPosition = newPosition;
            }
            
            attempts++;
        } while (attempts < 10);

        return bestPosition;
    }

    bool IsPositionRecentlyVisited(Vector3 position)
    {
        foreach (var visitedPos in visitedPositions)
        {
            if (Vector3.Distance(position, visitedPos) < revisitThreshold)
            {
                return true;
            }
        }
        return false;
    }

    void RememberVisitedLocations()
    {
        if (visitedPositions.Count > 20)
        {
            visitedPositions.RemoveAt(0);
        }
        visitedPositions.Add(transform.position);
    }

    void CheckIfStuck()
    {
        if (agent.velocity.magnitude < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > maxStuckTime)
            {
                // Пытаемся выбраться из застревания
                Vector3 randomDirection = Random.insideUnitSphere * 5f;
                randomDirection += transform.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 5f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && touchSound)
        {
            audioSource.PlayOneShot(touchSound);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация зоны зрения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Визуализация ближней зоны
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeRange);

        // Визуализация последнего известного положения игрока
        if (memoryTimer > 0f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastKnownPlayerPosition, 1f);
            Gizmos.DrawWireSphere(lastKnownPlayerPosition, searchRadius);
        }
    }
}