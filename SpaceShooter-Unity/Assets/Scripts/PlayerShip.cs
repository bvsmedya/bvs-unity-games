using UnityEngine;

public class PlayerShip : MonoBehaviour
{
    [Header("Hareket")]
    [SerializeField] private float moveSpeed = 8f;
    [Header("Ates")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    [Header("Can")]
    [SerializeField] private int maxHealth = 3;

    private float nextFireTime;
    private int currentHealth;
    private Rigidbody2D rb;
    private Vector2 screenBounds;
    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
            sr.sprite = MakeTriangle(Color.cyan);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        Camera cam = Camera.main;
        screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));
        if (GameManager.Instance != null) GameManager.Instance.UpdateHealthUI(currentHealth);
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(h, v).normalized * moveSpeed;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -screenBounds.x + 0.5f, screenBounds.x - 0.5f);
        pos.y = Mathf.Clamp(pos.y, -screenBounds.y + 0.5f, screenBounds.y - 0.5f);
        transform.position = pos;

        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(HitFlash());
        if (GameManager.Instance != null) GameManager.Instance.UpdateHealthUI(currentHealth);
        if (currentHealth <= 0) Die();
    }

    private System.Collections.IEnumerator HitFlash()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    private void Die()
    {
        if (GameManager.Instance != null) GameManager.Instance.GameOver();
        else UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            TakeDamage(1);
            if (other.CompareTag("EnemyBullet")) Destroy(other.gameObject);
        }
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
                // Yukari bakan ucgen
                bool inside = ny >= Mathf.Abs(nx - 0.5f) * 2f;
                tex.SetPixel(x, y, inside ? color : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}