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

    //Cache Animator Hashes
    private int animSpeedHash;
    private int animGroundedHash;
    private int animJumpHash;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Calculate hashes once
        animSpeedHash = Animator.StringToHash("Speed");
        animGroundedHash = Animator.StringToHash("IsGrounded");
        animJumpHash = Animator.StringToHash("Jump");
    }

    void Update()
    {
        // 1. Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        // 2. Animations (Using Hashes)
        if (anim != null)
        {
            Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
            anim.SetFloat(animSpeedHash, Mathf.Abs(localVel.x) + Mathf.Abs(localVel.z));
            anim.SetBool(animGroundedHash, isGrounded);
        }

        // 3. Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            if (anim) anim.SetTrigger(animJumpHash);
        }
    }

    void FixedUpdate()
    {
        float x = 0;
        float z = 0;

        if (Input.GetKey(KeyCode.W)) z = 1;
        if (Input.GetKey(KeyCode.S)) z = -1;
        if (Input.GetKey(KeyCode.D)) x = 1;
        if (Input.GetKey(KeyCode.A)) x = -1;

        Vector3 camFwd = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized;
        Vector3 camRight = Vector3.ProjectOnPlane(cameraTransform.right, transform.up).normalized;

        Vector3 moveDir = (camFwd * z + camRight * x).normalized;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        Vector3 targetVel = moveDir * moveSpeed;
        Vector3 gravityComponent = Vector3.Project(rb.linearVelocity, transform.up);
        rb.linearVelocity = targetVel + gravityComponent;
    }
}