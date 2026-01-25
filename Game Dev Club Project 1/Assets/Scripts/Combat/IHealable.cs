using UnityEngine;

public enum HealResult
{
    Healed,
    Canceled,
    Invalid
}

public struct HealInfo
{
    public float Amount;
    public GameObject Source;
}

public interface IHealable
{
    HealResult ApplyHeal(in HealInfo info);
}
