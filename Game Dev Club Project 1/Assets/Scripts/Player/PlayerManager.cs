using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private PlayerManager() { }

    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject player;
    private MovementLogic movementLogic;
    private AttackLogic attackLogic;
    private PlayerInputHandler inputHandler;
    
    public MovementLogic MovementLogic => movementLogic;
    public AttackLogic AttackLogic => attackLogic;
    public PlayerInputHandler InputHandler => InputHandler;
    public Animator Animator { get; private set; }
    public Vector2 moveInput { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        movementLogic = player.GetComponent<MovementLogic>();
        attackLogic = player.GetComponent<AttackLogic>();
    }

    void Start()
    {

    }

    void Update()
    {
        moveInput = inputHandler.MoveInput;
    }
}
