using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    IPlayerState currentState;
    // pre create state instances to avoid constant allocations on state switches
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMovingState MovingState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }

    public Transform Player { get; private set; }
    public PlayerData Data { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public Rigidbody Rb { get; private set; }
    public Animator Animator { get; private set; }

    [SerializeField] private AttackData[] AttackDataList;
    [SerializeField] private Transform AttackAnchor;

    void Awake()
    {
        PlayerManager.Instance.RegisterPlayer(gameObject);
    }

    void Start()
    {
        Player = PlayerManager.Instance.Transform;
        Data = PlayerManager.Instance.RunTimeData;
        Input = PlayerManager.Instance.Input;
        Rb = PlayerManager.Instance.Rb;
        Animator = PlayerManager.Instance.Animator;

        // create and cache state instances
        IdleState = new PlayerIdleState();
        MovingState = new PlayerMovingState();
        AttackState = new PlayerAttackState(AttackDataList, AttackAnchor);

        // default
        currentState = IdleState;
        currentState.Enter(this);

        TextDisplayManager.NewUI(new Vector3(-700f, 400f, 0f), 1f)
            .WithTrackedProvider(() => currentState.ToString())
            .WithDraggable()
            .WithInitialText("Waiting for Input")
            .Build();
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