using System;
using UnityEngine;
public class MovementController : MonoBehaviour
{

    public PlayerState currentState = PlayerState.Idle;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelAmount = 3f;
    [SerializeField] private float decelAmount = 3f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;
    private TextDisplay playerStateDisplayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStateDisplayer = TextDisplayManager.New(new Vector2(-4.7f, 3.3f), 0.1f)
            .WithTrackedProvider(() => currentState.ToString())
            .WithDraggable()
            .Build();
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
        if(currentState == PlayerState.Moving)
        {
            Run();
        }
        else
        {
            Decelerate();
        }
        Rotate();
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

    void EnterMoving() { }
    void ExitMoving() { }

    void EnterBasicAttack() { }
    void ExitBaasicAttack() { }


    private void Run()
    {
        Vector2 targetSpeed = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);

        Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + targetSpeed, Color.green);
        Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + rb.linearVelocity, Color.yellow);

        //calculate accel/deccel
        Vector2 accelRate = new Vector2();
        accelRate.x = (Mathf.Abs(targetSpeed.x) > 0.01f) ? accelAmount : decelAmount;
        accelRate.y = (Mathf.Abs(targetSpeed.y) > 0.01f) ? accelAmount : decelAmount;

        //conserve momentum 
        if (Mathf.Abs(rb.linearVelocity.x) > Mathf.Abs(targetSpeed.x) && Mathf.Sign(rb.linearVelocity.x) == Mathf.Sign(targetSpeed.x) && Mathf.Abs(targetSpeed.x) > 0.01f)
        {
            accelRate.x = 0;
        }
        if (Mathf.Abs(rb.linearVelocity.y) > Mathf.Abs(targetSpeed.y) && Mathf.Sign(rb.linearVelocity.y) == Mathf.Sign(targetSpeed.y) && Mathf.Abs(targetSpeed.y) > 0.01f)
        {
            accelRate.y = 0;
        }


        Vector2 speedDiff = new Vector2(targetSpeed.x - rb.linearVelocity.x, targetSpeed.y - rb.linearVelocity.y);
        Vector2 movement = new Vector2(speedDiff.x * accelRate.x, speedDiff.y * accelRate.y);

        rb.AddForce(movement.x * Vector2.right, ForceMode2D.Force);
        rb.AddForce(movement.y * Vector2.up, ForceMode2D.Force);
    }

    private void Decelerate()
    {
        //Vector2 speedDiff = new Vector2(-rb.linearVelocity.x, -rb.linearVelocity.y);
        Vector2 movement = new Vector2(-rb.linearVelocity.x * decelAmount, -rb.linearVelocity.y * decelAmount);

        rb.AddForce(movement.x * Vector2.right, ForceMode2D.Force);
        rb.AddForce(movement.y * Vector2.up, ForceMode2D.Force);
    }

    private void Rotate(){
        Vector2 velocity = rb.linearVelocity;
        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
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
