using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    private PlayerContext playerContext;

    private GameObject playerGO;
    private PlayerData data;
    private PlayerInputHandler input;
    private Rigidbody rb;
    private PlayerAnimationManager animationManager;
    private PlayerSpriteFlipper playerFlipper;

    [SerializeField] private GameObject defaultWeaponGO;
    [SerializeField] private Transform attackAnchor;

    void Awake()
    {
        PlayerManager.Instance.RegisterPlayer(gameObject);
    }

    void Start()
    {
        playerStateMachine = new PlayerStateMachine();
        playerGO = gameObject;
        data = PlayerManager.Instance.RunTimeData;
        input = PlayerManager.Instance.Input;
        rb = PlayerManager.Instance.Rb;
        animationManager = PlayerManager.Instance.AnimationManager;
        playerFlipper = PlayerManager.Instance.PlayerFlipper;
        playerFlipper.RegisterInputHandler(input);

        
        playerContext = new PlayerContext(
            playerStateMachine,
            data,
            this,
            playerGO,
            input,
            rb,
            animationManager,
            attackAnchor,
            defaultWeaponGO.GetComponent<IWeapon>(),
            playerFlipper
        );

        playerStateMachine.Initialize(new PlayerIdleState(), playerContext);

        TextDisplayManager.NewUI(new Vector3(-700f, 400f, 0f), 1f)
            .WithTrackedProvider(() => playerStateMachine.GetStateName())
            .WithDraggable()
            .WithInitialText("Waiting for Input")
            .Build();
    }

    void Update()
    {
        playerStateMachine.Tick(playerContext, Time.deltaTime);
    }

    void FixedUpdate()
    {
        playerStateMachine.FixedTick(playerContext, Time.fixedDeltaTime);
        Decelerate(rb, data.decelAmount);
    }

    public void ApplyMovement(Vector3 input, Transform player, Rigidbody rb, PlayerData data)
    {
        Vector3 targetSpeed = new Vector3(input.x * data.moveSpeed, 0f, input.z * data.moveSpeed);

        Debug.DrawLine(player.transform.position, player.transform.position + targetSpeed, Color.green);
        Debug.DrawLine(player.transform.position, player.transform.position + rb.linearVelocity, Color.yellow);

        //calculate accel/deccel
        Vector3 accelRate = new Vector3();
        accelRate.x = (Mathf.Abs(targetSpeed.x) > 0.01f) ? data.accelAmount : data.decelAmount;
        accelRate.y = 0f;
        accelRate.z = (Mathf.Abs(targetSpeed.z) > 0.01f) ? data.accelAmount : data.decelAmount;

        //conserve momentum
        if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed.x) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed.x) && Mathf.Abs(targetSpeed.x) > 0.01f)
        {
            accelRate.x = 0;
        }
        if (Mathf.Abs(rb.linearVelocity.z) > Mathf.Abs(targetSpeed.z) && Mathf.Sign(rb.linearVelocity.z) == Mathf.Sign(targetSpeed.z) && Mathf.Abs(targetSpeed.z) > 0.01f)
        {
            accelRate.z = 0;
        }

        Vector3 speedDiff = new Vector3(targetSpeed.x - rb.linearVelocity.x, 0f, targetSpeed.z - rb.linearVelocity.z);
        Vector3 movement = new Vector3(speedDiff.x * accelRate.x, 0f, speedDiff.z * accelRate.z);

        rb.AddForce(movement.x * Vector3.right, ForceMode.Force);
        rb.AddForce(movement.z * Vector3.forward, ForceMode.Force);
    }

    public void Decelerate(Rigidbody rb, float decelAmount)
    {
        Vector3 movement = new Vector3(-rb.linearVelocity.x * decelAmount, 0f, -rb.linearVelocity.z * decelAmount);
        rb.AddForce(movement.x * Vector3.right, ForceMode.Force);
        rb.AddForce(movement.y * Vector3.up, ForceMode.Force);
        rb.AddForce(movement.z * Vector3.forward, ForceMode.Force);
    }
}
