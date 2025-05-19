using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float crouchSpeed = 2.5f;
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float jumpForce = 7f;
    public float gravity = 20f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 100f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    private Animator animator;
    private CharacterController characterController;
    private Transform cameraTransform;
    private float verticalRotation;
    private Vector3 moveDirection = Vector3.zero;
    private float currentSpeed;
    private bool isCrouching = false;
    private bool isRunning = false;
    private bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        // Lock cursor and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set initial speed
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        HandleCameraRotation();
        HandleMovement();
        HandleCrouching();
        HandleRunning();
        UpdateAnimator();
    }

    void HandleCameraRotation()
    {
        // Mouse X rotation (left/right)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // Mouse Y rotation (up/down)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        isGrounded = characterController.isGrounded;

        // Reset y velocity when grounded
        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -2f; // Small force to keep player grounded
        }

        // Get input axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Jump if grounded and pressed jump button
        if (isGrounded && Input.GetButtonDown("Jump") && !isCrouching)
        {
            moveDirection.y = jumpForce;
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCrouching()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, crouchHeight / 2, 0); // Ajusta o centro
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            // Verifica espaço acima com SphereCast para ser mais preciso
            float raycastRadius = characterController.radius * 0.9f;
            float raycastDistance = standHeight - crouchHeight + 0.1f; // Margem de segurança

            if (!Physics.SphereCast(transform.position, raycastRadius, Vector3.up, out _, raycastDistance))
            {
                isCrouching = false;
                characterController.height = standHeight;
                characterController.center = new Vector3(0, standHeight / 2, 0); // Recentraliza
                currentSpeed = isRunning ? runSpeed : walkSpeed;
            }
            // Se não houver espaço, mantém agachado
        }
    }

    void HandleRunning()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCrouching)
        {
            isRunning = true;
            currentSpeed = runSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || isCrouching)
        {
            isRunning = false;
            currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
        }
    }

    void UpdateAnimator()
    {
        int transitionState = 0; // Default to idle

        // Only set movement states if grounded
        if (isGrounded)
        {
            // Get movement magnitude (0 when not moving)
            float moveMagnitude = new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")
            ).magnitude;

            if (moveMagnitude > 0.1f) // If moving
            {
                if (isCrouching)
                {
                    transitionState = 4; // Walking while crouched
                }
                else if (isRunning)
                {
                    transitionState = 2; // Running
                }
                else
                {
                    transitionState = 1; // Walking
                }
            }
            else if (isCrouching)
            {
                transitionState = 3; // Crouched idle
            }
        }

        animator.SetInteger("transition", transitionState);
    }
}