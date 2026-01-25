using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    public PlayerContext playerContext { get; private set; }

    [SerializeField] private Transform attackAnchor;

    private void Awake()
    {
        PlayerManager.Instance.RegisterPlayer(gameObject);
    }

    private void Start()
    {
        playerStateMachine = new PlayerStateMachine();
        var pm = PlayerManager.Instance;

        pm.PlayerFlipper.RegisterInputHandler(pm.Input);

        playerContext = new PlayerContext(
            playerStateMachine,
            pm.RunTimeData,
            this,
            pm.Player,
            pm.Input,
            pm.Rb,
            pm.AnimationManager,
            attackAnchor,
            null,
            null,
            pm.PlayerFlipper
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
        Decelerate(playerContext.Rb, playerContext.Data.decelAmount);
    }

    public void ApplyMovement(Vector3 input, Transform player, Rigidbody rb, PlayerData data)
    {
        Vector3 targetSpeed = new Vector3(input.x * data.moveSpeed, 0f, input.z * data.moveSpeed);

        Debug.DrawLine(player.position, player.position + targetSpeed, Color.green);
        Debug.DrawLine(player.position, player.position + rb.linearVelocity, Color.yellow);

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

    public void ApplyForce(float inputForce, PlayerContext context)
    {
        Vector3 playerDir = GetPlayerDirNormalized(context);
        context.Rb.AddForce(inputForce * playerDir, ForceMode.Impulse);
    }

    public Vector3 GetPlayerDirNormalized(PlayerContext context)
    {
        if(context.Input.MoveInputNormalized.sqrMagnitude > 0.01f)
        {
            return context.Input.MoveInputNormalized;
        } else {
            return context.PlayerFlipper.isFacingRight ? Vector3.right : Vector3.left;
        }
    }

    public void SetPrimaryWeapon(GameObject weapon)
    {
        if (weapon != null){
            playerContext.ActivePrimaryWeapon = weapon.GetComponent<IWeapon>();
            return;
        }
        playerContext.ActivePrimaryWeapon = null;
    }
    
    public void SetSecondaryWeapon(GameObject weapon)
    {
        if (weapon != null){
            playerContext.ActiveSecondaryWeapon = weapon.GetComponent<IWeapon>();
            return;
        }
        playerContext.ActiveSecondaryWeapon = null;
    }
}
