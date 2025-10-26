using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    [SerializeField] private KeyCode attackButton = KeyCode.Z;
    [SerializeField] private KeyCode dashButton = KeyCode.X;

    public Action AttackPressed;
    public Action OnDash;

    void Update()
    {
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        MoveInput.Normalize();

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
