using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private HitboxData hitboxData;
    private float damage; // supplyed from Attack Data
    private GameObject self;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    public event Action<DamageResult, DamageInfo> OnHit; // hit target
    public event Action OnReady; // finish attacking

    public void ConfigureAndDestroy(GameObject self, HitboxData data, float damage)
    {
        this.damage = damage;
        hitboxData = data;
        this.self = self;
        StartCoroutine(LifetimeCoroutine());
        
    }

    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(hitboxData.lifetime);
        Finish();
    }

    public void Cancel()
    {
        Finish();
    }

    private void Finish()
    {
        OnReady?.Invoke();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision == null) return;

        // Ignore self (including child colliders)
        if (self != null)
        {
            Transform selfTransform = self.transform;
            if (collision.gameObject == self || collision.transform.IsChildOf(selfTransform))
                return;
        }

        if (alreadyHit.Contains(collision.gameObject)) return;

        // Try to find IDamagable on the collider, its children, its parents, or attached rigidbody
        IDamagable target = collision.GetComponent<IDamagable>();
        if (target == null) target = collision.GetComponentInChildren<IDamagable>();
        if (target == null && collision.attachedRigidbody != null) target = collision.attachedRigidbody.GetComponent<IDamagable>();
        if (target == null) target = collision.GetComponentInParent<IDamagable>();
        if (target == null) return;

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
}
