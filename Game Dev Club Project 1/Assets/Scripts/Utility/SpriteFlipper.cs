using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    Rigidbody rb;
    float scalex;
    void Awake()
    {
        scalex = transform.localScale.x;
    }
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }
    void Update()
    {
        Flip();
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = (rb.linearVelocity.x > 0) ? scalex : -scalex;
        transform.localScale = scale;   
    }

}
