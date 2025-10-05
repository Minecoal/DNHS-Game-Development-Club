using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public PlayerState currentState = PlayerState.Idle;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelAmount = 3f;
    [SerializeField] private float decelAmount = 3f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    
    

    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
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
        }
    }


    //enter/exit state logic
    void EnterIdle() 
    {
        Debug.Log("enter idle state");
    }
    void ExitIdle() { }

    void EnterMoving() 
    {
        Debug.Log("enter moving state");
    }
    void ExitMoving() { }


    private void Run()
    {
        Vector2 targetSpeed = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);

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
}

public enum PlayerState
{
    Idle,
    Moving
}
