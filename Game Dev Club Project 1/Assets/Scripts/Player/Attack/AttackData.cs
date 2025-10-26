using UnityEngine;

[CreateAssetMenu(menuName = "Attack Data")]
public class AttackData : ScriptableObject
{
    [Header(header: "Basic")]
    public string attackName;
    public float damage = 10f;
    public float cooldown = 0.5f; // attack cooldown
    public float range = 1.2f;
    public float attackStateTime = 0.5f; // time entering attack state

    [Header(header: "Hitbox")]
    public GameObject hitboxPrefab; // a prefab with Hitbox component
    public float hitboxDuration = 0.2f; // attack duration

    [Header(header: "Animation")]
    public bool useAnimationEvent = false;
    public string animatorTrigger;

    [Header(header: "Projectile")]
    public bool isProjectile;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    // more fields: knockback, hitstun, comboNext, etc.
}