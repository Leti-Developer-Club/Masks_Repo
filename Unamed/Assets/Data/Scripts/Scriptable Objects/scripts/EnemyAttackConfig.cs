using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack Config")]
public class EnemyAttackConfig : ScriptableObject
{
    public float fireRate = 1f;
    public float shootingRange = 5f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
}
