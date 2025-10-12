using UnityEngine;

public class AttackController : MonoBehaviour
{
    public AttackData[] attackDatas; // assign in inspector
    private IAttack[] actions;
    public Animator animator;

    private int bufferedInput = -1; // the index of the buffered input
    private float bufferTime = 0.2f;
    private float bufferTimestamp;

    [SerializeField] private KeyCode attackButton = KeyCode.Z;

    void Awake()
    {
        actions = new IAttack[attackDatas.Length];
        for (int i = 0; i < attackDatas.Length; i++)
        {
            if (attackDatas[i].isProjectile)
            {
                // actions[i] = new ProjectileAttack(attacks[i])
            } else
            {
                actions[i] = new MeleeAttack(attackDatas[i]);
            }
        }
    }

    void Update()
    {
        // example input handling
        if (Input.GetKeyDown(attackButton))
        {
            TryStartAttack(0);
        }

        // buffered input
        if (bufferedInput >= 0 && Time.time - bufferTimestamp <= bufferTime)
        {
            if (actions[bufferedInput].IsAvailable())
            {
                StartAttack(bufferedInput);
                bufferedInput = -1;
            }
        }
    }

    public void TryStartAttack(int index)
    {
        if (actions[index].IsAvailable())
        {
            StartAttack(index);
        }
        else
        {
            // buffer it
            bufferedInput = index;
            bufferTimestamp = Time.time;
        }
    }

    private void StartAttack(int index)
    {
        AttackData data = attackDatas[index];

        if (animator != null && !string.IsNullOrEmpty(data.animatorTrigger)){ }
            // set animator trigger

            // if using animation events to time the hit, don't execute here, otherwise call Execute immediately
        if (!data.useAnimationEvent)
        {
            actions[index].ExecuteAttack(transform, PlayerManager.Instance.moveInput);
            Debug.Log("Attack Executed");
        } else {
            Debug.Log(data.useAnimationEvent);
        }
    }

    // called from animation event
    public void OnAnimationAttack(int attackIndex)
    {
        actions[attackIndex].ExecuteAttack(transform, PlayerManager.Instance.moveInput);
    }
}