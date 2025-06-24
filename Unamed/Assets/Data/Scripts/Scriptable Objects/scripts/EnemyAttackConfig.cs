using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack Config")]
public class EnemyAttackConfig : ScriptableObject
{
    [Header("Shooting Attack")]
    public float fireRate = 1f;
    public float shootingRange = 5f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;

    [Header("Melee Attack")]
    public LayerMask meleeAttackableLayer;
    public float damageInterval = 1f;
    public float knockbackForce = 5f;
    public float damage = 10f;
}
