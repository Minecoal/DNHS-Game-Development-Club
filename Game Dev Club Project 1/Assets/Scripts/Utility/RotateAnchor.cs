using UnityEngine;

public class RotateAnchor : MonoBehaviour
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }
    void Update()
    {
        Rotate();
    }

    private void Rotate(){
        Vector3 velocity = rb.linearVelocity;
        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
