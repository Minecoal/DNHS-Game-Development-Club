using System;
using UnityEngine;

public interface IWeapon
{
    public bool TryAttack(PlayerContext context);
    public Action OnEnableSwitchState { get; set; }
}