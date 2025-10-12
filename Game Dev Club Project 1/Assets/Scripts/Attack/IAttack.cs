using UnityEngine;

public interface IAttack
{
    public void ExecuteAttack(Transform origin, Vector2 direction);
    public void CancelAttack();
    public bool IsAvailable();
}
