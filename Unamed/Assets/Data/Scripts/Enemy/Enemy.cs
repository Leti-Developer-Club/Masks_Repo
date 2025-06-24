using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    private Transform currentTarget;
    private SignalTower shrine;
    private Rigidbody2D rb2;

    [Header("Movement")]
    public MovementType movementType;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stopDistance = 2f;

    [Header("Attack")]
    [SerializeField] private EnemyAttackConfig attackConfig;
    [SerializeField] private CombatType combatType;
    [SerializeField] private Transform attackPoint;
    private float nextDamageTime;

    [Header("Shooting")]
    [SerializeField] private Transform firePoint;
    private bool targetInFiringRange;
    private float nextFireTime;

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
        shrine = FindFirstObjectByType<SignalTower>();
        if (shrine != null)
        {
            currentTarget = shrine.transform;
        }
    }
    private void Update()
    {
        if (shrine == null || !shrine.gameObject.activeInHierarchy)
        {
            FindNew_Shrine();
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

        if (combatType != CombatType.None && Time.time >= nextFireTime)
        {
            TryShoot();
            nextFireTime = Time.time + attackConfig.fireRate;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.position);
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
                Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, stopDistance, attackConfig.meleeAttackableLayer);
                if (hits.Length > 0)
                {
                    print("attacking player");
                    hits[0].GetComponent<PlayerController>().KnockBack(transform, attackConfig.knockbackForce);
                    nextDamageTime = Time.time + attackConfig.damageInterval;
                }
            }
        }
    }
    private void FindNew_Shrine()
    {
        SignalTower[] shrines = FindObjectsByType<SignalTower>(FindObjectsSortMode.None);
        foreach (SignalTower t in shrines)
        {
            if (t.gameObject.activeInHierarchy)
            {
                shrine = t;
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
                return shrine != null ? shrine.transform : null;

            case MovementType.Closest:
                if (player == null && shrine == null) return null;
                if (player == null) return shrine.transform;
                if (shrine == null) return playerTransform;

                float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                float distToTower = Vector2.Distance(transform.position, shrine.transform.position);
                return distToPlayer < distToTower ? playerTransform : shrine.transform;

            default:
                return shrine != null ? shrine.transform : null;
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

