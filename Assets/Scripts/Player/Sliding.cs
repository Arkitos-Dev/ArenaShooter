using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")] 
    public Transform playerObject;
    private Rigidbody Rigid;
    private PlayerMovement playerMovement;

    [Header("Sliding")] 
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    private float slideTimer;
    [SerializeField] private float slideYScale;
    private float startYScale;
    
    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    
    void Start()
    {
        Rigid = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        startYScale = playerObject.localScale.y;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(slideKey) && (playerMovement.horizontalInput != 0 || playerMovement.verticalInput != 0))
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey))
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        if (playerMovement.sliding)
        {
            SlidingMovement();
        }
    }

    private void StartSlide()
    {
        playerMovement.sliding = true;

        playerObject.localScale = new Vector3(playerObject.localScale.x, slideYScale, playerObject.localScale.z);
        Rigid.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        //sliding normal
        if (!playerMovement.OnSlope() || Rigid.velocity.y > -0.1f)
        {
            Rigid.AddForce(playerMovement.moveDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            Rigid.AddForce(playerMovement.GetSlopeMoveDirection(playerMovement.moveDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }
    
    private void StopSlide()
    {
        playerMovement.sliding = false;
        
        playerObject.localScale = new Vector3(playerObject.localScale.x, startYScale, playerObject.localScale.z);
    }
}
