using UnityEngine;

public class PlayerSpriteFlipper : MonoBehaviour
{
    public bool isFacingRight = true;
    float scalex;
    PlayerInputHandler input;

    void Awake()
    {
        scalex = transform.localScale.x;
    }

    public void RegisterInputHandler( PlayerInputHandler input)
    {
        this.input = input;
        input.OnMove += Flip;
    }
    
    private void Flip(Vector3 dir)
    {
        
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = (dir.x > 0) ? scalex : -scalex;
            isFacingRight = dir.x > 0;
            transform.localScale = scale;
        }
        
    }
}
