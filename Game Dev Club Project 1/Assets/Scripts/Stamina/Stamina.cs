using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;

    private Player player;
    private PlayerData playerData;

    private float currentStamina;
    private float targetValue;
    private float rechargeTimer;

    private float lerpSpeed = 15f;


    public void InitializeStaminaBar(Player player)
    {
        this.player = player;
        playerData = player.playerContext.Data;

        targetValue = playerData.maxStamina;
        currentStamina = playerData.maxStamina;
        staminaBar.maxValue = playerData.maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerData == null)
            return;

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            UseStamina(50);
        }

        currentStamina = Mathf.Lerp(currentStamina, targetValue, Time.deltaTime * lerpSpeed);
        staminaBar.value = currentStamina;
    }

    private void FixedUpdate()
    {
        if (playerData == null)
            return;

        RechargeBar();
    }

    public void UseStamina(float amountToUse)
    {
        targetValue -= amountToUse;

        rechargeTimer = playerData.staminaRechargeDelay;
    }

    public bool CanUseStamina(float amountToUse)
    {
        return currentStamina >= amountToUse;
    }

    private void RechargeBar()
    {
        if (targetValue >= playerData.maxStamina)
        {
            targetValue = playerData.maxStamina;
            return;
        }


        if (rechargeTimer > 0f)
        {
            rechargeTimer -= Time.deltaTime;
            return;
        }

        targetValue += (playerData.staminaRechargeRate / 60);

        if (Mathf.Abs(currentStamina - targetValue) < 0.05f)
            currentStamina = targetValue;
    }

    public void UpdateUI()
    {
        staminaBar.maxValue = playerData.maxStamina;
    }
}
