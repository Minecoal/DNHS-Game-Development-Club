using UnityEngine;
using System.Collections.Generic;

public class BaseAnimationManager : MonoBehaviour
{
    Animator animator;
    const float crossFadeDuration = 0.0f;
    [SerializeField] public AnimationList animationList;
    private Dictionary<AnimationID, int> animationHashes;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animationList != null)
        {
            animationHashes = new Dictionary<AnimationID, int>();
            foreach (var entry in animationList.entries)
            {
                animationHashes[entry.id] = entry.hash;
            }
        }
    }

    public void StopAnimation()
    {
        if (animator == null){
            Debug.LogWarning($"{GetType().Name} missing reference to an animator");
            return;
        }

        animator.StopPlayback();
    }

    public void PlayAnimation(int stateHash, float animationSpeed = 1f)
    {
        if (animator == null){
            Debug.LogWarning($"{GetType().Name} missing reference to an animator");
            return;
        }
        animator.speed = animationSpeed;
        animator.CrossFade(stateHash, crossFadeDuration);
    }

    public void PlayAnimation(int stateHash)
    {
        PlayAnimation(stateHash, 1f);
    }

    public void PlayAnimationForce(int stateHash, float speed = 1f)
    {
        if (animator == null) return;

        animator.speed = speed;
        animator.Play(stateHash, 0, 0f);
    }

    public void PlayAnimationForce(int statehash)
    {
        PlayAnimationForce(statehash, 1f);
    }

    public void PlayAnimation(AnimationID id, float animationSpeed = 1f)
    {
        if (animationHashes != null && animationHashes.TryGetValue(id, out int hash))
        {
            PlayAnimation(hash, animationSpeed);
        }
        else
        {
            Debug.LogWarning($"Animation hash for {id} not found.");
        }
    }

    public void PlayAnimation(AnimationID id)
    {
        PlayAnimation(id, 1f);
    }

    public void PlayAnimationForce(AnimationID id, float speed = 1f)
    {
        if (animationHashes != null && animationHashes.TryGetValue(id, out int hash))
        {
            PlayAnimationForce(hash, speed);
        }
        else
        {
            Debug.LogWarning($"Animation hash for {id} not found.");
        }
    }

    public void PlayAnimationForce(AnimationID id)
    {
        PlayAnimationForce(id, 1f);
    }
}