using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// Bu script Unity Editor'de otomatik calisir ve sahneyi kurar.
/// Assets > Setup > Create Space Shooter Scene menusu ile tetiklenir.
public class SpaceShooterSetup : Editor
{
    [MenuItem("Assets/Setup/Create Space Shooter Scene")]
    public static void CreateScene()
    {
        // Yeni sahne olustur
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ===== KAMERA =====
        var camGO = new GameObject("Main Camera");
        camGO.tag = ("MainCamera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.orthographic = true;
        cam.orthographicSize = 6f;
        camGO.transform.position = new Vector3(0, 0, -10);
        camGO.AddComponent<AudioListener>();

        // ===== OYUNCU GEMİSİ =====
        var playerGO = new GameObject("Player");
        playerGO.tag = "Player";
        var playerSprite = playerGO.AddComponent<SpriteRenderer>();
        playerSprite.sprite = CreateTriangleSprite(Color.cyan);
        playerSprite.sortingOrder = 2;
        playerGO.transform.position = new Vector3(0, -4f, 0);

        var playerRb = playerGO.AddComponent<Rigidbody2D>();
        playerRb.gravityScale = 0;
        playerRb.freezeRotation = true;
        playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var playerCol = playerGO.AddComponent<PolygonCollider2D>();
        playerCol.isTrigger = true;

        // FirePoint
        var firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(playerGO.transform);
        firePoint.transform.localPosition = new Vector3(0, 0.6f, 0);

        // PlayerShip script
        var playerScript = playerGO.AddComponent<PlayerShip>();
        SetPrivateField(playerScript, "firePoint", firePoint.transform);

        // ===== YILDIZ ARKA PLANI =====
        var starBgGO = new GameObject("StarBackground");
        var starBg = starBgGO.AddComponent<StarBackground>();

        // ===== ENEMY SPAWNER =====
        var spawnerGO = new GameObject("EnemySpawner");
        spawnerGO.AddComponent<EnemySpawner>();

        // ===== GAME MANAGER =====
        var gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();

        // Sahneyi kaydet
        string scenePath = "Assets/Scenes/SpaceShooter.unity";
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.Refresh();

        Debug.Log("=== SPACE SHOOTER SAHNESI OLUSTURULDU! ===");
        Debug.Log("Simdi prefab'lari olusturmaniz gerekiyor:");
        Debug.Log("1. Hierarchy > Player > Inspector > PlayerShip > Bullet Prefab alani icin:");
        Debug.Log("   Yeni Circle objesi olustur, Bullet.cs ekle, Prefab yap");
        Debug.Log("2. EnemySpawner > Enemy Prefab icin Triangle objesi, EnemyShip.cs ekle");
    }

    private static Sprite CreateTriangleSprite(Color color)
    {
        // Basit kare sprite (Unity'nin varsayilan sprite'i)
        var tex = new Texture2D(32, 32);
        var pixels = new Color[32 * 32];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null) field.SetValue(obj, value);
    }
}