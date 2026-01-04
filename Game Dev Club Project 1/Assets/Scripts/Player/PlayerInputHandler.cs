using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector3 MoveInputNormalized { get; private set; }

    [SerializeField] private KeyCode attackButton = KeyCode.Z;
    [SerializeField] private KeyCode dashButton = KeyCode.X;

    public Action<Vector3> OnMove;
    public Action OnAttack;
    public Action OnDash;

    void Update()
    {
        // XZ plane: Horizontal -> X, Vertical -> Z
        Vector3 raw = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (raw.sqrMagnitude > 0.01f) raw.Normalize();
        MoveInputNormalized = raw;

        OnMove?.Invoke(raw);

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
