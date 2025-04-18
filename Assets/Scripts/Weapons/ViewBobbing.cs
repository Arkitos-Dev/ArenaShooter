using UnityEngine;

public class ViewBobbing : MonoBehaviour
{
    [Header("Sway")]
    [SerializeField] private float step = 0.01f;
    [SerializeField] private float maxStepDistance = 0.06f;
    private Vector3 swayPos = Vector3.zero;

    [Header("Sway Rotation")]
    [SerializeField] private float rotationStep = 4f;
    [SerializeField] private float maxRotationStep = 5f;
    private Vector3 swayEulerRot = Vector3.zero;

    public float smooth = 10f;
    float smoothRot = 12f;

    [Header("Bobbing")]
    private float speedCurve;
    private float curveSin { get => Mathf.Sin(speedCurve); }
    private float curveCos { get => Mathf.Cos(speedCurve); }

    [SerializeField] private Vector3 travelLimit = Vector3.one * 0.025f;
    [SerializeField] private Vector3 bobLimit = Vector3.one * 0.01f;
    private Vector3 bobPosition = Vector3.zero;

    [Header("Bob Rotation")]
    [SerializeField] private Vector3 multiplier;
    [SerializeField] private float bobExaggeration;
    private Vector3 bobEulerRotation = Vector3.zero;
    private Vector3 initialPosition;
    
    private PlayerMovement mover;
    private Rigidbody playerRigidbody;
    

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
        
        playerRigidbody = GetComponentInParent<Rigidbody>();
        mover = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();
    }

    Vector2 walkInput;
    Vector2 lookInput;

    void GetInput()
    {
        walkInput.x = Input.GetAxisRaw("Horizontal");
        walkInput.y = Input.GetAxisRaw("Vertical");
        walkInput = walkInput.normalized;

        lookInput.x = Input.GetAxisRaw("Mouse X");
        lookInput.y = Input.GetAxisRaw("Mouse Y");
    }
    
    void Sway()
    {
        Vector3 invertLook = new Vector3(-lookInput.x * step, lookInput.y * step, 0f);
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }
    
    void SwayRotation()
    {
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);
        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }

    void CompositePositionRotation()
    {
        Vector3 desiredPosition = initialPosition + swayPos + bobPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPosition, Time.deltaTime * smooth);
        Quaternion desiredRotation = Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, desiredRotation, Time.deltaTime * smoothRot);
    }

    void LateUpdate()
    {
        CompositePositionRotation();
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (mover.isGrounded ? (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical")) * bobExaggeration : 1f) + 0.01f;

        if (playerRigidbody.velocity.magnitude < 0.1f)
        {
            // Player has no velocity, reduce bobbing
            bobPosition = Vector3.zero;
            
        }
        else
        {
            // Player is moving, apply bobbing
            bobPosition.x = (curveCos * bobLimit.x * (mover.isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x);
            bobPosition.y = (curveSin * bobLimit.y) - (Input.GetAxis("Vertical") * travelLimit.y);
            bobPosition.z = -(walkInput.y * travelLimit.z);

            // Increase bobbing when sprinting
            if (mover.state == PlayerMovement.MovementState.sprint)
            {
                bobPosition *= 1.5f;
            }
        }
    }

    void BobRotation()
    {
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) : multiplier.x * (Mathf.Sin(2 * speedCurve) / 2));
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);
    }
}
