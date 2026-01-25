using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Hitbox Data")]
public class HitboxData : ScriptableObject
{
    public GameObject prefab;
    public float lifetime = 0.2f;
    public float targetKnockBackForce = 0f; // negative for forward boost, positive for knockback;
}