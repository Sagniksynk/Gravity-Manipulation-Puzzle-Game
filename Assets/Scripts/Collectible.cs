using UnityEngine;

public class Collectible : MonoBehaviour
{ 

    void OnTriggerEnter(Collider other)
    {
        // Robust check: Look for the Player tag or the component specifically
        if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
        { 
            GameManager.Instance.CollectCube();
            Destroy(gameObject);
        }
    }
}