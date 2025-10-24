using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform Target;
    [SerializeField] private float smoothing;
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector3 offset;

    void FixedUpdate()
    {
        if (transform.position != Target.position)
        {
            Vector3 targetPosition = new Vector3(Target.transform.position.x + offset.x, transform.position.y, Target.transform.position.z + offset.z);
            //targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            //targetPosition.z = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
        }
    }
}
