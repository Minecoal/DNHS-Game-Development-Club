using System;
using UnityEngine;

public abstract class AttackCommand : ICommand
{
    protected readonly PlayerContext context;
    protected readonly AttackData attackData;
    // public Action OnReady; // called when attack cooldown ready

    public abstract void Execute();

    protected AttackCommand(PlayerContext context, AttackData attackData)
    {
        this.context = context;
        this.attackData = attackData;
    }

    public static T Create<T>(PlayerContext context, AttackData data) where T : AttackCommand
    {
        return (T) System.Activator.CreateInstance(typeof(T), context, data); // create instance of class with unknown type
    }
}