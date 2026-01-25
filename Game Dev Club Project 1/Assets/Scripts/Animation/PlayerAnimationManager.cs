using UnityEngine;

public class PlayerAnimationManager : BaseAnimationManager
{
    [SerializeField] private AnimationID idle;
    [SerializeField] private AnimationID walk;
    [SerializeField] private AnimationID dash;

    public AnimationID Idle => idle;
    public AnimationID Walk => walk;
    public AnimationID Dash => dash;
}
