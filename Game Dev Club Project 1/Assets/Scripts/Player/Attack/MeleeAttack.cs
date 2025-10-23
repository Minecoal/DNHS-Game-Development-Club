using System;
using UnityEngine;

public class MeleeAttack : IAttack
{
    [SerializeField] private AttackData attackData;
    private float lastUsedTime = -999f;
    private int attackIndex = -1;
    private Action<int, DamageResult, DamageInfo> onHitCallback;

    public MeleeAttack(AttackData data, int index = -1, Action<int, DamageResult, DamageInfo> onHit = null)
    {
        this.attackData = data;
        this.attackIndex = index;
        this.onHitCallback = onHit;
    }

    public void ExecuteAttack(Transform spawnParent, Transform originator, Vector2 direction)
    {
        lastUsedTime = Time.time;

        if (attackData.hitboxPrefab != null)
        {
            // Instantiate hitbox as a child of the spawnParent (attack anchor), but configure the hitbox with the originator
            GameObject hb = UnityEngine.Object.Instantiate(attackData.hitboxPrefab, spawnParent);
            Hitbox hitbox = hb.GetComponent<Hitbox>();
            if (hitbox != null)
            {
                // subscribe to hit event
                if (onHitCallback != null)
                {
                    hitbox.OnHit += (result, info) => onHitCallback(attackIndex, result, info);
                }
                hitbox.ConfigureAndDestroy(attackData.damage, originator.gameObject, attackData.hitboxDuration);
            }
            // change to pooling if needed in the future
        }
        // trigger animation
    }

    public void CancelAttack()
    {
        //cancel attack animation
        //trigger cancel animation

        //Destory hb
    }

    public bool IsAvailable() => Time.time >= lastUsedTime + attackData.cooldown;
}
