using System.Collections;
using UnityEngine;

public class Attack_Melee_Light : AttackCommand
{
    public Attack_Melee_Light(PlayerContext context, AttackData attackData) : base(context, attackData){}

    public override void Execute()
    {
        context.AnimationManager.PlayAnimationForce(PlayerAnimationManager.SlashHash);
        context.Player.ApplyForce(attackData.selfImpulseForce, context);
        GameObject hitbox = HitboxManager.Instance.CreateNewHitbox(HitboxType.Light).WithSpawnPoint(context.AttackAnchor).Build(context.PlayerGO, attackData.damage);
    }
}

public class Attack_Melee_Heavy : AttackCommand
{
    public Attack_Melee_Heavy(PlayerContext context, AttackData attackData) : base(context, attackData){}

    public override void Execute()
    {
        context.AnimationManager.PlayAnimationForce(PlayerAnimationManager.SlashHash, 0.50f);
        context.Player.ApplyForce(attackData.selfImpulseForce, context);
        GameObject hitbox = HitboxManager.Instance.CreateNewHitbox(HitboxType.Heavy).WithSpawnPoint(context.AttackAnchor).Build(context.PlayerGO, attackData.damage);
    }
}

public class Attack_Melee_Dash : AttackCommand
{
    public Attack_Melee_Dash(PlayerContext context, AttackData attackData) : base(context, attackData){}

    public override void Execute()
    {
        context.AnimationManager.PlayAnimationForce(PlayerAnimationManager.SlashHash, 0.75f);
        context.Player.ApplyForce(attackData.selfImpulseForce, context);
        GameObject hitbox = HitboxManager.Instance.CreateNewHitbox(HitboxType.Dash).WithSpawnPoint(context.AttackAnchor).Build(context.PlayerGO, attackData.damage);
    }
}




