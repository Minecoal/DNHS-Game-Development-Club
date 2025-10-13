using UnityEngine;

public class AttackController : MonoBehaviour
{
    public AttackData[] attackDatas; // assign in inspector
    private IAttack[] actions;
    public Animator animator;
    [SerializeField] private Transform attackAnchor;

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
        if (Input.GetKey(attackButton))
        {
            TryStartAttack(0);
        }

        // buffered input
        if (bufferedInput >= 0 && Time.time - bufferTimestamp <= bufferTime)
        {
            if (actions[bufferedInput].IsAvailable())
            {
                StartAttack(bufferedInput);
            }
        } else
        {
            bufferedInput = -1;
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
            // spawn hit/effects at attackAnchor, but pass the player transform as the logical originator
            Transform spawnParent = attackAnchor != null ? attackAnchor : transform;
            actions[index].ExecuteAttack(spawnParent, transform, PlayerManager.Instance.moveInput);
        }
    }

    // called from animation event
    public void OnAnimationAttack(int attackIndex)
    {
        Transform spawnParent = attackAnchor != null ? attackAnchor : transform;
        actions[attackIndex].ExecuteAttack(spawnParent, transform, PlayerManager.Instance.moveInput);
    }
}