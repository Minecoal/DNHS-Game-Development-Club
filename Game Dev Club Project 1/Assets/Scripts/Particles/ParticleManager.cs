using UnityEngine;

public class ParticleManager : GenericSingleton<ParticleManager>
{
    [SerializeField] private ParticlePlayer hitParticles;
    [SerializeField] private ParticlePlayer deathParticles;

    public ParticlePlayer HitParticles => hitParticles;
    public ParticlePlayer DeathParticles => deathParticles;
}