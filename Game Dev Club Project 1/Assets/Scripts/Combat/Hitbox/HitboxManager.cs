using System;
using UnityEditor.SearchService;
using UnityEngine;

public class HitboxManager : PersistentGenericSingleton<HitboxManager>
{
    private Transform container;

    protected override void Awake()
    {
        base.Awake();
    }

    private Transform GetOrCreateContainer()
    {
        if (container == null)
        {
            container = new GameObject("Hitbox Container").transform;
        }
        return container;
    }

    public Builder CreateNewHitbox(HitboxData data)
    {
        GameObject prefab = data.prefab;

        if (!prefab || !data)
        {
            Debug.LogWarning($"Cannot spawn hitbox, prefab or data missing");
            return null;
        }
        return new Builder(prefab, data);
    }

    public class Builder
    {
        public readonly GameObject prefab;
        private HitboxData data;
        private Transform spawnPoint;


        public Builder(GameObject prefab, HitboxData data)
        {
            this.prefab = prefab;
            this.data = data;
        }

        public Builder WithSpawnPoint(Transform spawnPoint)
        {
            this.spawnPoint = spawnPoint;
            return this;
        }

        public GameObject Build(GameObject self, float damage)
        {
            Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;
            Quaternion rot = spawnPoint ? spawnPoint.rotation : Quaternion.identity;
            Vector3 scale = spawnPoint ? spawnPoint.lossyScale : Vector3.one; // fix this later to not use scaling

            GameObject obj = Instantiate(prefab, pos, rot);
            obj.transform.localScale= scale;
            obj.transform.SetParent(HitboxManager.Instance.GetOrCreateContainer(), true);

            // if (spawnPoint) obj.transform.SetParent(spawnPoint);
            Hitbox hitbox = obj.GetComponent<Hitbox>();
            if (hitbox == null){
                hitbox = obj.AddComponent<Hitbox>();
            }
            hitbox.OnHit += HandleOnHit;
            hitbox.ConfigureAndDestroy(self, data, damage);

            Animator animator = obj.GetComponentInChildren<Animator>();
            if (animator != null) animator.Play(0, -1, 0f); // players default animation clip;
            return obj;
        }

        private void HandleOnHit(DamageResult result, DamageInfo info)
        {

        }
    }

    
}