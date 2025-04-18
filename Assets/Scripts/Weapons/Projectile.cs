using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    private float damage; // Initialize via method rather than directly
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerStats stats = collision.collider.GetComponentInParent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
