using System;
using UnityEngine;

public class HitboxManager : PersistentGenericSingleton<HitboxManager>
{
    [SerializeField] private HitboxRegistry registry;
    private Transform container;

    protected override void Awake()
    {
        base.Awake();
        if (registry == null) Debug.LogWarning("missing hitbox registry in Hitbox Manager");
    }

    private Transform GetOrCreateContainer()
    {
        if (container == null)
        {
            container = new GameObject("Hitbox Container").transform;
        }
        return container;
    }

    public Builder CreateNewHitbox(HitboxType type)
    {
        GameObject prefab = registry.GetPrefab(type);
        HitboxData data = registry.GetData(type);

        if (!prefab || !data)
        {
            Debug.LogWarning($"Cannot spawn hitbox, prefab or data missing: {type}");
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
            return obj;
        }

        private void HandleOnHit(DamageResult result, DamageInfo info)
        {
            PlayerManager.Instance.Camera.ScreenShake(Mathf.Max(Mathf.Sqrt(info.Amount), 1f), 0.1f);
            
        }
    }

    
}