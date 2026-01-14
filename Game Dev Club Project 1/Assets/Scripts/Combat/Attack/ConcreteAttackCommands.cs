using System.Collections;
using UnityEngine;

public class Attack_Melee_Slash : AttackCommand
{
    public Attack_Melee_Slash(PlayerContext context, AttackData attackData) : base(context, attackData){}

    public override void Execute()
    {
        context.AnimationManager.PlayAnimationForce(PlayerAnimationManager.SlashHash);
        GameObject hitbox = HitboxManager.Instance.CreateNewHitbox(HitboxType.SmallHorizontal).WithSpawnPoint(context.AttackAnchor).Build(context.PlayerGO, attackData.damage);
    }
}

public class Attack_Melee_Heavy : AttackCommand
{
    public Attack_Melee_Heavy(PlayerContext context, AttackData attackData) : base(context, attackData){}

    public override void Execute()
    {
        context.AnimationManager.PlayAnimationForce(PlayerAnimationManager.SlashHash, 0.25f);
        GameObject hitbox = HitboxManager.Instance.CreateNewHitbox(HitboxType.LargeVertical).WithSpawnPoint(context.AttackAnchor).Build(context.PlayerGO, attackData.damage);
    }
}


