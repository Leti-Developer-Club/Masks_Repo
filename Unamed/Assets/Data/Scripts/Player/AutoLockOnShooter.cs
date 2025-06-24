using System.Collections.Generic;
using UnityEngine;

public class AutoLockOnShooter : MonoBehaviour
{
    [Header("Public References")]
    [SerializeField] private Transform playerGun;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject circleSprite;
    [SerializeField] private LayerMask attackableLayer;

    [Header("Private References")]
    private Animator anim;
    private RaycastHit2D[] hits;

    [Header("Shooting Stuff")]
    [SerializeField] private float gunOffsetDistance = 1f;
    [SerializeField] private float bulletSpeed = 10f; 
    [SerializeField] private float lockOnRange = 10f;
    [SerializeField] private float fireRate = 0.5f;
    public bool canShoot = false;
    private float nextFireTime;

    [Header("Melee Melee_Attack")]
    [SerializeField] private float timeSinceAttack = 0.0f;
    [SerializeField] private float timeBtwAttacks = 0.25f;
    [SerializeField] private List<GameObject> hitVFXList;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float damageAmount = 1f;

    private int currentAttack = 0;
    private int currentVFXIndex = 0;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        timeSinceAttack = timeBtwAttacks;
    }
    private void Update()
    {
        timeSinceAttack += Time.deltaTime;
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

    public void Melee_Attack()
    {
        //deal damage
        if(timeSinceAttack >= timeBtwAttacks)
        {
            hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, transform.right, 0f, attackableLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable i_Damageable = hits[i].collider.gameObject.GetComponent<IDamageable>();

                if (hitVFXList.Count > 0)
                {
                    GameObject instantiatedVFX = Instantiate(hitVFXList[currentVFXIndex], hits[i].transform.position, Quaternion.identity);
                    Destroy(instantiatedVFX, 0.5f);

                    // Cycle to the next VFX in the list
                    currentVFXIndex = (currentVFXIndex + 1) % hitVFXList.Count;
                }
                i_Damageable?.TakeDamage(damageAmount);
            }

            //combo & animations
            currentAttack++;
            // Loop back to one after third attack
            if (currentAttack > 3)
                currentAttack = 1;

            // Reset Melee_Attack combo if time since last attack is too large
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            anim.SetTrigger("Melee_Attack" + currentAttack);

            // Reset timer
            timeSinceAttack = 0.0f;
        }
    }

}

