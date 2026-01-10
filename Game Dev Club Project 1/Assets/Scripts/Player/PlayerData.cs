using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header(header: "Movement")]
    public float moveSpeed = 8;
    public float accelAmount = 12;
    public float decelAmount = 9;


    [Header(header: "Dash")]
    public float dashForce = 6;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
}
