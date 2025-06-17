using System.Collections;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField, Range(0, 100) ] private float damageDealt;
    [SerializeField, Range(0, 20) ] private float bulletLifetime;

    private void Update()
    {
        StartCoroutine(RemoveBullet());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemyChaseLogic = collision.gameObject.GetComponent<Enemy>();
            enemyChaseLogic.PlayerShotMe();

            var enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageDealt);

            BulletDeath();
        }
    }

    IEnumerator RemoveBullet()
    {
        yield return new WaitForSeconds(bulletLifetime);
        BulletDeath();
    }

    private void BulletDeath()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, 3f);
    }
}
