public interface IPlayerState
{
    public void Enter(PlayerStateMachine playerStateManager);
    public void Exit(PlayerStateMachine playerStateManager);
    public void Update(PlayerStateMachine playerStateManager);
    public void UpdateFixed(PlayerStateMachine playerStateManager);
}