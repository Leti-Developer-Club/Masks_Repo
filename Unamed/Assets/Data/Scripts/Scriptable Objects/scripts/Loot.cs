using UnityEngine;

[CreateAssetMenu]
public class Loot : ScriptableObject
{
    public Sprite lootprite;
    public string lootName;
    public int dropChance;
    public int points;

    public Loot(string lootName, int dropChance, int points)
    {
        this.lootName = lootName;
        this.dropChance = dropChance;
        this.points = points;
    }
}
