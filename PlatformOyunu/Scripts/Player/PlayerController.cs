using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlari")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    [Header("Zemin Kontrolu")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Can Sistemi")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invincibilityDuration = 1.5f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private int currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        rb.freezeRotation = true;
        rb.gravityScale = 3f;
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * 8f, 1f));
            if (invincibilityTimer <= 0) { isInvincible = false; spriteRenderer.color = Color.white; }
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        rb.linearVelocity = new Vector2(0, 8f);
        if (GameManager.Instance != null) GameManager.Instance.UpdateHealthUI(currentHealth);
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (GameManager.Instance != null) GameManager.Instance.GameOver();
        else UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (rb.linearVelocity.y < -0.1f)
            {
                Destroy(collision.gameObject);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.7f);
                if (GameManager.Instance != null) GameManager.Instance.AddScore(100);
            }
            else TakeDamage(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone")) Die();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); }
    }
}
