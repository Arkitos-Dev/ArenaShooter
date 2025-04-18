using UnityEngine;

public class TurretStats : BaseEnemy
{
    
    protected override void Die()
    {
        base.Die();
        isAlive = false;
        Destroy(gameObject);
    }
    
    public override void TakeDamage(float amount, Vector3 impactDir, float impactForce)
    {
        health -= amount;
        
        if (health <= 0f)
        {
            Die();
        }
        Debug.Log("Turret Health:" + health);
    }

    public override void ScaleStats(int roundNumber)
    {
        base.ScaleStats(roundNumber);
        Debug.Log($"Turret health scaled for round {roundNumber}: Health is now {health}");
    }
}
