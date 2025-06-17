using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    private Transform currentTarget;
    private SignalTower tower;
    private Rigidbody2D rb2;

    [Header("Movement")]
    public MovementType movementType;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 2f;

    [Header("Attack")]
    [SerializeField] private EnemyAttackConfig attackConfig;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float damage = 10f;
    private float nextDamageTime;
    public CombatType combatType;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float shootingRange;
    private float nextFireTime;
    private bool targetInFiringRange;

    public enum MovementType
    {
        ToTower,
        ToPlayer,
        Closest
    }
    public enum CombatType
    {
        None,
        ShootAtTower,
        ShootAtPlayer,
        ShootClosest
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackConfig.shootingRange);
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }

    private void Awake()
    {
        rb2 = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<PlayerController>();
    }

    private void Start()
    {
        tower = FindFirstObjectByType<SignalTower>();
        if (tower != null)
        {
            currentTarget = tower.transform;
        }
    }

    private void Update()
    {
        if (tower == null || !tower.gameObject.activeInHierarchy)
        {
            FindNewTower();
        }

        HandleCombat();
    }

    private void HandleCombat()
    {
        currentTarget = GetTargetBasedOnType();

        if (currentTarget == null)
        {
            rb2.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (combatType != CombatType.None && Time.time >= nextFireTime)
        {
            TryShoot();
            nextFireTime = Time.time + attackConfig.fireRate;
        }

        if (distance > stopDistance && !targetInFiringRange)
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            rb2.linearVelocity = direction * moveSpeed;
        }
        else
        {
            rb2.linearVelocity = Vector2.zero;

            if (Time.time >= nextDamageTime && !targetInFiringRange)
            {
                if (currentTarget.CompareTag("SignalTower") && tower != null)
                {
                    tower.TakeDamage(damage);
                    nextDamageTime = Time.time + damageInterval;
                }
            }
        }
    }
    private void FindNewTower()
    {
        SignalTower[] towers = FindObjectsByType<SignalTower>(FindObjectsSortMode.None);
        foreach (SignalTower t in towers)
        {
            if (t.gameObject.activeInHierarchy)
            {
                tower = t;
                break;
            }
        }
    }
    void TryShoot()
    {
        if (Time.time < nextFireTime || attackConfig == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
        targetInFiringRange = distanceToTarget <= attackConfig.shootingRange;

        switch (combatType)
        {
            case CombatType.ShootAtTower:
                if (currentTarget.CompareTag("SignalTower") && targetInFiringRange)
                    FireProjectileAt(currentTarget);
                break;

            case CombatType.ShootAtPlayer:
                if (currentTarget.CompareTag("Player") && targetInFiringRange)
                    FireProjectileAt(currentTarget);
                break;

            case CombatType.ShootClosest:
                if (targetInFiringRange)
                    FireProjectileAt(currentTarget);
                break;
        }
    }

    Transform GetTargetBasedOnType()
    {
        Transform playerTransform = player.transform;

        switch (movementType)
        {
            case MovementType.ToPlayer:
                return playerTransform;

            case MovementType.ToTower:
                return tower != null ? tower.transform : null;

            case MovementType.Closest:
                if (player == null && tower == null) return null;
                if (player == null) return tower.transform;
                if (tower == null) return playerTransform;

                float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                float distToTower = Vector2.Distance(transform.position, tower.transform.position);
                return distToPlayer < distToTower ? playerTransform : tower.transform;

            default:
                return tower != null ? tower.transform : null;
        }
    }
    private void FireProjectileAt(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;

        GameObject projectile = Instantiate(attackConfig.projectilePrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * attackConfig.projectileSpeed;

        nextFireTime = Time.time + attackConfig.fireRate;
    }

    public void PlayerShotMe()
    {
        if(movementType == MovementType.ToTower && combatType == CombatType.ShootAtTower)
        {
            movementType = MovementType.ToPlayer;
            combatType = CombatType.ShootAtPlayer;
        }

        else if(movementType == MovementType.ToTower && combatType == CombatType.None)
        {
            movementType = MovementType.ToPlayer;
        }
    }
}

