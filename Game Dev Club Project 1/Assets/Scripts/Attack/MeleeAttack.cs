using System;
using UnityEngine;
using System.Collections;

public class MeleeAttack : IAttack
{
    [SerializeField] private AttackData attackData;
    private float executeTimeStamp = -999f;
    private int attackIndex = -1;
        private Action<int, DamageResult, DamageInfo> onHitCallback;
        private Action<int> onReadyCallback;

        public MeleeAttack(AttackData data, int index = -1, Action<int, DamageResult, DamageInfo> onHit = null, Action<int> onReady = null)
    {
        this.attackData = data;
        this.attackIndex = index;
        this.onHitCallback = onHit;
        this.onReadyCallback = onReady;
    }

    public void ExecuteAttack(Transform spawnOrigin, Transform owner, Vector3 direction)
    {
        executeTimeStamp = Time.time;

        if (attackData.hitboxPrefab == null) return;
        GameObject hb = UnityEngine.Object.Instantiate(attackData.hitboxPrefab, spawnOrigin);
        hb.transform.position = spawnOrigin.position;
        Hitbox hitbox = hb.GetComponent<Hitbox>();

        if (hitbox == null) return;

        if (onHitCallback != null)
        {
            hitbox.OnHit += (result, info) => onHitCallback(attackIndex, result, info);
        }

        hitbox.ConfigureAndDestroy(attackData.damage, owner.gameObject, attackData.hitboxDuration);
        // change to pooling if needed in the future
    }

    public void CancelAttack()
    {
        //cancel attack animation
        //trigger cancel animation
        //Destory hb
    }

    public bool IsAvailable(){
        return Time.time >= executeTimeStamp + attackData.cooldown;
    }

    public IEnumerator ReadyAfterCooldown()
    {
        float remaining = attackData.attackStateTime;
        if (remaining <= 0f)
        {
            onReadyCallback?.Invoke(attackIndex);
            yield break;
        }
        yield return new WaitForSeconds(remaining);
        onReadyCallback?.Invoke(attackIndex);
    }
}
