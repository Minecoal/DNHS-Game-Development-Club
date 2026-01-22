using System;
using NUnit.Framework;
using UnityEngine;


public interface IWeapon
{
    public bool TryAttack(PlayerContext context, bool isDashing);
    public Action OnEnableSwitchState { get; set; }
}