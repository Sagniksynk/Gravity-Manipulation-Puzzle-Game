using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpForce = 8f;
    public float rotationSpeed = 10f;

    [Header("References")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform cameraTransform;

    private Rigidbody rb;
    private Animator anim;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // Auto-find camera if not assigned
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        // 2. Animations
        if (anim != null)
        {
            // Use 'velocity' for Unity 2022-, 'linearVelocity' for Unity 6
            Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
            anim.SetFloat("Speed", Mathf.Abs(localVel.x) + Mathf.Abs(localVel.z));
            anim.SetBool("IsGrounded", isGrounded);
        }

        // 3. Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            if (anim) anim.SetTrigger("Jump");
        }
    }

    void FixedUpdate()
    {
        // 4. Input - STRICTLY WASD (Ignoring Arrow Keys)
        float x = 0;
        float z = 0;

        if (Input.GetKey(KeyCode.W)) z = 1;
        if (Input.GetKey(KeyCode.S)) z = -1;
        if (Input.GetKey(KeyCode.D)) x = 1;
        if (Input.GetKey(KeyCode.A)) x = -1;

        // 5. Calculate Camera-Relative Direction
        // Project camera vectors onto the player's "ground plane" (defined by transform.up)
        Vector3 camFwd = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized;

        // Combine inputs with camera vectors
        Vector3 moveDir = (camFwd * z + camRight * x).normalized;

        // 6. Rotate Character to Face Movement
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // 7. Apply Velocity
        Vector3 targetVel = moveDir * moveSpeed;

        // Apply velocity while preserving gravity (Y-local velocity)
        Vector3 currentLocalVel = transform.InverseTransformDirection(rb.linearVelocity);

        // Only overwrite X and Z relative to the character
        Vector3 gravityComponent = Vector3.Project(rb.linearVelocity, transform.up);
        rb.linearVelocity = targetVel + gravityComponent;
    }
}