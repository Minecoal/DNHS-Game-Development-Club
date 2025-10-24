using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerState currentState = PlayerState.Idle;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelAmount = 3f;
    [SerializeField] private float decelAmount = 3f;
    private Rigidbody rb;
    private Animator anim;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
    void Start()
    {

    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        if (moveInput.sqrMagnitude > 0.01f && (currentState == PlayerState.Moving || currentState == PlayerState.Idle))
            ChangeState(PlayerState.Moving);
        else
            ChangeState(PlayerState.Idle);

    }

    private void FixedUpdate()
    {
        if (currentState == PlayerState.Moving)
        {
            Run();
        }
        else
        {
            Decelerate();
        }
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState)
            return;

        //exit
        switch (currentState)
        {
            case PlayerState.Idle:
                ExitIdle();
                break;
            case PlayerState.Moving:
                ExitMoving();
                break;
            case PlayerState.BasicAttack:
                ExitBaasicAttack();
                break;

        }

        currentState = newState;

        //enter
        switch (currentState)
        {
            case PlayerState.Idle:
                EnterIdle();
                break;
            case PlayerState.Moving:
                EnterMoving();
                break;
            case PlayerState.BasicAttack:
                EnterBasicAttack();
                break;
        }
    }


    //enter/exit state logic
    void EnterIdle() { }
    void ExitIdle() { }

    void EnterMoving() {}
    void ExitMoving() { }

    void EnterBasicAttack() { }
    void ExitBaasicAttack() { }


    private void Run()
    {
        Vector3 targetSpeed = new Vector3(moveInput.x * moveSpeed, 0, moveInput.y * moveSpeed);

        //calculate accel/deccel
        Vector3 accelRate = new Vector3();
        accelRate.x = (Mathf.Abs(targetSpeed.x) > 0.01f) ? accelAmount : decelAmount;
        accelRate.z = (Mathf.Abs(targetSpeed.z) > 0.01f) ? accelAmount : decelAmount;

        //conserve momentum 
        if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed.x) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed.x) && Mathf.Abs(targetSpeed.x) > 0.01f)
        {
            accelRate.x = 0;
        }
        if (Mathf.Abs(rb.linearVelocity.z) > Mathf.Abs(targetSpeed.z) && Mathf.Sign(rb.linearVelocity.z) == Mathf.Sign(targetSpeed.z) && Mathf.Abs(targetSpeed.z) > 0.01f)
        {
            accelRate.z = 0;
        }


        Vector3 speedDiff = new Vector3(targetSpeed.x - rb.linearVelocity.x, 0, targetSpeed.z - rb.linearVelocity.z);
        Vector3 movement = new Vector3(speedDiff.x * accelRate.x, 0, speedDiff.z * accelRate.z);

        rb.AddForce(movement.x * Vector3.right, ForceMode.Force);
        rb.AddForce(movement.z * new Vector3(0,0,1), ForceMode.Force);
    }

    private void Decelerate()
    {
        //Vector2 speedDiff = new Vector2(-rb.linearVelocity.x, -rb.linearVelocity.y);
        Vector3 movement = new Vector3(-rb.linearVelocity.x * decelAmount, 0, -rb.linearVelocity.z * decelAmount);

        rb.AddForce(movement.x * Vector2.right, ForceMode.Force);
        rb.AddForce(movement.z * Vector3.forward, ForceMode.Force);
    }

    public Vector2 GetInputVector()
    {
        return moveInput;
    }
}

public enum PlayerState
{
    Idle,
    Moving,
    BasicAttack,
    HeavyAttack,
    Dash
}
