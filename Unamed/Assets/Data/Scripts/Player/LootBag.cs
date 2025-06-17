using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LootBag : MonoBehaviour
{
    public GameObject droppedItemPrefab;
    public List<Loot> lootList = new();

    Loot GetDroppedItem()
    {
        int randomNumber = Random.Range(1, 101); //0 will be exluded & 101 will be excluded, keeping it between 1 & 100
        List<Loot> possibleItems = new ();

        foreach (Loot item in lootList)
        {
            if(randomNumber <= item.dropChance)
            {
                possibleItems.Add(item);
            }
        }

        if(possibleItems.Count > 0)
        {
            Loot droppedItem = possibleItems[Random.Range(0, possibleItems.Count)];
            return droppedItem;
        }
        Debug.Log("No Loot!");
        return null;
    }

    public void InstantiateLoot(Vector3 spawnPos)
    {
        Loot droppedItem = GetDroppedItem();
        if (droppedItem != null)
        {
            GameObject lootGameobject = Instantiate(droppedItemPrefab, spawnPos, Quaternion.identity);
            lootGameobject.GetComponent<SpriteRenderer>().sprite = droppedItem.lootprite;
            lootGameobject.name = droppedItem.name;

            // Set the loot data on the prefab
            LootPickUp lootPickup = lootGameobject.GetComponent<LootPickUp>();
            if (lootPickup != null)
            {
                lootPickup.lootData = droppedItem;
            }

            float dropForce = 25f;
            Vector2 dropDirection = new (Random.Range(-1f, 1f), Random.Range(1f, 11f));
            lootGameobject.GetComponent<Rigidbody2D>().AddForce(dropDirection *  dropForce, ForceMode2D.Impulse);
        }
    }
}
