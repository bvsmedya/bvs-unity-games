using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
    [SerializeField] private bool useBounds = false;
    [SerializeField] private float minX = -10f, maxX = 50f, minY = -5f, maxY = 20f;

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
        if (useBounds) { smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX); smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY); }
        smoothed.z = offset.z;
        transform.position = smoothed;
    }
}