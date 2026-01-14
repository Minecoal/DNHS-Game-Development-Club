using UnityEngine;

[CreateAssetMenu(menuName = "Attack Data")]
public class AttackData : ScriptableObject
{
    [Header(header: "Basic")]
    public float damage = 10f;
    public float cooldown = 0.5f; // attack cooldown

    [Header(header: "knockbacks")]
    public float selfImpulseForce = 0f; // negative for recoil, positive for forward boost;
    public float targetImpulseForce = 0f; // negative for recoil, positive for forward boost;
     
    [Header(header: "Interrupt")]
    public bool interruptible;
}