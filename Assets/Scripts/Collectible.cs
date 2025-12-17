using UnityEngine;
public class Collectible : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the Player
        // We check the root because the collider might be on a child object
        if (other.transform.root.CompareTag("Player"))
        {
            GameManager.Instance.CollectCube();
            Destroy(gameObject); // Remove the cube
        }
    }
}