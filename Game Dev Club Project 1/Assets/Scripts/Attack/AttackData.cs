using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public float damage = 10f;
    public float cooldown = 0.5f; // attack cooldown
    public float range = 1.2f;
    public GameObject hitboxPrefab; // a prefab with Hitbox component
    public float hitboxDuration = 0.2f; // attack duration
    public bool useAnimationEvent = false;
    public string animatorTrigger;
    public bool isProjectile;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    // more fields: knockback, hitstun, comboNext, etc.

    void OnValidate()
    {
        //forces serialization to refresh
    }
}