using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private float lifetime = 0.2f;
    private float damage;
    private GameObject self;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>(); //HastSet for O(1) complexity

    public void ConfigureAndDestroy(float damage, GameObject self, float lifetime = 0.2f)
    {
        this.damage = damage;
        this.self = self;
        this.lifetime = lifetime;
        Destroy(gameObject, this.lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamagable target = collision.GetComponent<IDamagable>();
        if (target == null) return;
        if (collision.gameObject == self) return;
        if (alreadyHit.Contains(collision.gameObject)) return;

        Vector2 hitPoint = collision.ClosestPoint(transform.position);
        Vector3 hitPoint3 = new Vector3(hitPoint.x, hitPoint.y, transform.position.z);

        DamageInfo info = new DamageInfo
        {
            Amount = damage,
            Source = self,
            HitPoint = hitPoint3,
            HitNormal = (transform.position - hitPoint3).normalized
        };

        target.ApplyDamage(in info);
        alreadyHit.Add(collision.gameObject);
    }
}
