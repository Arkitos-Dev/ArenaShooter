using System.Collections;
using UnityEngine;

public class DroneEnemy : BaseEnemy
{
    private Drone drone;
    private DronePhysics dronePhys;
    private MaterialPropertyBlock propBlock;
    private Renderer rend;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        rend = GetComponentInChildren<Renderer>();
        drone = GetComponent<Drone>();
        dronePhys = GetComponent<DronePhysics>();

        propBlock = new MaterialPropertyBlock();
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_DissolveAmount", -1f);
        rend.SetPropertyBlock(propBlock);
    }

    protected override void Die()
    {
        base.Die();
        isAlive = false;
        drone.travelForce = 0f;
        dronePhys.pGain = 0f;
        dronePhys.dGain = 0f;
        dronePhys.iGain = 0f;
        dronePhys.thrust = 0f;

        StartCoroutine(DissolveEffect());
        Invoke("DestroyGameObject", 3f);
    }

    public override void TakeDamage(float amount, Vector3 impactDir, float impactForce)
    {
        rb.AddForce(impactDir * impactForce, ForceMode.Impulse);
        health -= amount;
        
        if (health <= 0f)
        {
            Die();
        }
    }

    protected new IEnumerator DissolveEffect()
    {
        float elapsed = 0.0f;
        float startValue = -1f;
        float endValue = 1f;
        
        while (elapsed < timeToDie)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsed / timeToDie);
            propBlock.SetFloat("_DissolveAmount", currentValue);
            rend.SetPropertyBlock(propBlock);
            yield return null;
        }
    }
    
    public override void ScaleStats(int roundNumber)
    {
        base.ScaleStats(roundNumber);
        Debug.Log($"Drone health scaled for round {roundNumber}: Health is now {health}");
    }
}