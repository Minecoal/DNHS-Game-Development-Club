using System.Collections.Generic;
using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private float lifetime = 0.2f;
    private float damage;
    private GameObject self;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>(); //HastSet for O(1) complexity

    public event Action<DamageResult, DamageInfo> OnHit; // hit target
    public event Action OnReady; // finish attacking

    public void ConfigureAndDestroy(float damage, GameObject self, float lifetime = 0.2f)
    {
        this.damage = damage;
        this.self = self;
        this.lifetime = lifetime;
        Destroy(gameObject, this.lifetime);
    }

    private void OnTriggerEnter(Collider collision)
    {
        IDamagable target = collision.GetComponent<IDamagable>();
        if (target == null) return;
        if (collision.gameObject == self) return;
        if (alreadyHit.Contains(collision.gameObject)) return;

        Vector3 hitPoint3 = collision.ClosestPoint(transform.position);

        DamageInfo info = new DamageInfo
        {
            Amount = damage,
            Source = self,
            HitPoint = hitPoint3,
            HitNormal = (transform.position - hitPoint3).normalized
        };

        DamageResult damageResult = target.ApplyDamage(in info);
        CreateDamageReading(damageResult, info);
        alreadyHit.Add(collision.gameObject);
        // notify listeners
        OnHit?.Invoke(damageResult, info);
    }

    private void CreateDamageReading(DamageResult result, DamageInfo info)
    {
        if (result == DamageResult.Damaged)
        {
            TextDisplayManager.New3D(info.HitPoint, 0.1f)
            .WithAutoDestroy(2f)
            .WithInitialText(info.Amount.ToString())
            .Build();
        } else
        {
            TextDisplayManager.New3D(info.HitPoint, 0.1f)
            .WithAutoDestroy(2f)
            .WithInitialText(result.ToString())
            .Build();
        }
        
    }

    private void OnDestroy()
    {
        OnReady?.Invoke();
    }
}
