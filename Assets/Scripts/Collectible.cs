using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Static counter to keep track of ALL collectibles simply
    private static int globalCollectedCount = 0;
    private static int globalTotalCount = 0;

    void Awake()
    {
        globalTotalCount++;
    }

    void OnDestroy()
    {
        // Cleanup if scene reloads or object destroyed properly
        globalTotalCount--;
    }

    // Reset statics when scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetStatics()
    {
        globalCollectedCount = 0;
        globalTotalCount = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
        {
            globalCollectedCount++;

            // Trigger Event"
            GameEvents.TriggerCubeCollected(globalCollectedCount, globalTotalCount);

            // Disable collider immediately so it can't be triggered twice before Destroy
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject);
        }
    }
}