using UnityEngine;

/// PARALLAX UZAY ARKA PLANI
/// KURULUM:
/// 1. Main Camera'ya bu scripti ekle
/// 2. Sahneye birden fazla Quad veya Sprite koy (yildiz gorunumu)
///    - Stars Layer 1 (hizli), Stars Layer 2 (yavas)
/// Veya sadece kameranin arka plan rengini siyah yap, hazir!

public class StarBackground : MonoBehaviour
{
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private int starCount = 80;
    [SerializeField] private float scrollSpeed = 1.5f;
    [SerializeField] private float spawnWidth = 9f;
    [SerializeField] private float spawnHeight = 12f;

    private Transform[] stars;
    private float bottomY;

    private void Start()
    {
        if (starPrefab == null) return;
        bottomY = -spawnHeight / 2f;
        stars = new Transform[starCount];
        for (int i = 0; i < starCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnWidth, spawnWidth),
                Random.Range(-spawnHeight / 2f, spawnHeight / 2f),
                1f);
            stars[i] = Instantiate(starPrefab, pos, Quaternion.identity).transform;
            // Rastgele boyut
            float s = Random.Range(0.05f, 0.15f);
            stars[i].localScale = new Vector3(s, s, 1);
        }
    }

    private void Update()
    {
        if (stars == null) return;
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] == null) continue;
            stars[i].position += Vector3.down * scrollSpeed * Time.deltaTime;
            if (stars[i].position.y < bottomY)
            {
                float x = Random.Range(-spawnWidth, spawnWidth);
                stars[i].position = new Vector3(x, spawnHeight / 2f, 1f);
            }
        }
    }
}