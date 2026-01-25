using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem particlePrefab;
    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private int poolExpansionSize = 5;

    private Queue<ParticleSystem> pool = new Queue<ParticleSystem>();
    private List<ParticleSystem> activeParticles = new List<ParticleSystem>();

    void Start()
    {
        InitializePool();
    }

    void Update()
    {
        // Check for stopped particles and return to pool
        for (int i = activeParticles.Count - 1; i >= 0; i--)
        {
            ParticleSystem ps = activeParticles[i];
            if (!ps.isPlaying)
            {
                ReturnToPool(ps);
                activeParticles.RemoveAt(i);
            }
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateParticleInPool();
        }
    }

    private void CreateParticleInPool()
    {
        ParticleSystem ps = Instantiate(particlePrefab, transform);
        ps.gameObject.SetActive(false);
        pool.Enqueue(ps);
    }

    private ParticleSystem GetParticleFromPool()
    {
        if (pool.Count == 0)
        {
            // Expand pool if empty
            for (int i = 0; i < poolExpansionSize; i++)
            {
                CreateParticleInPool();
            }
        }
        ParticleSystem ps = pool.Dequeue();
        ps.gameObject.SetActive(true);
        activeParticles.Add(ps);
        return ps;
    }

    private void ReturnToPool(ParticleSystem ps)
    {
        ps.Stop();
        ps.Clear();
        ps.gameObject.SetActive(false);
        pool.Enqueue(ps);
    }

    public void PlayParticle(Vector3 position)
    {
        PlayParticle(position, Quaternion.identity);
    }

    public void PlayParticle(Vector3 position, Quaternion rotation)
    {
        ParticleSystem ps = GetParticleFromPool();
        ps.transform.position = position;
        ps.transform.rotation = rotation;
        ps.Play();
    }

    public void PlayParticle(Transform parent)
    {
        PlayParticle(parent.position, parent.rotation);
    }

    public void StopParticle(ParticleSystem ps)
    {
        if (activeParticles.Contains(ps))
        {
            ps.Stop();
        }
    }

    public void StopAllParticles()
    {
        foreach (ParticleSystem ps in activeParticles)
        {
            ps.Stop();
        }
    }
}
