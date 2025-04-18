using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float slideSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public MovementState state;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    public bool readyToJump = true;
    public bool wasAirborne;
    [SerializeField] private float gravityScale;
    private int jumpCount = 0;
    [SerializeField] private int maxJumps = 1;

    [Header("Crouching")] 
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchHeight;
    private float startHeight;
    public bool isCrouching = false;
    
    [Header("Keybinds")] 
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    
    [Header("Ground Check")] 
    [SerializeField] private float playerHeight;
    public LayerMask whatIsGround;
    public bool isGrounded;

    [Header("Slope Handling")] 
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;
    
    public Transform orientiation;
    
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;
    
    [HideInInspector] public Vector3 moveDirection;
    public Rigidbody Rigid;
    
    public bool sliding;
    public Transform playerObject;
    public Audio audioScript;
    
    public enum MovementState
    {
        walking, 
        sprint,
        sliding,
        crouching,
        air
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Rigid = GetComponent<Rigidbody>();

        startHeight = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        //ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        PlayerInput();
        SpeedControl();
        StateHandler();
        
        wasAirborne = !isGrounded;
        
        //handle drag
        if (isGrounded)
        {
            Rigid.drag = groundDrag;
            jumpCount = 0;
        }
        else
        {
            Rigid.drag = 0;
        }
        
        if (state == MovementState.sprint)
        {
            audioScript.SetStepRate(0.3f); // Faster step rate when sprinting
        }
        else
        {
            audioScript.SetStepRate(0.5f); // Normal step rate otherwise
        }
    }

    private void FixedUpdate()
    {
        //Increased Gravity
        Vector3 extraGravityForce = (Physics.gravity * Rigid.mass) * gravityScale;
        Rigid.AddForce(extraGravityForce);
        MovePlayer();
    }
    
    private void StateHandler()
    {
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && Rigid.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }
        else if (isGrounded && Input.GetKey(sprintKey) && !isCrouching)
        {
            state = MovementState.sprint;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (isCrouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }

        //checkt ob sich desiredMoveSpeed drastisch verändert hat
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }
        
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    //Ändert die Geschwindigkeit auf Zeit
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }
    
    private void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump && jumpCount < maxJumps)
        {
            Jump();
        }
        if (Input.GetKeyDown(crouchKey) && isGrounded && (verticalInput < 0.1 || horizontalInput < 0.1))
        {
            Crouch();
        }
        if (Input.GetKeyUp(crouchKey) && isCrouching)
        {
            StopCrouch();
        }
        
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientiation.forward * verticalInput + orientiation.right * horizontalInput;
        
        //on slope
        if (OnSlope() && !exitingSlope)
        {
            Rigid.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
        }
        
        //applied force in die richtung in die man sich bewegen will
        else if (isGrounded)
        {
            Rigid.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }
        //Air Control
        else if (!isGrounded)
        {
            Rigid.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
        //turn off gravity while on slope
        /*Rigid.useGravity = !OnSlope();*/
    }

    private void SpeedControl()
    {
        //limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (Rigid.velocity.magnitude > moveSpeed)
            {
                Rigid.velocity = Rigid.velocity.normalized * moveSpeed;
            }
        }
        //limiting speed
        else
        {
            Vector3 flatVel = new Vector3(Rigid.velocity.x, 0f, Rigid.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                Rigid.velocity = new Vector3(limitedVel.x, Rigid.velocity.y, limitedVel.z);
            }
        }
    }
    
    private void Jump()
    {
        jumpCount++;
        readyToJump = false;
        Rigid.velocity = new Vector3(Rigid.velocity.x, 0f, Rigid.velocity.z); // Reset Y Velocity
        Rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private void Crouch()
    {
        isCrouching = true;
        playerObject.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        Rigid.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    private void StopCrouch()
    {
        isCrouching = false;
        playerObject.localScale = new Vector3(transform.localScale.x, startHeight, transform.localScale.z);
    }
    
    //Checkt ob der spieler auf einer Steigung steht
    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    //Passt den moveDirection Vector an die Steigung an
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
