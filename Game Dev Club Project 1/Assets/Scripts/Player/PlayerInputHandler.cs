using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector3 MoveInputRaw { get; private set; }
    public Vector3 MoveInputNormalized { get; private set; }

    [SerializeField] private KeyCode attackButton = KeyCode.Z;
    [SerializeField] private KeyCode dashButton = KeyCode.X;

    public Action<Vector3> OnMove;
    public Action OnAttack;
    public Action OnDash;

    void Update()
    {
        // XZ plane: Horizontal -> X, Vertical -> Z
        MoveInputRaw = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (MoveInputRaw.sqrMagnitude > 0.01f) MoveInputRaw.Normalize();
        MoveInputNormalized = MoveInputRaw;

        if (MoveInputRaw.magnitude != 0)
        {
            OnMove?.Invoke(MoveInputRaw);
        }

        if (Input.GetKeyDown(attackButton))
        {
            OnAttack?.Invoke();
        }
        if (Input.GetKeyDown(dashButton))
        {
            OnDash?.Invoke();
        }
    }
}
