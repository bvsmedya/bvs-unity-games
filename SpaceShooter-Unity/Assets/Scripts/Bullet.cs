using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool isEnemyBullet = false;

    private void Awake()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null)
            sr.sprite = isEnemyBullet ? MakeCircle(new Color(1f, 0.3f, 0.3f)) : MakeCircle(Color.yellow);
    }

    private void Start()
    {
        float direction = isEnemyBullet ? -1f : 1f;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed * direction;
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isEnemyBullet)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerShip>()?.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyShip>()?.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    public static Sprite MakeCircle(Color color, int size = 32)
    {
        var tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Bilinear;
        float c = size / 2f;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(c, c));
                tex.SetPixel(x, y, d <= c - 1 ? color : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}