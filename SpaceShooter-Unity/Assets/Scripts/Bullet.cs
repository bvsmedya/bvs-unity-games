using UnityEngine;

/// KURULUM (Bullet Prefab):
/// 1. 2D Object > Sprites > Circle olustur, kucult (Scale 0.2, 0.2)
///    - Sari veya mavi renk ver (oyuncu mermisi)
/// 2. Rigidbody2D ekle (Gravity Scale = 0)
/// 3. CircleCollider2D ekle, Is Trigger ISARETLI
/// 4. Bu scripti ekle
/// 5. Prefab olarak kaydet → 'PlayerBullet' adi
/// 6. EnemyBullet icin ayni seyi yap, kirmizi renk, tag='EnemyBullet'

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool isEnemyBullet = false;

    private void Start()
    {
        // Yukari (oyuncu) veya asagi (dusman) git
        float direction = isEnemyBullet ? -1f : 1f;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * speed * direction;

        // 3 saniye sonra yok ol (ekran disi kacan mermiler)
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
}