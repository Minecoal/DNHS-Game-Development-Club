using UnityEngine;

public enum DamageResult
{
    Damaged,
    Blocked,
    Immune
}

public enum HealResult
{
    Healed,
    Canceled,
    Invalid
}

public struct DamageInfo
{
    public float Amount;
    public GameObject Source;
    public Vector3 HitPoint;
    public Vector3 HitNormal;
    public bool IsCritical;
    public Vector3 Knockback;
    public string DamageType; // string tag for damage classification

    public static DamageInfo FromAmount(float amount)
    {
        return new DamageInfo { Amount = amount };
    }
}

public struct HealInfo
{
    public float Amount;
    public GameObject Source;
}


public interface IDamagable
{
    DamageResult ApplyDamage(in DamageInfo info); //read only
    HealResult ApplyHeal(in HealInfo info);
    bool IsDead();
}
