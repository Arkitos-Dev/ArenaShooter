using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public abstract class Weapon : MonoBehaviour
{
    [Header("General Stats")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float range = 100f;
    [SerializeField] protected int maxAmmo = 30;
    [SerializeField] protected int reserveAmmo;
    [SerializeField] protected int maxReserveAmmo = 90;
    [SerializeField] protected float reloadTime = 2f;
    [SerializeField] protected float timeForAltFire;
    [SerializeField] protected float shootCoolDown;
    [SerializeField] protected float standingSpread = 0.1f;
    [SerializeField] protected float movingSpread = 2.0f;
    [SerializeField] protected float impactForce = 10f;
    [SerializeField] protected TextMeshProUGUI ammoText;
    
    protected float mouse1PressDuration = 0f;
    protected int currentAmmo;
    protected bool isReloading = false;
    protected bool shooting;
    protected Vector3 spreadDirection;
    
    [Header("References")]
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject muzzlePos;
    public TrailRenderer bulletTracerPrefab;
    public ParticleSystem hitEffectMetal;
    public ParticleSystem hitEffectEnemy;
    public RecoilAnim recoilAnim;
    public Recoil recoilFunc;
    public PlayerMovement playerMove;
    public Image reloadIndicator;
    public Image reloadBg;
    public Animator weaponAnim;
    public AudioClip reloadingSound;
    public AudioClip shootingSound;
    public AudioSource audioSource;
    
    protected virtual void Start()
    {
        reserveAmmo = maxReserveAmmo;
        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !shooting && !isReloading && currentAmmo != maxAmmo || currentAmmo <= 0 && !shooting && !isReloading) 
        {
            StartCoroutine(Reload());
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading && !shooting && currentAmmo > 0)
        {
            Shoot();
        }
    }
    
    protected abstract void Shoot();
    protected void ResetShootingDelay()
    {
        shooting = false;
    }
    
    public IEnumerator Reload()
    {
        if (isReloading || reserveAmmo <= 0)
            yield break;

        isReloading = true;
        audioSource.PlayOneShot(reloadingSound);
        weaponAnim.SetBool("Reload", true);
        reloadIndicator.gameObject.SetActive(true);
        reloadBg.gameObject.SetActive(true);
        reloadIndicator.fillAmount = 0;

        float elapsedTime = 0;

        while (elapsedTime < reloadTime)
        {
            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;
            reloadIndicator.fillAmount = Mathf.Clamp01(elapsedTime / reloadTime);
        }

        int ammoNeeded = maxAmmo - currentAmmo;
        if (ammoNeeded > reserveAmmo)
        {
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }
        else
        {
            currentAmmo += ammoNeeded;
            reserveAmmo -= ammoNeeded;
        }

        isReloading = false;
        UpdateAmmoText();
        weaponAnim.SetBool("Reload", false);
        reloadIndicator.fillAmount = 1;
        yield return new WaitForEndOfFrame();
        reloadIndicator.fillAmount = 0; 
        reloadIndicator.gameObject.SetActive(false);
        reloadBg.gameObject.SetActive(false);
    }
    
    public Vector3 Spread(float spread)
    {
        //spread range
        float spreadX = Random.Range(-spread / 2, spread / 2);
        float spreadY = Random.Range(-spread / 2, spread / 2);

        //calc for ray direction (multiplying both quaternions and the dir results in a cone like shape)
        return spreadDirection = Quaternion.AngleAxis(spreadY, fpsCam.transform.right) * 
                                 Quaternion.AngleAxis(spreadX, fpsCam.transform.up) * 
                                 fpsCam.transform.forward;
    }

    public void BulletTracer(Ray ray)
    {
        TrailRenderer bulletTracer = Instantiate(bulletTracerPrefab, muzzlePos.transform.position, Quaternion.identity);
        bulletTracer.AddPosition(muzzlePos.transform.position);

        RaycastHit tracerHit;
        if (Physics.Raycast(ray, out tracerHit, range)) {
            bulletTracer.transform.position = tracerHit.point;
        }
        else {
            bulletTracer.transform.position = ray.GetPoint(range);
        }
        
        Destroy(bulletTracer.gameObject, 1f);
    }
    
    protected void HitEffect(RaycastHit hit, ParticleSystem effectPrefab)
    {
        ParticleSystem impactEffect = Instantiate(effectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
        impactEffect.transform.forward = hit.normal;
        impactEffect.Emit(1);
        
        Destroy(impactEffect.gameObject, 1f);
    }

    public void AddAmmo(int ammo)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + ammo, maxReserveAmmo);
        UpdateAmmoText();
    }
    
    public void UpdateAmmoText()
    {
        ammoText.text = $"{currentAmmo} / {reserveAmmo}";
    }
}