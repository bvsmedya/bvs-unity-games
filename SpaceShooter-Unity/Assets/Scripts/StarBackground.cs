using UnityEngine;

public class StarBackground : MonoBehaviour
{
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private int starCount = 80;
    [SerializeField] private float scrollSpeed = 1.5f;
    [SerializeField] private float spawnWidth = 9f;
    [SerializeField] private float spawnHeight = 12f;

    private Transform[] stars;
    private float bottomY;

    // starPrefab yoksa runtime'da basit beyaz nokta yarat
    private GameObject CreateStarObject()
    {
        var go = new GameObject("Star");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Bullet.MakeCircle(Color.white, 8);
        sr.sortingOrder = -1;
        return go;
    }

    private void Start()
    {
        bottomY = -spawnHeight / 2f;
        stars = new Transform[starCount];
        for (int i = 0; i < starCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-spawnWidth, spawnWidth),
                Random.Range(-spawnHeight / 2f, spawnHeight / 2f),
                1f);

            GameObject s = starPrefab != null ? Instantiate(starPrefab, pos, Quaternion.identity)
                                              : Instantiate(CreateStarObject(), pos, Quaternion.identity);
            stars[i] = s.transform;
            stars[i].position = pos;

            // Yildiz sprite'i yoksa olustur
            var sr = s.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite == null)
                sr.sprite = Bullet.MakeCircle(Color.white, 8);

            float size = Random.Range(0.05f, 0.15f);
            stars[i].localScale = new Vector3(size, size, 1f);
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