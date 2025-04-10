using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float mouseSensitivity = 2f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Transform cameraTransform;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isGrounded;

    private Animation anim;

    private float shootAnimCooldown = 0.5f;
    private float lastShootTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Lock mouse to screen

        // Get the Animation component from PlayerModel (child of Player)
        anim = GetComponentInChildren<Animation>();
        if (anim != null)
        {
            anim.Play("idle");
        }
    }

    void Update()
    {
        // Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep grounded
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (anim != null)
            {
                anim.CrossFade("jump");
            }
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animation: Idle / Walk when grounded
        if (anim != null && isGrounded)
        {
            if (move.magnitude > 0.1f)
            {
                if (!anim.IsPlaying("walk"))
                {
                    anim.CrossFade("walk");
                }
            }
            else
            {
                if (!anim.IsPlaying("idle"))
                {
                    anim.CrossFade("idle");
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && isGrounded && anim != null && anim.GetClip("holding-right") != null)
        {
            if (Time.time - lastShootTime >= shootAnimCooldown)
            {
                anim.CrossFade("holding-right");
                lastShootTime = Time.time;
            }
        }


    }

    // Optional: Show ground check sphere in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
