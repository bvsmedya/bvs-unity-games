using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool useLocalPoints = true;
    [SerializeField] private float leftDistance = 3f;
    [SerializeField] private float rightDistance = 3f;
    [SerializeField] private Transform pointA, pointB;

    private Vector2 targetPoint, localPointA, localPointB;
    private bool movingToB = true;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (useLocalPoints)
        {
            localPointA = new Vector2(transform.position.x - leftDistance, transform.position.y);
            localPointB = new Vector2(transform.position.x + rightDistance, transform.position.y);
        }
        else { localPointA = pointA.position; localPointB = pointB.position; }
        targetPoint = localPointB;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(targetPoint.x, transform.position.y), moveSpeed * Time.deltaTime);
        if (Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(targetPoint.x, 0)) < 0.1f)
        {
            if (movingToB) { targetPoint = localPointA; movingToB = false; }
            else { targetPoint = localPointB; movingToB = true; }
        }
        spriteRenderer.flipX = targetPoint.x <= transform.position.x;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (Application.isPlaying) { Gizmos.DrawLine(localPointA, localPointB); }
        else if (useLocalPoints)
        {
            Vector2 a = new Vector2(transform.position.x - leftDistance, transform.position.y);
            Vector2 b = new Vector2(transform.position.x + rightDistance, transform.position.y);
            Gizmos.DrawLine(a, b);
        }
    }
}