using UnityEngine;

public class PlayerContext
{
    public PlayerStateMachine StateMachine;
    public PlayerData Data;
    public Player Player;
    public GameObject PlayerGO;
    public PlayerInputHandler Input;
    public Rigidbody Rb;
    public PlayerAnimationManager AnimationManager;
    public Transform AttackAnchor;
    public IWeapon ActivePrimaryWeapon;
    public IWeapon ActiveSecondaryWeapon;
    public PlayerSpriteFlipper PlayerFlipper;

    public PlayerContext(
        PlayerStateMachine StateMachine,
        PlayerData Data, 
        Player Player,
        GameObject PlayerGO, 
        PlayerInputHandler Input, 
        Rigidbody Rb, 
        PlayerAnimationManager AnimationManager,
        Transform AttackAnchor,
        IWeapon ActivePrimaryWeapon,
        IWeapon ActiveSecondaryWeapon,
        PlayerSpriteFlipper PlayerFlipper)
    {
        this.PlayerGO = PlayerGO;
        this.Data = Data;
        this.Player = Player;
        this.StateMachine = StateMachine;
        this.Input = Input;
        this.Rb = Rb;
        this.AnimationManager = AnimationManager;
        this.AttackAnchor = AttackAnchor;
        this.ActivePrimaryWeapon = ActivePrimaryWeapon;
        this.ActiveSecondaryWeapon = ActiveSecondaryWeapon;
        this.PlayerFlipper = PlayerFlipper;
    }
}
