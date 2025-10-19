using UnityEngine;

public class MeleeAttack : IAttack
{
    [SerializeField] private AttackData attackData;
    private float lastUsedTime = -999f;

    public MeleeAttack(AttackData data)
    {
        this.attackData = data;
    }

    public void ExecuteAttack(Transform spawnParent, Transform originator, Vector2 direction)
    {
        lastUsedTime = Time.time;

        if (attackData.hitboxPrefab != null)
        {
            // Instantiate hitbox as a child of the spawnParent (attack anchor), but configure the hitbox with the originator
            GameObject hb = Object.Instantiate(attackData.hitboxPrefab, spawnParent);
            Hitbox hitbox = hb.GetComponent<Hitbox>();
            hitbox.ConfigureAndDestroy(attackData.damage, originator.gameObject, attackData.hitboxDuration);
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
