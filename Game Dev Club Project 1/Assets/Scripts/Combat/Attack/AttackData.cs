using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    [Header(header: "Basic")]
    public float damage = 10f;
    public float cooldown = 0.5f; // attack cooldown

    [Header(header: "knockbacks")]
    public float selfKnockbackForce = 0f; // negative for recoil, positive for forward boost;
     
    [Header(header: "Interrupt")]
    public bool interruptible;

    [Header(header: "Animation")]
    public float animationSpeed = 1f;
    public AnimationID animationID;

    [Header(header: "Hitbox")]
    public HitboxData hitboxData;
}