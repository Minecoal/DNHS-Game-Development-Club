using UnityEngine;

// place on animator
public class PlayerAnimationManager : BaseAnimationManager
{
    public static readonly int IdleHash = Animator.StringToHash("Idle");
    public static readonly int WalkHash = Animator.StringToHash("Walk");
    public static readonly int SlashHash = Animator.StringToHash("Attack_Melee_Slash");
    // public static readonly int HeavyHash = Animator.StringToHash("Attack_Melee_Heavy");
    public static readonly int DashHash = Animator.StringToHash("Dash");
}