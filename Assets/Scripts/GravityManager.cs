using UnityEngine;
using System.Collections;

public class GravityManager : MonoBehaviour
{
    [Header("Settings")]
    public float gravityForce = 9.81f;
    public float rotationSpeed = 5f;
    public float hologramSmoothSpeed = 15f; // New: Controls how fast the hologram moves to new spot

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
        rb.linearVelocity = Vector3.zero;

        currentGravityDir = -transform.up;

        if (hologramPrefab != null)
        {
            currentHologram = Instantiate(hologramPrefab, transform.position, transform.rotation);
            currentHologram.transform.SetParent(transform); // Initially child to keep organized

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
            if (Input.GetKeyDown(KeyCode.LeftArrow)) SetHologramTarget(-90, Vector3.forward); // Left (Fixed)
            if (Input.GetKeyDown(KeyCode.RightArrow)) SetHologramTarget(90, Vector3.forward);  // Right (Fixed)
        }

        // --- SMOOTH HOLOGRAM MOVEMENT ---
        if (isHologramActive)
        {
            // Smoothly move hologram to the calculated target
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
        // 1. Activate if not already
        if (!isHologramActive)
        {
            currentHologram.SetActive(true);
            isHologramActive = true;

            // If just starting, snap to player so we don't lerp from 0,0,0
            currentHologram.transform.position = transform.position;
            currentHologram.transform.rotation = transform.rotation;
        }

        // 2. Calculate where the hologram SHOULD be
        // We create a temporary dummy calculation to find the target position/rotation
        // without actually moving the object yet.

        Vector3 headPivot = transform.position + (transform.up * headHeight);

        // Start from player's current exact spot
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        // Calculate rotation
        Quaternion rotationOffset = Quaternion.AngleAxis(angle, transform.TransformDirection(axis));
        Quaternion finalRot = rotationOffset * startRot;

        // Calculate position (Rotate position around pivot)
        // Math: NewPos = Pivot + Rotation * (StartPos - Pivot)
        Vector3 dirFromPivot = startPos - headPivot;
        Vector3 finalPos = headPivot + (rotationOffset * dirFromPivot);

        // 3. Set Targets
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

        // We transition to where the Hologram WAS aimed (Target), 
        // not necessarily where it visually "is" if the lerp wasn't finished.
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

        transform.rotation = endRot;
        transform.position = endPos;
        currentGravityDir = -transform.up;

        rb.linearVelocity = Vector3.zero;

        isSwitchingGravity = false;
    }
}