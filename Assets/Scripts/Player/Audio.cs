using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    public AudioClip[] footstepSounds;
    public AudioClip jumpSound;
    public AudioClip landingSound;
    public AudioClip slidingSound;
    public float stepRate = 0.5f;
    public float stepCooldown;

    private bool isCurrentlySliding = false;
    private AudioSource audioSource;
    private Rigidbody Rigid;
    private PlayerMovement playerMov;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Rigid = GetComponent<Rigidbody>();
        playerMov = GetComponent<PlayerMovement>();
        stepCooldown = stepRate;
    }

    void Update()
    {
        if (playerMov.isGrounded && Rigid.velocity.magnitude > 0.5f)
        {
            stepCooldown -= Time.deltaTime;
            
            if (stepCooldown <= 0f && !audioSource.isPlaying)
            {
                PlayFootstepSound();
                stepCooldown = stepRate;
            }
            
        }
        else
        {
            stepCooldown = stepRate;
        }
        if (!playerMov.readyToJump)
        {
            PlaySound(jumpSound);
        }

        if (playerMov.sliding && !isCurrentlySliding)
        {
            audioSource.clip = slidingSound;
            audioSource.loop = true; // Ensure the sound loops
            audioSource.Play();
            isCurrentlySliding = true;
        }
        else if (!playerMov.sliding && isCurrentlySliding)
        {
            audioSource.Stop();
            isCurrentlySliding = false;
        }

        if (playerMov.wasAirborne && playerMov.isGrounded)
        {
            PlaySound(landingSound);
        }
        
    }
    
    private void PlayFootstepSound()
    {
        AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.PlayOneShot(clip);
    }
    
    private void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    
    public void SetStepRate(float rate)
    {
        stepRate = rate;
    }
}