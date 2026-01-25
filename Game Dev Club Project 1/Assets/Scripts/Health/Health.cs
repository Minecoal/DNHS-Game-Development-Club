using System.Collections;
using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamagable, IHealable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Defense")]
    [Tooltip("Damage reduction as a fraction: 0 = no reduction, 1 = fully immune")]
    [Range(0f, 1f)]
    [SerializeField] private float damageReduction = 0f;

    [Header("I-frames")]
    [SerializeField] private bool startInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 0.2f;

    public Action<DamageInfo> OnDamageTaken; // passes final damage amount
    public Action<HealInfo> OnHeal;
    public Action OnDied;

    public bool isDead = false;
    private bool isInvulnerable = false;

    private void Reset()
    {
        maxHealth = 100f;
        currentHealth = maxHealth;
        damageReduction = 0f;
    }

    private void Awake()
    {
        if (startInvulnerable && invulnerabilityDuration > 0f)
            StartCoroutine(TemporaryInvulnerability(invulnerabilityDuration));

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);


    }

    private void Start()
    {
        TextDisplay healthDisplay = TextDisplayManager.New3D(Vector3.zero, 0.1f)
            .WithTrackedProvider(() => currentHealth.ToString())
            .WithParent(transform)
            .WithDraggable()
            .Build();
    }

    public DamageResult ApplyDamage(in DamageInfo info)
    {
        if (isDead) return DamageResult.Immune;
        if (isInvulnerable) return DamageResult.Immune;

        float raw = info.Amount;
        if (raw == 0f) return DamageResult.Immune;
        float final = Mathf.Max(0f, raw * (1f - damageReduction));

        if (final <= 0f)
            return DamageResult.Blocked;

        currentHealth -= final;
        OnDamageTaken?.Invoke(info);

        if (invulnerabilityDuration > 0f)
            StartCoroutine(TemporaryInvulnerability(invulnerabilityDuration));

        if (currentHealth <= 0f)
        {
            Die();
            return DamageResult.Damaged;
        }

        return DamageResult.Damaged;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDied?.Invoke();
        gameObject.SetActive(false);   // dissable by default, listeners can overide
    }

    public HealResult ApplyHeal(in HealInfo info)
    {
        if (info.Amount <= 0f || isDead) return HealResult.Invalid;
        currentHealth = Mathf.Min(maxHealth, currentHealth + info.Amount);
        OnHeal?.Invoke(info);
        return HealResult.Healed;
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
    public void ResetToFull()
    {
        currentHealth = maxHealth;
        if (startInvulnerable)
            StartCoroutine(TemporaryInvulnerability(invulnerabilityDuration));
    }

    private IEnumerator TemporaryInvulnerability(float seconds)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(seconds);
        isInvulnerable = false;
    }
}
