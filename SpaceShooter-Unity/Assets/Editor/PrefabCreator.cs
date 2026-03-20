using UnityEngine;
using UnityEditor;

public class PrefabCreator : Editor
{
    [MenuItem("Assets/Setup/Create Prefabs")]
    public static void CreatePrefabs()
    {
        string prefabPath = "Assets/Prefabs/";
        System.IO.Directory.CreateDirectory(prefabPath);

        // ===== OYUNCU MERMİSİ =====
        var bulletGO = new GameObject("PlayerBullet");
        bulletGO.tag = "PlayerBullet";

        var bulletSR = bulletGO.AddComponent<SpriteRenderer>();
        bulletSR.sprite = CreateCircleSprite(Color.yellow);
        bulletSR.sortingOrder = 3;
        bulletGO.transform.localScale = new Vector3(0.15f, 0.3f, 1f);

        var bulletRb = bulletGO.AddComponent<Rigidbody2D>();
        bulletRb.gravityScale = 0;

        var bulletCol = bulletGO.AddComponent<CircleCollider2D>();
        bulletCol.isTrigger = true;

        var bulletScript = bulletGO.AddComponent<Bullet>();
        SetField(bulletScript, "isEnemyBullet", false);
        SetField(bulletScript, "speed", 14f);

        PrefabUtility.SaveAsPrefabAsset(bulletGO, prefabPath + "PlayerBullet.prefab");
        Object.DestroyImmediate(bulletGO);

        // ===== DÜŞMAN MERMİSİ =====
        var eBulletGO = new GameObject("EnemyBullet");
        eBulletGO.tag = "EnemyBullet";

        var eBulletSR = eBulletGO.AddComponent<SpriteRenderer>();
        eBulletSR.sprite = CreateCircleSprite(new Color(1f, 0.3f, 0.3f));
        eBulletSR.sortingOrder = 3;
        eBulletGO.transform.localScale = new Vector3(0.15f, 0.3f, 1f);

        var eBulletRb = eBulletGO.AddComponent<Rigidbody2D>();
        eBulletRb.gravityScale = 0;

        var eBulletCol = eBulletGO.AddComponent<CircleCollider2D>();
        eBulletCol.isTrigger = true;

        var eBulletScript = eBulletGO.AddComponent<Bullet>();
        SetField(eBulletScript, "isEnemyBullet", true);
        SetField(eBulletScript, "speed", 8f);

        PrefabUtility.SaveAsPrefabAsset(eBulletGO, prefabPath + "EnemyBullet.prefab");
        Object.DestroyImmediate(eBulletGO);

        // ===== DÜŞMAN GEMİSİ =====
        var enemyGO = new GameObject("Enemy");
        enemyGO.tag = "Enemy";

        var enemySR = enemyGO.AddComponent<SpriteRenderer>();
        enemySR.sprite = CreateTriangleSprite(new Color(1f, 0.2f, 0.2f));
        enemySR.sortingOrder = 2;
        enemyGO.transform.localScale = new Vector3(0.8f, -0.8f, 1f); // Ters üçgen

        var enemyRb = enemyGO.AddComponent<Rigidbody2D>();
        enemyRb.gravityScale = 0;
        enemyRb.freezeRotation = true;

        var enemyCol = enemyGO.AddComponent<PolygonCollider2D>();
        enemyCol.isTrigger = true;

        var enemyScript = enemyGO.AddComponent<EnemyShip>();
        SetField(enemyScript, "moveSpeed", 2f);
        SetField(enemyScript, "scoreValue", 100);

        // Düşman mermisini bağla
        var eBulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "EnemyBullet.prefab");
        SetField(enemyScript, "enemyBulletPrefab", eBulletPrefab);

        PrefabUtility.SaveAsPrefabAsset(enemyGO, prefabPath + "Enemy.prefab");
        Object.DestroyImmediate(enemyGO);

        // ===== YILDIZ PARÇACIĞı =====
        var starGO = new GameObject("Star");
        var starSR = starGO.AddComponent<SpriteRenderer>();
        starSR.sprite = CreateCircleSprite(Color.white);
        starSR.sortingOrder = -1;
        starGO.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        PrefabUtility.SaveAsPrefabAsset(starGO, prefabPath + "Star.prefab");
        Object.DestroyImmediate(starGO);

        AssetDatabase.Refresh();
        Debug.Log("=== TUM PREFABLAR OLUSTURULDU! ===");
        Debug.Log("Simdi oyuncu gemisine PlayerBullet prefabini baglayabilirsiniz.");

        // Sahneye bagla
        ConnectPrefabsToScene();
    }

    private static void ConnectPrefabsToScene()
    {
        string prefabPath = "Assets/Prefabs/";

        var playerBullet = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "PlayerBullet.prefab");
        var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "Enemy.prefab");
        var starPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "Star.prefab");

        // Player gemisine mermiyi bagla
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var ps = player.GetComponent<PlayerShip>();
            SetField(ps, "bulletPrefab", playerBullet);
            Debug.Log("PlayerShip bullet prefab baglandi!");
        }

        // EnemySpawner'a enemy prefabini bagla
        var spawner = GameObject.FindObjectOfType<EnemySpawner>();
        if (spawner != null)
        {
            SetField(spawner, "enemyPrefab", enemyPrefab);
            Debug.Log("EnemySpawner enemy prefab baglandi!");
        }

        // StarBackground'a star prefabini bagla
        var starBg = GameObject.FindObjectOfType<StarBackground>();
        if (starBg != null)
        {
            SetField(starBg, "starPrefab", starPrefab);
            Debug.Log("StarBackground star prefab baglandi!");
        }

        // Sahneyi kaydet
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("=== SAHNE KAYDEDILDI - PLAY'E BASABILIRSINIZ! ===");
    }

    private static Sprite CreateCircleSprite(Color color)
    {
        int size = 32;
        var tex = new Texture2D(size, size);
        float center = size / 2f;
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                tex.SetPixel(x, y, dist <= center ? color : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private static Sprite CreateTriangleSprite(Color color)
    {
        int size = 32;
        var tex = new Texture2D(size, size);
        for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
            {
                float norm_x = x / (float)size;
                float norm_y = y / (float)size;
                bool inTriangle = norm_y >= Mathf.Abs(norm_x - 0.5f) * 2f;
                tex.SetPixel(x, y, inTriangle ? color : Color.clear);
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    private static void SetField(object obj, string name, object val)
    {
        if (obj == null) return;
        var f = obj.GetType().GetField(name,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (f != null) f.SetValue(obj, val);
    }
}