using UnityEngine;

public enum PlayerActionType { None, Move, Attack, Dash }

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerActionType QueuedAction { get; private set; }
    public Vector2 MoveInput { get; private set; }

    void Update()
    {
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        MoveInput.Normalize();

        if (Input.GetKeyDown(KeyCode.Z))
            QueueAction(PlayerActionType.Attack);
        else if (Input.GetKeyDown(KeyCode.X))
            QueueAction(PlayerActionType.Dash);
        else if (MoveInput.sqrMagnitude > 0.01f)
            QueueAction(PlayerActionType.Move);
        else
            QueueAction(PlayerActionType.None);
    }

    void QueueAction(PlayerActionType action) => QueuedAction = action;
}
