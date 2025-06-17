using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance { get; private set; }

    [SerializeField] private TMP_Text wisdomPointsTxt;
    [SerializeField] private TMP_Text killCountTxt;
    [SerializeField] private TMP_Text pointsTxt;

    private int wisdomPoints;
    private int killCount;
    private int points;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        pointsTxt.text = "Points: " + points;
        killCountTxt.text = "KillCount: " + killCount;
        wisdomPointsTxt.text = "Wisdom: " + wisdomPoints;
    }

    public void AddPoints(int amount)
    {
        points += amount;
    }
    public void KillCount(int kill)
    {
        killCount += kill;
    }

    public bool TryRemovePoints(int amount)
    {
        if (points >= amount)
        {
            points -= amount;
            return true;
        }
        return false;
    }

    public void AddWisdomPoint()
    {
        wisdomPoints++;
    }
}

