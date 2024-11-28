using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SC_CharacterController : MonoBehaviour
{   
    private FootstepController footstepController;
    public float speed = 7.5f;
    public float crouchSpeed = 3.0f; // Speed while crouching
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public float crouchHeight = 1.0f; // Character height while crouching
    public float standHeight = 2.0f; // Character height while standing
    public float crouchTransitionSpeed = 5.0f; // Speed of height transition
    public Vector3 crouchCameraOffset = new Vector3(0, -0.5f, 0); // Camera position offset while crouching

    private CharacterController characterController;
    public Vector3 moveDirection = Vector3.zero;
    private Vector2 rotation = Vector2.zero;

    private bool isCrouching = false;
    private bool isTransitioningCrouch = false;
    private Vector3 originalCameraPosition;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;

        // Store the initial camera position
        originalCameraPosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        HandleMovement();
        HandleCrouching();
        HandleRotation();
        
    }

    void HandleMovement()
    {
        if (characterController.isGrounded)
        {
            // Recalculate move direction based on input axes
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float curSpeedX = canMove ? (isCrouching ? crouchSpeed : speed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isCrouching ? crouchSpeed : speed) * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);

            if (Input.GetButton("Jump") && canMove && !isCrouching)
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void HandleCrouching()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            isTransitioningCrouch = true;
        }

        // Smooth transition between crouch and stand heights
        if (isTransitioningCrouch)
        {
            float targetHeight = isCrouching ? crouchHeight : standHeight;
            characterController.height = Mathf.Lerp(characterController.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);

            // Adjust the camera position
            Vector3 targetCameraPosition = isCrouching
                ? originalCameraPosition + crouchCameraOffset
                : originalCameraPosition;
            playerCamera.transform.localPosition = Vector3.Lerp(
                playerCamera.transform.localPosition,
                targetCameraPosition,
                Time.deltaTime * crouchTransitionSpeed
            );

            // Stop transitioning when the target height is reached
            if (Mathf.Abs(characterController.height - targetHeight) < 0.01f)
            {
                characterController.height = targetHeight;
                playerCamera.transform.localPosition = targetCameraPosition;
                isTransitioningCrouch = false;
            }
        }
    }

    void HandleRotation()
    {
        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }
}
