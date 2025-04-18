using UnityEngine;
public class Pistol : Weapon
{
    protected override void Shoot()
    {
        shooting = true;
        currentAmmo--;
        muzzleFlash.Play();
        recoilAnim.RecoilAnimation();
        recoilFunc.RecoilFunc();
        audioSource.PlayOneShot(shootingSound);
        
        float spreadPistol = (playerMove.Rigid.velocity.sqrMagnitude > 0.1f) ? movingSpread : standingSpread;
        
        Ray ray = new Ray(fpsCam.transform.position, Spread(spreadPistol));
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
        Invoke(nameof(ResetShootingDelay), shootCoolDown);
    }
}