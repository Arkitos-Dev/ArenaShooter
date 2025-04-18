using UnityEngine;

public class Shotgun : Weapon
{
    [Header("Shotgun")]
    [SerializeField] private int pelletsPerShot = 10;
    [SerializeField] private float baseRecoilForce = 5f;
    private Vector3 recoilDirection;
    public Rigidbody playerRigidbody;
    
    protected override void Shoot()
    {
        shooting = true;
        currentAmmo--;
        muzzleFlash.Play();
        recoilAnim.RecoilAnimation();
        recoilFunc.RecoilFunc();
        audioSource.PlayOneShot(shootingSound);
        
        //For multiple shots
        for (int i = 0; i < pelletsPerShot; i++)
        {
            float spreadShotgun = (playerMove.Rigid.velocity.sqrMagnitude > 0.1f) ? movingSpread : standingSpread;
            
            Ray ray = new Ray(fpsCam.transform.position, Spread(spreadShotgun));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, range))
            {
                BaseEnemy enemy = hit.collider.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    Vector3 forceDir = -hit.normal;
                    enemy.TakeDamage(damage, forceDir, impactForce);
                    HitEffect(hit, hitEffectMetal);
                }
                else
                {
                    HitEffect(hit, hitEffectMetal);
                }
                
                playerRigidbody.AddForce(RocketJump(hit,Spread(spreadShotgun)), ForceMode.Impulse);
            }
            BulletTracer(ray);
        }
        UpdateAmmoText();
        Invoke(nameof(ResetShootingDelay), shootCoolDown);
    }
    
    private Vector3 RocketJump(RaycastHit hit, Vector3 spreadDirection)
    {
        //get distance between hitpoint and player, add force in opposite dir of spread dir, divide force by distance
        float distance = Vector3.Distance(hit.point, fpsCam.transform.position);
        float recoilForce = baseRecoilForce / distance;
        return recoilDirection = -spreadDirection * recoilForce;
    }
}
