using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Hitbox Data")]
public class HitboxData : ScriptableObject
{
    public float lifetime = 0.2f;
    public bool triggerSlowmo = false;
}