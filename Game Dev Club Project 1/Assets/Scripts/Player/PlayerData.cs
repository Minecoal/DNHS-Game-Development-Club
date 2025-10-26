using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header(header: "Movement")]
    public float moveSpeed = 8;
    public float accelAmount = 12;
    public float decelAmount = 9;
}
