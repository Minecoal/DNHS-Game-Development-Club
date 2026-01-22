using UnityEngine;

public class BaseAnimationManager : MonoBehaviour
{
    Animator animator;
    const float crossFadeDuration = 0.0f;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StopAnimation()
    {
        if (animator == null){
            Debug.LogWarning($"{GetType().Name} missing reference to an animator");
            return;
        }

        animator.StopPlayback();
    }

    public void PlayAnimation(int stateHashName, float animationSpeed = 1f)
    {
        if (animator == null){
            Debug.LogWarning($"{GetType().Name} missing reference to an animator");
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
