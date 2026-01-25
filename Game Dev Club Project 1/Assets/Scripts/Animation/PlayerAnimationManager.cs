using UnityEngine;

// place on animator
public class PlayerAnimationManager : MonoBehaviour
{
    Animator animator;

    public static readonly int IdleHash = Animator.StringToHash("Idle");
    public static readonly int WalkHash = Animator.StringToHash("Walk");
    public static readonly int SlashHash = Animator.StringToHash("Attack_Melee_Slash");
    // public static readonly int HeavyHash = Animator.StringToHash("Attack_Melee_Heavy");
    public static readonly int DashHash = Animator.StringToHash("Dash");
    
    const float crossFadeDuration = 0.0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StopAnimation()
    {
        if (animator == null){
            Debug.LogWarning("PlayerAnimationManager missing reference to an animator");
            return;
        }

        animator.StopPlayback();
    }

    public void PlayAnimation(int stateHashName, float animationSpeed) // do not use this, does not not work
    {
        if (animator == null){
            Debug.LogWarning("PlayerAnimationManager missing reference to an animator");
            return;
        }
        animator.speed = animationSpeed;
        animator.CrossFade(stateHashName, crossFadeDuration);
    }

    public void PlayAnimation(int stateHashName)
    {
        PlayAnimation(stateHashName, 1f);
    }

    public void PlayAnimationForce(int stateHashName, float speed = 1f)
    {
        if (animator == null) return;

        animator.speed = speed;
        animator.Play(stateHashName, 0, 0f);
    }

    public void PlayAnimationForce(int statehashName)
    {
        PlayAnimationForce(statehashName, 1f);
    }
}