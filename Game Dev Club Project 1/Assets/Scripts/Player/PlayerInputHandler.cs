using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector3 MoveInput { get; private set; }

    [SerializeField] private KeyCode attackButton = KeyCode.Z;
    [SerializeField] private KeyCode dashButton = KeyCode.X;

    public Action AttackPressed;
    public Action OnDash;

    void Update()
    {
        // For 3D movement on the XZ plane: Horizontal -> X, Vertical -> Z
        Vector3 raw = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (raw.sqrMagnitude > 0.01f) raw.Normalize();
        MoveInput = raw;

        if (Input.GetKeyDown(attackButton))
        {
            AttackPressed?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnDash?.Invoke();
        }
    }
}
