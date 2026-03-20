using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;
    [SerializeField] private bool floatEffect = true;
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatSpeed = 2f;

    private Vector3 startPosition;

    private void Start() { startPosition = transform.position; }

    private void Update()
    {
        if (floatEffect) { float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude; transform.position = new Vector3(startPosition.x, newY, startPosition.z); }
        transform.Rotate(0, 0, 100f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue); Destroy(gameObject); }
    }
}