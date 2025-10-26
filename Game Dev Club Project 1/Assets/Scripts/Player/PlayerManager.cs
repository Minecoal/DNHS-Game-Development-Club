using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private PlayerData data;
    public PlayerData Data => data;

    public GameObject Player { get; private set; }
    public Transform Transform { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public PlayerInputHandler Input { get; private set; }
    public Animator Animator { get; private set; }
    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        MoveInput = Input.MoveInput;
    }

    public void RegisterPlayer(GameObject player){
        Player = player;
        Transform = Player.transform;
        Rb = Player.GetComponent<Rigidbody2D>();
        Input = Player.GetComponent<PlayerInputHandler>();
        Animator = Player.GetComponentInChildren<Animator>();
    }
}
