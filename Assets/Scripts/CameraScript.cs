using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2.5f, -4f);
    public float smoothSpeed = 0.125f;

    void FixedUpdate()
    {
        if (!target) return;

        // Position: Relative to player's rotation
        Vector3 desiredPos = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);

        // Rotation: Look at player, align Up with player Up
        Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position, target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothSpeed);
    }
}