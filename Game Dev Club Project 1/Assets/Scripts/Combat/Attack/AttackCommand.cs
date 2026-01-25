using System;
using UnityEngine;

public class AttackCommand : ICommand
{
    protected readonly PlayerContext context;
    protected readonly AttackData attackData;

    public AttackCommand(PlayerContext context, AttackData attackData)
    {
        this.context = context;
        this.attackData = attackData;
    }

    public void Execute()
    {
        context.AnimationManager.PlayAnimationForce(attackData.animationID, attackData.animationSpeed);
        context.Player.ApplyForce(attackData.selfKnockbackForce, context);
        GameObject hitbox = HitboxManager.Instance.CreateNewHitbox(attackData.hitboxData).WithSpawnPoint(context.AttackAnchor).Build(context.PlayerGO, attackData.damage);
    }
}