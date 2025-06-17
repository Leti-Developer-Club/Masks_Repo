using System.Collections.Generic;
using UnityEngine;

public class MeleeAbility : MonoBehaviour, IAbility
{
    public string AbilityName => "Melee";

    [Header("Melee Melee_Attack")]
    [SerializeField] private float timeSinceAttack = 0.0f;
    [SerializeField] private float timeBtwAttacks = 0.25f;
    [SerializeField] private List<GameObject> hitVFXList;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private bool isAttacking = false;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float damageAmount = 1f;

    private int currentAttack = 0;
    private int currentVFXIndex = 0;
    public Animator anim;
    private RaycastHit2D[] hits;
    [SerializeField] private LayerMask attackableLayer;

    private void Start()
    {
        timeSinceAttack = timeBtwAttacks;
    }
    void Update()
    {
        timeSinceAttack += Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }

    public void Use()
    {
        Melee_Attack();
    }
    public void Melee_Attack()
    {
        //deal damage
        if (timeSinceAttack >= timeBtwAttacks)
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
