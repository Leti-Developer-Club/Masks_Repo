using Unity.VisualScripting;
using UnityEngine;

public class ShootingAbility : MonoBehaviour, IAbility
{
    public string AbilityName => "Shoot";
    [Header("Public References")]
    [SerializeField] private Transform playerGun;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject circleSprite;
    [SerializeField] private LayerMask attackableLayer;

    [Header("Shooting Stuff")]
    [SerializeField] private float gunOffsetDistance = 1f;
    [SerializeField] private float circleRangeOffset = 2f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float lockOnRange = 10f;
    [SerializeField] private float fireRate = 0.5f;
    public bool canShoot = false;
    private float nextFireTime;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lockOnRange);
        circleSprite.transform.localScale = circleRangeOffset * lockOnRange * Vector3.one;
    }

    public void Use()
    {
        Shoot_ClosestEnemy();
    }
    private void Update()
    {
        Shoot_ClosestEnemy();
    }
    Transform GetClosestEnemy(Collider2D[] hits)
    {
        float minDist = Mathf.Infinity;
        Transform closest = null;
        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = hit.transform;
            }
        }
        return closest;
    }
    void FireBullet(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, playerGun.position, playerGun.rotation);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.linearVelocity = direction * bulletSpeed;
    }
    void Shoot_ClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, lockOnRange, attackableLayer);
        if (hits.Length == 0) return;

        Transform closest = GetClosestEnemy(hits);
        if (!closest) return;

        Vector2 targetDirection = (closest.position - transform.position).normalized;// direction from player to enemy
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;//angle for rotation

        // Smoothly rotate the gun toward the enemy
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        playerGun.rotation = Quaternion.Lerp(playerGun.rotation, targetRotation, Time.deltaTime * 10f);
        playerGun.position = (Vector2)transform.position + targetDirection * gunOffsetDistance;//Offset the gun position from the player (in facing direction)

        if (Time.time >= nextFireTime && canShoot)
        {
            FireBullet(targetDirection);
            nextFireTime = Time.time + fireRate;
        }
    }
}
