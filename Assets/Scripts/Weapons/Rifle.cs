using UnityEngine;

public class Rifle : Weapon
{
    [Header("Rifle")]
    [SerializeField] private float fireRate = 0.1f;
    private float nextTimeToFire = 0f;
    
    protected override void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && Time.time >= nextTimeToFire && !isReloading && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
        if (Input.GetKeyDown(KeyCode.R) && !shooting && !isReloading && currentAmmo != maxAmmo || currentAmmo <= 0 && !shooting && !isReloading) 
        {
            StartCoroutine(Reload());
        }
    }
    
    protected override void Shoot()
    {
        shooting = true;
        currentAmmo--;
        muzzleFlash.Play();
        recoilAnim.RecoilAnimation();
        recoilFunc.RecoilFunc();
        audioSource.PlayOneShot(shootingSound);

        float spreadRifle = (playerMove.Rigid.velocity.sqrMagnitude > 0.1f) ? movingSpread : standingSpread;
        
        Ray ray = new Ray(fpsCam.transform.position, Spread(spreadRifle));
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
        }
        
        UpdateAmmoText();
        BulletTracer(ray);
        Invoke(nameof(ResetShootingDelay), fireRate);
    }
}
