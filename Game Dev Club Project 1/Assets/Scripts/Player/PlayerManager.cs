using UnityEngine;

public class PlayerManager : PersistentGenericSingleton<PlayerManager>
{
    [SerializeField] private PlayerData data;
    public PlayerData BaseData => data;
    public PlayerData RunTimeData => data;

    public GameObject Player { get; private set; }
    public Transform Transform { get; private set; }
    public Rigidbody Rb { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public Animator Animator { get; private set; }
    public PlayerAnimationManager AnimationManager { get; private set; }
    public CameraController Camera { get; private set; }
    public PlayerSpriteFlipper PlayerFlipper { get; private set; }

    public void RegisterPlayer(GameObject player)
    {
        Player = player;
        Transform = Player.transform;
        Rb = Player.GetComponent<Rigidbody>();
        Input = Player.GetComponent<PlayerInputHandler>();
        Animator = Player.GetComponentInChildren<Animator>();
        AnimationManager = Player.GetComponentInChildren<PlayerAnimationManager>();
        PlayerFlipper = player.GetComponentInChildren<PlayerSpriteFlipper>();
    }

    public void RegisterCamera(CameraController camera)
    {
        Camera = camera;
        Camera.RegisterPlayer(Transform);
    }
}
