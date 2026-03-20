using UnityEngine;

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

    private void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
            sr.sprite = MakeTriangle(new Color(1f, 0.2f, 0.2f));
    }

    private void Start()
    {
        startX = transform.position.x;
        nextFireTime = Time.time + Random.Range(minFireDelay, maxFireDelay);
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        float newX = startX + Mathf.Sin(timeAlive * zigzagFrequency) * zigzagAmplitude;
        float newY = transform.position.y - moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, newY, 0);
        if (transform.position.y < -7f) Destroy(gameObject);
        if (Time.time >= nextFireTime) { Shoot(); nextFireTime = Time.time + Random.Range(minFireDelay, maxFireDelay); }
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
        Destroy(gameObject);
    }

    public static Sprite MakeTriangle(Color color, int size = 48)
    {
        var tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Bilinear;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float nx = x / (float)size;
                float ny = y / (float)size;
                // Asagi bakan ucgen (ters)
                bool inside = (1f - ny) >= Mathf.Abs(nx - 0.5f) * 2f;
                tex.SetPixel(x, y, inside ? color : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}