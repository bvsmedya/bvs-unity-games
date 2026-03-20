using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// MOBIL JOYSTICK - Dokunmatik ekran icin sanal joystick
///
/// KURULUM:
/// 1. Canvas altina bir Panel olustur, adi 'JoystickArea' yap
///    - Sol alta yerlestir (Anchor: bottom-left)
///    - Size: 200x200
/// 2. Icine bir Image olustur, adi 'JoystickBg' (daire sprite, yari saydam)
///    - Size: 150x150, ortala
/// 3. Icine baska bir Image, adi 'JoystickHandle' (kucuk daire)
///    - Size: 70x70, ortala
/// 4. Bu scripti 'JoystickArea' objesine ekle
/// 5. Inspector'dan Background ve Handle alanlarini bagla
/// </summary>
public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referanslar")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Ayarlar")]
    [SerializeField] private float handleRange = 1f;
    [SerializeField] private float deadZone = 0f;

    // Joystick degeri (-1 ile 1 arasi)
    public Vector2 Direction { get; private set; }
    public float Horizontal => Direction.x;
    public float Vertical => Direction.y;

    private RectTransform baseRect;
    private Canvas canvas;
    private Camera cam;
    private Vector2 startPos;

    private void Start()
    {
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        startPos = RectTransformUtility.WorldToScreenPoint(cam, background.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        Direction = (eventData.position - position) / (radius * canvas.scaleFactor);

        // Deadzone kontrolu
        if (Direction.magnitude <= deadZone)
            Direction = Vector2.zero;
        else
            Direction = Direction.magnitude > 1 ? Direction.normalized : Direction;

        // Handle pozisyonunu guncelle
        handle.anchoredPosition = new Vector2(
            Direction.x * radius.x * handleRange,
            Direction.y * radius.y * handleRange
        );
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}