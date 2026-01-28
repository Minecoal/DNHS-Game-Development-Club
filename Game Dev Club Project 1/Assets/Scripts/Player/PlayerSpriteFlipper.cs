using UnityEngine;

public class PlayerSpriteFlipper : MonoBehaviour
{
    public bool isFacingRight = true;
    private bool canFlip = true;
    float scalex;

    void Awake()
    {
        scalex = transform.localScale.x;
        canFlip = true;
    }

    public void RegisterInputHandler(PlayerInputHandler input)
    {
        input.OnMove += Flip;
    }

    public void CanFlip(bool canFlip)
    {
        this.canFlip = canFlip;
    }
    
    private void Flip(Vector3 dir)
    {
        if (dir.x != 0 && canFlip)
        {
            Vector3 scale = transform.localScale;
            scale.x = (dir.x > 0) ? scalex : -scalex;
            isFacingRight = dir.x > 0;
            transform.localScale = scale;
        }
    }
}
