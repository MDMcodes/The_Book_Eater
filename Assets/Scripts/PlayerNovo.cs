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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("enemy"))
        {
            Debug.Log("Player colidiu com inimigo!");
        }
    }
        void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -2f;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump") && !isCrouching)
        {
            moveDirection.y = jumpForce;
        }

        moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCrouching()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, crouchHeight / 2, 0);
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            float raycastRadius = characterController.radius * 0.9f;
            float raycastDistance = standHeight - crouchHeight + 0.1f;

            if (!Physics.SphereCast(transform.position, raycastRadius, Vector3.up, out _, raycastDistance))
            {
                isCrouching = false;
                characterController.height = standHeight;
                characterController.center = new Vector3(0, standHeight / 2, 0);
                currentSpeed = isRunning ? runSpeed : walkSpeed;
            }
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
        int transitionState = 0;

        if (isGrounded)
        {
            float moveMagnitude = new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")
            ).magnitude;

            if (moveMagnitude > 0.1f)
            {
                if (isCrouching)
                {
                    transitionState = 4;
                }
                else if (isRunning)
                {
                    transitionState = 2;
                }
                else
                {
                    transitionState = 1;
                }
            }
            else if (isCrouching)
            {
                transitionState = 3;
            }
        }

        animator.SetInteger("transition", transitionState);
    }
}
