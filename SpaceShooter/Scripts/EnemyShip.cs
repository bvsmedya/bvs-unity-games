using UnityEngine;

/// KURULUM (Enemy Prefab):
/// 1. 2D Object > Sprites > Triangle olustur
///    - Rotate 180 derece (asagi baksın)
///    - Kirmizi renk ver
///    - Scale: 0.8, 0.8
/// 2. Rigidbody2D (Gravity Scale 0, Freeze Rotation Z)
/// 3. PolygonCollider2D
/// 4. Tag: 'Enemy'
/// 5. Bu scripti ekle
/// 6. Prefab olarak kaydet

public class EnemyShip : MonoBehaviour
{
    [Header("Hareket")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float zigzagAmplitude = 2f;
    [SerializeField] private float zigzagFrequency = 1.5f;

    [Header("Can ve Skor")]
    [SerializeField] private int health = 2;
    [SerializeField] private int scoreValue = 100;

    [Header("Ates")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float minFireDelay = 1f;
    [SerializeField] private float maxFireDelay = 3f;

    private float startX;
    private float timeAlive;
    private float nextFireTime;

    private void Start()
    {
        startX = transform.position.x;
        nextFireTime = Time.time + Random.Range(minFireDelay, maxFireDelay);
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        // Asagi in + zigzag hareketi
        float newX = startX + Mathf.Sin(timeAlive * zigzagFrequency) * zigzagAmplitude;
        float newY = transform.position.y - moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, newY, 0);

        // Ekranin altina dustuyse yok ol
        if (transform.position.y < -7f) Destroy(gameObject);

        // Ates
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + Random.Range(minFireDelay, maxFireDelay);
        }
    }

    private void Shoot()
    {
        if (enemyBulletPrefab == null) return;
        Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        StartCoroutine(HitFlash());
        if (health <= 0) Die();
    }

    private System.Collections.IEnumerator HitFlash()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;
        Color original = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        sr.color = original;
    }

    private void Die()
    {
        if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);
        // Patlama efekti buraya eklenebilir
        Destroy(gameObject);
    }
}