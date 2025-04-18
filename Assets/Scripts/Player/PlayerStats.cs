using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float hitIntensity = 0.4f;
    [SerializeField] private TextMeshProUGUI healthTxt;
    private float _maxHealth = 100f;

    public PostProcessVolume volume;
    private Vignette _vignette;
    public GameObject gameOver;
    public PlayerMovement playerMovement;
    public Weapon weapon;
    public GameObject Interface;


    void Start()
    {
        volume.profile.TryGetSettings(out _vignette);
        
        ResetVignetteEffectImmediately();
        currentHealth = _maxHealth;
        UpdateHealth();
    }

    public void AddHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, _maxHealth);
        UpdateHealth();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealth();
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        
        TriggerVignetteEffect();
    }

    private void TriggerVignetteEffect()
    {
        if (_vignette != null)
        {
            _vignette.color.value = Color.red;
            _vignette.intensity.value = hitIntensity;
            StartCoroutine(ResetVignetteEffect());
        }
    }

    private IEnumerator ResetVignetteEffect()
    {
        yield return new WaitForSeconds(1f);

        float fadeOutDuration = 0.5f;
        float fadeOutRate = hitIntensity / fadeOutDuration;
        
        while (_vignette.intensity.value > 0)
        {
            _vignette.intensity.value -= Time.deltaTime * fadeOutRate;
            yield return null;
        }

        _vignette.intensity.value = 0;
    }

    private void ResetVignetteEffectImmediately()
    {
        if (_vignette != null)
        {
            _vignette.intensity.value = 0;
        }
    }

    private void Die()
    {
        playerMovement.enabled = false;
        weapon.enabled = false;
        Interface.SetActive(false);
        
        Time.timeScale = 0;
        gameOver.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UpdateHealth()
    {
        healthTxt.text = currentHealth.ToString("F0");
    }
}
