using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    Rigidbody2D rb;
    float scalex;
    void Awake()
    {
        scalex = transform.localScale.x;
    }
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }
    void Update()
    {
        Flip();
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = (rb.linearVelocityX > 0) ? scalex : -scalex;
        transform.localScale = scale;   
    }

}
