using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    [Header("Defense")]
    [Tooltip("Damage reduction as a fraction: 0 = no reduction, 0.5 = 50% damage reduced, 1 = fully immune")]
    [Range(0f, 1f)]
    [SerializeField] private float damageReduction = 0f;

    [Header("Invulnerability / I-frames")]
    [SerializeField] private bool startInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 0.0f;

    [Header("Events")]
    public UnityEvent<float> OnDamageTaken; // passes final damage amount
    public UnityEvent OnDied;

    private bool isDead = false;
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

    public DamageResult ApplyDamage(in DamageInfo info)
    {
        if (isDead) return DamageResult.Immune;
        if (isInvulnerable) return DamageResult.Immune;

        float raw = info.Amount;
        float final = Mathf.Max(0f, raw * (1f - damageReduction));

        if (final <= 0f)
        {
            // fully blocked by armor/defense
            return DamageResult.Blocked;
        }

        currentHealth -= final;
        OnDamageTaken?.Invoke(final);

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
        // default behaviour: disable GameObject. Consumers can override by listening to OnDied.
        gameObject.SetActive(false);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || isDead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;

    private IEnumerator TemporaryInvulnerability(float seconds)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(seconds);
        isInvulnerable = false;
    }
}
