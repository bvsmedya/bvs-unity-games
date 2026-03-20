using UnityEngine;

/// KURULUM:
/// 1. Bos sahne ac, 2D proje
/// 2. Hierarchy > 2D Object > Sprites > Triangle → adi 'Player'
///    - Rigidbody2D ekle (Gravity Scale = 0, Freeze Rotation Z)
///    - PolygonCollider2D ekle
///    - Tag: 'Player'
///    - Bu scripti ekle
/// 3. Bos GameObject olustur 'FirePoint' adi, Player'in icine koy
///    - Y pozisyonu: 0.6 (geminin burnu)
/// 4. Inspector'dan bulletPrefab ve firePoint bagla

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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Ekran sinirlarini hesapla
        Camera cam = Camera.main;
        screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));
    }

    private void Update()
    {
        // Hareket
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector2(h, v).normalized * moveSpeed;

        // Ekrandan cikmayi engelle
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -screenBounds.x + 0.5f, screenBounds.x - 0.5f);
        pos.y = Mathf.Clamp(pos.y, -screenBounds.y + 0.5f, screenBounds.y - 0.5f);
        transform.position = pos;

        // Ates - Space veya sol tikla
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
        // Kirmizi yanip sonme efekti
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
        else UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            TakeDamage(1);
            if (other.CompareTag("EnemyBullet")) Destroy(other.gameObject);
        }
    }
}