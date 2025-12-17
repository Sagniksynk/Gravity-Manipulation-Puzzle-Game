using UnityEngine;
using System.Collections;

public class GravityManager : MonoBehaviour
{
    [Header("Settings")]
    public float gravityForce = 9.81f;
    public float rotationSpeed = 5f;
    public float hologramSmoothSpeed = 15f;

    [Tooltip("The Pivot Point. 1.6 is ideal for vaulting from the head.")]
    public float headHeight = 1.6f;

    [Header("References")]
    public GameObject hologramPrefab;

    private Rigidbody rb;
    private GameObject currentHologram;
    private Vector3 currentGravityDir;

    // State for Hologram Smooth Movement
    private Vector3 targetHoloPos;
    private Quaternion targetHoloRot;
    private bool isHologramActive = false;
    private bool isSwitchingGravity = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        // Use 'velocity' for older Unity, 'linearVelocity' for Unity 6+
        rb.linearVelocity = Vector3.zero;

        currentGravityDir = -transform.up;

        if (hologramPrefab != null)
        {
            currentHologram = Instantiate(hologramPrefab, transform.position, transform.rotation);
            currentHologram.transform.SetParent(transform);

            // Clean up
            Destroy(currentHologram.GetComponent<Rigidbody>());
            Destroy(currentHologram.GetComponent<Collider>());
            Destroy(currentHologram.GetComponent<Animator>());
            Destroy(currentHologram.GetComponent<PlayerController>());

            currentHologram.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(currentGravityDir * gravityForce, ForceMode.Acceleration);
    }

    void Update()
    {
        if (isSwitchingGravity || currentHologram == null) return;

        bool holdingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // --- INPUTS ---
        if (holdingShift)
        {
            // Ceiling / Floor
            if (Input.GetKeyDown(KeyCode.UpArrow)) SetHologramTarget(180, Vector3.right);
            if (Input.GetKeyDown(KeyCode.DownArrow)) SetHologramTarget(0, Vector3.right);
        }
        else
        {
            // Walls
            if (Input.GetKeyDown(KeyCode.UpArrow)) SetHologramTarget(-90, Vector3.right);   // Front
            if (Input.GetKeyDown(KeyCode.DownArrow)) SetHologramTarget(90, Vector3.right);    // Back
            if (Input.GetKeyDown(KeyCode.LeftArrow)) SetHologramTarget(-90, Vector3.forward); // Left
            if (Input.GetKeyDown(KeyCode.RightArrow)) SetHologramTarget(90, Vector3.forward);  // Right
        }

        // --- SMOOTH HOLOGRAM MOVEMENT ---
        if (isHologramActive)
        {
            currentHologram.transform.position = Vector3.Lerp(currentHologram.transform.position, targetHoloPos, Time.deltaTime * hologramSmoothSpeed);
            currentHologram.transform.rotation = Quaternion.Slerp(currentHologram.transform.rotation, targetHoloRot, Time.deltaTime * hologramSmoothSpeed);
        }

        // --- EXECUTE ---
        if (Input.GetKeyDown(KeyCode.Return) && isHologramActive)
        {
            StartCoroutine(SwitchGravitySequence());
        }
    }

    void SetHologramTarget(float angle, Vector3 axis)
    {
        if (!isHologramActive)
        {
            currentHologram.SetActive(true);
            isHologramActive = true;
            // Snap to start
            currentHologram.transform.position = transform.position;
            currentHologram.transform.rotation = transform.rotation;
        }

        // Calculate Target
        Vector3 headPivot = transform.position + (transform.up * headHeight);
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Quaternion rotationOffset = Quaternion.AngleAxis(angle, transform.TransformDirection(axis));
        Quaternion finalRot = rotationOffset * startRot;

        Vector3 dirFromPivot = startPos - headPivot;
        Vector3 finalPos = headPivot + (rotationOffset * dirFromPivot);

        if (Mathf.Abs(angle) < 0.01f) // Reset/Floor case
        {
            targetHoloPos = transform.position;
            targetHoloRot = transform.rotation;
        }
        else
        {
            targetHoloPos = finalPos;
            targetHoloRot = finalRot;
        }
    }

    IEnumerator SwitchGravitySequence()
    {
        isSwitchingGravity = true;
        isHologramActive = false;
        currentHologram.SetActive(false);

        Quaternion startRot = transform.rotation;
        Quaternion endRot = targetHoloRot;
        Vector3 startPos = transform.position;
        Vector3 endPos = targetHoloPos;

        float duration = Quaternion.Angle(startRot, endRot) > 100 ? 0.8f : 0.5f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t);

            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        // --- FIX STARTS HERE ---

        // 1. Force position to end point
        transform.position = endPos;

        // 2. Calculate approximate new vectors based on rotation
        // FIX: Replaced "-endRot * Vector3.up" with "endRot * Vector3.down"
        Vector3 approximateGravity = endRot * Vector3.down;
        Vector3 approximateForward = endRot * Vector3.forward;

        // 3. SNAP GRAVITY to the nearest perfect World Axis (0,1,0), (1,0,0), etc.
        currentGravityDir = SnapToNearestAxis(approximateGravity);

        // 4. SNAP FORWARD to nearest World Axis (prevents slightly skewed looking direction)
        Vector3 snappedForward = SnapToNearestAxis(approximateForward);

        // 5. Force Player Rotation to align perfectly with these snapped vectors
        // This ensures the camera (which tracks Up) will be perfectly level.
        if (currentGravityDir != Vector3.zero && snappedForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(snappedForward, -currentGravityDir);
        }

        // Reset Physics Velocity
        rb.linearVelocity = Vector3.zero;

        isSwitchingGravity = false;
    }

    // Helper to find the closest perfect axis
    Vector3 SnapToNearestAxis(Vector3 direction)
    {
        float x = Mathf.Abs(direction.x);
        float y = Mathf.Abs(direction.y);
        float z = Mathf.Abs(direction.z);

        if (x > y && x > z) return new Vector3(Mathf.Sign(direction.x), 0, 0);
        if (y > x && y > z) return new Vector3(0, Mathf.Sign(direction.y), 0);
        return new Vector3(0, 0, Mathf.Sign(direction.z));
    }
}