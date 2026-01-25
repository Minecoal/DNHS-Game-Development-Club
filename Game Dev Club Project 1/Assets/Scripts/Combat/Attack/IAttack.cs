using UnityEngine;
using System.Collections;

public interface IAttack
{
    // Executes the attack. 'spawnOrigin' is the spwanpoint,
    // owner is the logical owner of the attack (eg., the player GameObject) used for damage attribution and collision ignore.
    public void ExecuteAttack(Transform spawnOrigin, Transform owner, Vector3 direction);
    public void CancelAttack();
    public bool IsAvailable();
    public IEnumerator ReadyAfterCooldown();
}
