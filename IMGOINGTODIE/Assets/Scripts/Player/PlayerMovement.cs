using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    [Header("Movement")]
    public float gravity = -9.81f * 2;
    public float walkSpeed;
    public float sprintSpeed;
    private float moveSpeed;
    public float doubleTapTime = 0.3f;
    private float lastTapTime = -1f;
    private bool isSprinting = false;

    [Header("Slide Settings")]
    public float slideDuration;
    public float slideSpeed;
    private bool isSliding = false;
    private float slideTimer;

    [Header("Jump Settings")]
    public float jumpHeight;
    private int jumpCount;
    public int maxJumps;

    [Header("Crouch Settings")]
    public float crouchYScale = 0.5f;
    public float crouchSpeed = 2f;
    private float originalYScale;
    private bool isCrouching = false;

    [Header("FOV Settings")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovSmoothSpeed = 10f;

    [Header("Hand Settings")]
    public Transform handTransform; // Assign in inspector (Sphere or cube)
    public Vector3 handOffset = new Vector3(1.2f, 1.7f, 2f); // right, up, forward
    private GameObject grabbedObject = null;


    [Header("Keybinds")]
    [Tooltip("Jump key")] public KeyCode jumpKey = KeyCode.Space;
    [Tooltip("Crouch key")] public KeyCode crouchKey = KeyCode.LeftShift;

    private Vector3 velocity;
    private Vector3 lastPosition;
    private bool isMoving;

    public enum MovementState { walking, sprinting, crouching, sliding, air }
    public MovementState state;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        originalYScale = transform.localScale.y;
    }

    private void Update()
    {
        HandleDoubleTapSprint();
        HandleSlideTimer();
        HandleMovementInput();
        HandleJumpInput();
        UpdateHandPosition();
        HandleGrabInput();
        HandleCrouchInput();
        ApplyGravity();
        UpdateFOV();
        stateHandler();


        // Movement check
        isMoving = lastPosition != transform.position && controller.isGrounded;
        lastPosition = transform.position;
    }

    private void HandleMovementInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }


    private void UpdateHandPosition()
    {
        Vector3 offset = playerCamera.transform.right * handOffset.x +
                         playerCamera.transform.up * handOffset.y +
                         playerCamera.transform.forward * handOffset.z;

        handTransform.position = playerCamera.transform.position + offset;
        handTransform.rotation = playerCamera.transform.rotation;
    }



    private void HandleGrabInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (grabbedObject != null)
            {
                grabbedObject.transform.parent = null;
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject = null;
            }
            else
            {
                // Try to grab
                Collider[] hits = Physics.OverlapSphere(handTransform.position, 0.5f);
                foreach (Collider hit in hits)
                {
                    if (hit.CompareTag("Grabbable"))
                    {
                        grabbedObject = hit.gameObject;
                        grabbedObject.transform.position = handTransform.position;
                        grabbedObject.transform.parent = handTransform;
                        grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                        break;
                    }
                }
            }
        }
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && jumpCount < maxJumps)
        {
            if (isCrouching)
            {
                Uncrouch(); // Automatically uncrouch before jumping
            }

            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCount++;
        }

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpCount = 0;

            if (isCrouching && state != MovementState.crouching)
            {
                Uncrouch();
            }
        }
    }

    private void HandleCrouchInput()
    {
        // Trigger slide only on crouch press while sprinting
        if (Input.GetKeyDown(crouchKey))
        {
            if (isSprinting && controller.isGrounded)
            {
                StartSlide(); // Triggers slide
            }
            else if (!isSliding) // normal crouch if not sliding
            {
                isCrouching = true;
                isSprinting = false;
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            }
        }

        // Uncrouch when crouch key is released and not sliding
        if (Input.GetKeyUp(crouchKey) && !isSliding)
        {
            Uncrouch();
        }
    }

    private void HandleDoubleTapSprint()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            float timeSinceLastTap = Time.time - lastTapTime;

            if (timeSinceLastTap <= doubleTapTime && !isCrouching)
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }

            lastTapTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            isSprinting = false;
        }

        // Override moveSpeed immediately based on crouch
        moveSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
    }

    private void HandleSlideTimer()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
                // Stay crouched only if still holding crouch key
                if (Input.GetKey(crouchKey))
                {
                    isCrouching = true;
                    transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                }
                else
                {
                    Uncrouch();
                }
            }
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateFOV()
    {
        float targetFOV = (state == MovementState.sprinting) ? sprintFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
    }

    private void StartSlide()
    {
        isSliding = true;
        isCrouching = false;
        isSprinting = false;
        slideTimer = slideDuration;
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
    }

    private void Uncrouch()
    {
        isCrouching = false;
        transform.localScale = new Vector3(transform.localScale.x, originalYScale, transform.localScale.z);
    }

    private void stateHandler()
    {
        if (!controller.isGrounded)
        {
            state = MovementState.air;
        }
        else if (isSliding)
        {
            state = MovementState.sliding;
        }
        else if (isCrouching)
        {
            state = MovementState.crouching;
        }
        else if (isSprinting)
        {
            state = MovementState.sprinting;
        }
        else
        {
            state = MovementState.walking;
        }

        switch (state)
        {
            case MovementState.walking:
                moveSpeed = walkSpeed;
                break;
            case MovementState.sprinting:
                moveSpeed = sprintSpeed;
                break;
            case MovementState.crouching:
                moveSpeed = crouchSpeed;
                break;
            case MovementState.sliding:
                moveSpeed = slideSpeed;
                break;
        }
    }
}
