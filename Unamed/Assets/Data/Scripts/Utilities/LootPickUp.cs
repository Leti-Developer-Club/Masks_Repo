using UnityEngine;

public class LootPickUp : MonoBehaviour
{
    public Loot lootData; // Assigned when instantiated

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (lootData != null)
            {
                PointsManager.Instance.AddPoints(lootData.points);
                Debug.Log($"Picked up {lootData.lootName}, +{lootData.points} points");
            }

            Destroy(gameObject);
        }
    }
}
