using UnityEngine;

public class ThirdPersonOrbitCam : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;             // Drag Player here
    public Vector3 targetOffset = new Vector3(0, 1.5f, 0); // Height offset (Shoulders/Head)

    [Header("Camera Settings")]
    public float distance = 5.0f;        // Default distance from player
    public float minDistance = 1.0f;     // Closest zoom
    public float maxDistance = 8.0f;     // Furthest zoom
    public float sensitivityX = 4.0f;    // Mouse X speed
    public float sensitivityY = 2.0f;    // Mouse Y speed
    public float yMinLimit = -40f;       // Look down limit
    public float yMaxLimit = 80f;        // Look up limit
    public LayerMask collisionLayers;    // What blocks the camera (e.g., Ground/Walls)

    [Header("Smoothing")]
    public float rotationSmoothTime = 0.12f;
    public float gravityAlignmentSpeed = 5f;

    // Internal State
    private float currentX = 0.0f;
    private float currentY = 20.0f; // Start looking slightly down
    private float currentDistance;
    private Vector3 currentVelocity;
    private Quaternion gravityRotation = Quaternion.identity;

    void Start()
    {
        // 1. Lock Cursor for TPP feel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentDistance = distance;

        // Initialize angles to match current camera view
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        // Initialize gravity alignment
        if (target != null) gravityRotation = Quaternion.FromToRotation(Vector3.up, target.up);
    }

    void LateUpdate()
    {
        if (!target) return;

        // 2. Input
        currentX += Input.GetAxis("Mouse X") * sensitivityX;
        currentY -= Input.GetAxis("Mouse Y") * sensitivityY;
        currentY = Mathf.Clamp(currentY, yMinLimit, yMaxLimit);

        // 3. Gravity Alignment
        // We calculate the rotation needed to align "World Up" to "Player's Up"
        Quaternion targetGravityRotation = Quaternion.FromToRotation(Vector3.up, target.up);
        gravityRotation = Quaternion.Slerp(gravityRotation, targetGravityRotation, gravityAlignmentSpeed * Time.deltaTime);

        // 4. Calculate Rotation
        // We combine Gravity Alignment + Mouse Orbit
        // IMPORTANT: We apply gravity rotation FIRST, then local mouse rotation
        Quaternion mouseRotation = Quaternion.Euler(currentY, currentX, 0);
        Quaternion finalRotation = gravityRotation * mouseRotation;

        // 5. Calculate Position & Collision
        // Raycast from Target towards Camera Position to find walls
        Vector3 desiredDistVector = new Vector3(0.0f, 0.0f, -distance);
        Vector3 targetPos = target.position + (gravityRotation * targetOffset); // Offset relative to gravity
        Vector3 desiredPos = targetPos + (finalRotation * desiredDistVector);

        // Collision Check: Raycast from player head to camera
        RaycastHit hit;
        if (Physics.Linecast(targetPos, desiredPos, out hit, collisionLayers))
        {
            // If we hit a wall, move camera to the hit point (plus a small buffer)
            currentDistance = Vector3.Distance(targetPos, hit.point) - 0.2f;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, distance);
        }
        else
        {
            // Reset to full distance slowly if space clears up
            currentDistance = Mathf.Lerp(currentDistance, distance, Time.deltaTime * 5f);
        }

        // 6. Apply Final Transform
        Vector3 finalPos = targetPos + (finalRotation * new Vector3(0, 0, -currentDistance));

        // Smoothly move and rotate
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, rotationSmoothTime); // Optional smoothing
        transform.position = finalPos;
    }
}