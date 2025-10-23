using UnityEngine;

public interface IAttack
{
    // Executes the attack. 'spawnParent' is where any spawned objects should be parented (e.g., attackAnchor),
    // originator is the logical owner of the attack (eg., the player GameObject) used for damage attribution.
    public void ExecuteAttack(Transform spawnParent, Transform originator, Vector2 direction);
    public void CancelAttack();
    public bool IsAvailable();
}
