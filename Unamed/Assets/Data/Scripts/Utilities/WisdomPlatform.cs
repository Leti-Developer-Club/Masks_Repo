using UnityEngine;
using UnityEngine.Events;

public class WisdomPlatform : MonoBehaviour
{
    [SerializeField] private int maxPlatformPoints = 50;
    [SerializeField] private float leechInterval = 1f;
    [SerializeField] private int pointsPerLeech = 1;
    [SerializeField] private float timeBeforeStartLeeching = 3f;

    // NEW: event invoked when platform fills up
    public UnityEvent onPlatformFull;
    public bool IsFull => currentPlatformPoints >= maxPlatformPoints;

    private int currentPlatformPoints = 0;
    private bool playerOnPlatform = false;
    private float timeOnPlatform = 0f;
    private float nextLeechTime = 0f;

    private PointsManager pointsManager;
    [SerializeField] private SignalTower tower; // assign the correct shrine in inspector
    [SerializeField, Range(0f, 1f)] private float healPercentOnFill = 0.2f;

    private void Start()
    {
        pointsManager = FindFirstObjectByType<PointsManager>();
    }

    private void Update()
    {
        if (!playerOnPlatform || pointsManager == null || currentPlatformPoints >= maxPlatformPoints)
            return;

        timeOnPlatform += Time.deltaTime;

        if (timeOnPlatform >= timeBeforeStartLeeching && Time.time >= nextLeechTime)//wait few sec before filling up
        {
            if (pointsManager.TryRemovePoints(pointsPerLeech))//remove collected points...
            {
                currentPlatformPoints += pointsPerLeech;
                nextLeechTime = Time.time + leechInterval;//...and add into platform

                if (currentPlatformPoints >= maxPlatformPoints)
                {
                    // 1) heal shrine
                    if (tower != null)
                        tower.HealPercentage(healPercentOnFill);

                    // 2) award wisdom
                    pointsManager.AddWisdomPoint();

                    // 3) signal wave manager
                    onPlatformFull?.Invoke();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = true;
            timeOnPlatform = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = false;
            timeOnPlatform = 0f;
        }
    }
}

