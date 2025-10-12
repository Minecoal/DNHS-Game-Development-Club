using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private PlayerManager() { }

    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject player;
    private MovementController movementController;
    private AttackController attackController;

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

    }

    void Start()
    {
        movementController = player.GetComponent<MovementController>();
        attackController = player.GetComponent<AttackController>();
    }

    void Update()
    {
        moveInput = movementController.GetInputVector();
    }

    public void DamagePlayer()
    {

    }
}
