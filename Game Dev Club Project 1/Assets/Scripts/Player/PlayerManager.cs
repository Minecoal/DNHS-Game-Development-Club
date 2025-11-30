using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private PlayerData data;
    public PlayerData BaseData => data;
    public PlayerData RunTimeData => data;

    public GameObject Player { get; private set; }
    public Transform Transform { get; private set; }
    public Rigidbody Rb { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public Animator Animator { get; private set; }
    public CameraController Camera { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Disable when parented to a DonDestoryOnLoad object
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    public void RegisterPlayer(GameObject player)
    {
        Player = player;
        Transform = Player.transform;
        Rb = Player.GetComponent<Rigidbody>();
        Input = Player.GetComponent<PlayerInputHandler>();
        Animator = Player.GetComponentInChildren<Animator>();
    }

    public void RegisterCamera(CameraController camera)
    {
        Camera = camera;
        Camera.RegisterPlayer(Transform);
    }
}
