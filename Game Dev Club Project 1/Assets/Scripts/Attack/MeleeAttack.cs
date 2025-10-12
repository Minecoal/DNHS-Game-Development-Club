using UnityEngine;

public class MeleeAttack : IAttack
{
    [SerializeField] private AttackData attackData;
    private float lastUsedTime = -999f;

    public MeleeAttack(AttackData data)
    {
        this.attackData = data;
    }

    public void ExecuteAttack(Transform origin, Vector2 direction)
    {
        lastUsedTime = Time.time;

        if (attackData.hitboxPrefab != null)
        {
            GameObject hb = Object.Instantiate(attackData.hitboxPrefab, origin);
            Hitbox hitbox = hb.GetComponent<Hitbox>();
            hitbox.ConfigureAndDestroy(attackData.damage, origin.gameObject, attackData.hitboxDuration);
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
