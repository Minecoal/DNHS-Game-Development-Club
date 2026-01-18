// using System;
// using UnityEngine;

// public class PlayerInputHandler : MonoBehaviour
// {
//     public Vector3 MoveInputRaw { get; private set; }
//     public Vector3 MoveInputNormalized { get; private set; }

//     [SerializeField] private KeyCode attackButton = KeyCode.Z;
//     [SerializeField] private KeyCode dashButton = KeyCode.X;

//     public Action<Vector3> OnMove;
//     public Action OnAttack;
//     public Action OnDash;

//     void Update()
//     {
//         
//         MoveInputRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
//         if (MoveInputRaw.sqrMagnitude > 0.01f) MoveInputRaw.Normalize();
//         MoveInputNormalized = MoveInputRaw;

//         if (MoveInputRaw.magnitude != 0)
//         {
//             OnMove?.Invoke(MoveInputRaw);
//         }

//         if (Input.GetKeyDown(attackButton))
//         {
//             OnAttack?.Invoke();
//         }
//         if (Input.GetKeyDown(dashButton))
//         {
//             OnDash?.Invoke();
//         }
//     }
// }

using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector3 MoveInputRaw { get; private set; }
    public Vector3 MoveInputNormalized { get; private set; }

    [SerializeField] private KeyCode primaryAttackButton = KeyCode.Z;
    [SerializeField] private KeyCode secondaryAttackButton = KeyCode.V;
    [SerializeField] private KeyCode dashButton = KeyCode.X;

    [Header("Input Buffering")]
    [SerializeField] private float attackBufferTime = 0.25f;
    [SerializeField] private float dashBufferTime = 0.25f;

    private float lastPrimaryAttackPressedTime = -1f;
    private float lastSecondaryAttackPressedTime = -1f;
    private float lastDashPressedTime = -1f;

    public Action<Vector3> OnMove;
    public Action OnPrimaryAttack;
    public Action OnSecondaryAttack;
    public Action OnDash;

    void Update()
    {
        // XZ plane: Horizontal -> X, Vertical -> Z
        MoveInputRaw = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        );

        if (MoveInputRaw.sqrMagnitude > 0.01f){
            MoveInputNormalized = MoveInputRaw.normalized;
        } else {
            MoveInputNormalized = Vector3.zero;
        }
        if (MoveInputRaw.sqrMagnitude > 0.01f)
            OnMove?.Invoke(MoveInputRaw);

        // Primary Attack
        if (Input.GetKeyDown(primaryAttackButton))
        {
            lastPrimaryAttackPressedTime = Time.time;
            OnPrimaryAttack?.Invoke();
        }
        
        // Secondary Attack
        if (Input.GetKeyDown(secondaryAttackButton))
        {
            lastSecondaryAttackPressedTime = Time.time;
            OnSecondaryAttack?.Invoke();
        }

        // Dash
        if (Input.GetKeyDown(dashButton))
        {
            lastDashPressedTime = Time.time;
            OnDash?.Invoke();
        }
    }

    #region buffer consumption
    public bool ConsumePrimaryAttack()
    {
        if (lastPrimaryAttackPressedTime < 0f) return false;

        if (Time.time - lastPrimaryAttackPressedTime > attackBufferTime)
        {
            lastPrimaryAttackPressedTime = -1f;
            return false;
        }

        lastPrimaryAttackPressedTime = -1f;
        return true;
    }
    public bool ConsumeSecondaryAttack()
    {
        if (lastSecondaryAttackPressedTime < 0f) return false;

        if (Time.time - lastSecondaryAttackPressedTime > attackBufferTime)
        {
            lastSecondaryAttackPressedTime = -1f;
            return false;
        }

        lastSecondaryAttackPressedTime = -1f;
        return true;
    }

    public bool ConsumeDash()
    {
        if (lastDashPressedTime < 0f) return false;

        if (Time.time - lastDashPressedTime > dashBufferTime)
        {
            lastDashPressedTime = -1f;
            return false;
        }

        lastDashPressedTime = -1f;
        return true;
    }
    #endregion
}
