using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private PlayerManager() { }

    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject player;
    private PlayerController playerController;

    public float mouseSpeed { get; private set; }

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
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {

    }

    public void DamagePlayer()
    {

    }
}
