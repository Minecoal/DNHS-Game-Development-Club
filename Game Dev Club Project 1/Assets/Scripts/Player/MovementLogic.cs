using System;
using UnityEngine;
public class MovementLogic : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelAmount = 3f;
    [SerializeField] private float decelAmount = 3f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Public surface for state objects
    public Rigidbody2D Rb => rb;
    public float MoveSpeed => moveSpeed;
    public float AccelAmount => accelAmount;
    public float DecelAmount => decelAmount;

    // Called by Moving
    public void ApplyMovement(Vector2 input, float deltaTime)
    {
        Vector2 targetSpeed = new Vector2(input.x * moveSpeed, input.y * moveSpeed);

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

    // Called by Idle
    public void Decelerate()
    {
        Vector2 movement = new Vector2(-rb.linearVelocity.x * decelAmount, -rb.linearVelocity.y * decelAmount);
        rb.AddForce(movement.x * Vector2.right, ForceMode2D.Force);
        rb.AddForce(movement.y * Vector2.up, ForceMode2D.Force);
    }
}