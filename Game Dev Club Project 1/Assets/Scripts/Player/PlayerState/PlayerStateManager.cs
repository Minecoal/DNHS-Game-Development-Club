using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    IPlayerState currentState;
    // pre-create state instances to avoid allocations on state switches
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMovingState MovingState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    
    public MovementLogic Movement { get; private set; }
    public AttackLogic Attack { get; private set; }
    public Animator Anim { get; private set; }
    public PlayerInputHandler Input { get; private set; }

    void Start()
    {
        Movement = PlayerManager.Instance.MovementLogic;
        Attack = PlayerManager.Instance.AttackLogic;
        Anim = PlayerManager.Instance.Animator;
        Input = PlayerManager.Instance.InputHandler;
        if (Attack != null)
        {
            Attack.OnAttackHit += HandleAttackHit;
        }
        // create and cache state instances
        IdleState = new PlayerIdleState();
        MovingState = new PlayerMovingState();
        AttackState = new PlayerAttackState();

        currentState = IdleState;
        currentState.Enter(this);
    }

    private void HandleAttackHit(int attackIndex, DamageResult result, DamageInfo info)
    {
        Debug.Log($"Attack {attackIndex} hit: {result} amount={info.Amount}");
    }

    void Update()
    {
        currentState.Update(this);
    }

    void FixedUpdate()
    {
        currentState.UpdateFixed(this);
    }

    public void SwitchState(IPlayerState state)
    {
        currentState.Exit(this);
        currentState = state;
        currentState.Enter(this);
    }
}