using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected float baseHealth = 100f;
    [SerializeField] protected float health;
    [SerializeField] protected float baseDamage = 10f;  // Base damage to start with
    public float damage;
    [SerializeField] protected float scaleFactor = 1.2f;  // Factor to increase stats each round
    [SerializeField] protected float timeToDie = 1f;

    public bool isAlive = true;
    public Material material;
    public Material material1;

    public delegate void DeathDelegate();
    public event DeathDelegate OnDeath;

    public Rigidbody rb;
    protected Renderer renderer;

    public virtual void ScaleStats(int roundNumber)
    {
        float scale = Mathf.Pow(scaleFactor, roundNumber);
        health = baseHealth * scale;
        damage = baseDamage * scale;  // Scale damage by 1.2 each round
        Debug.Log($"Scaled Health: {health}, Scaled Damage: {damage}");
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        isAlive = true; // Ensure the enemy starts alive
    }

    public virtual void TakeDamage(float amount, Vector3 impactDir, float impactForce)
    {
        if (!isAlive) return; // Ignore damage if already dead

        rb.AddForce(impactDir * impactForce, ForceMode.Impulse);
        health -= amount;
        
        if (health <= 0f && isAlive)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }
        
        OnDeath = null; 
        StartCoroutine(DissolveEffect());  // Start the dissolve effect upon death
    }

    protected IEnumerator DissolveEffect()
    {
        float elapsed = 0.0f;
        float startValue = -1f;
        float endValue = 1f;

        while (elapsed < timeToDie)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsed / timeToDie);
            material.SetFloat("_DissolveAmount", currentValue);
            material1.SetFloat("_DissolveAmount", currentValue);
            yield return null;
        }

        material.SetFloat("_DissolveAmount", endValue);
        material1.SetFloat("_DissolveAmount", endValue);
        DestroyGameObject();  // Destroy game object after dissolve effect completes
    }

    protected void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
