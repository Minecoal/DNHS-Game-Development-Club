public interface IPlayerState
{
    public void Enter(PlayerStateManager playerStateManager);
    public void Exit(PlayerStateManager playerStateManager);
    public void Update(PlayerStateManager playerStateManager);
    public void UpdateFixed(PlayerStateManager playerStateManager);
}