using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MOBIL INPUT MANAGER
/// Hem klavye hem dokunmatik girisi destekler.
/// PlayerController.cs ile birlikte calisir.
///
/// KURULUM:
/// 1. Bu scripti sahneye bos bir objeye ekle, adi 'MobileInputManager'
/// 2. Inspector'dan joystick ve jumpButton alanlarini bagla
/// 3. Canvas altina Jump butonu olustur (Image + Button component)
///    - Sag alta yerlestir, buyuk ve yuvarlak yap
/// 4. PlayerController'daki Update() fonksiyonunu asagidaki gibi degistir:
///    moveInput = MobileInputManager.Instance != null
///        ? MobileInputManager.Instance.GetHorizontal()
///        : Input.GetAxisRaw("Horizontal");
/// </summary>
public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager Instance { get; private set; }

    [Header("UI Referanslar")]
    [SerializeField] private MobileJoystick joystick;
    [SerializeField] private Button jumpButton;

    [Header("Ayarlar")]
    [Tooltip("Mobil UI'yi goster/gizle (editor'de gizlemek icin)")]
    [SerializeField] private bool showMobileUI = true;

    [SerializeField] private GameObject mobileUIRoot;

    // Jump butonu basili mi?
    private bool jumpPressed = false;
    private bool jumpConsumed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Mobil platformda UI'yi goster, editor'de gizle
        bool isMobile = Application.isMobilePlatform;
        if (mobileUIRoot != null)
            mobileUIRoot.SetActive(showMobileUI || isMobile);

        // Jump butonu event'i bagla
        if (jumpButton != null)
        {
            jumpButton.onClick.AddListener(OnJumpPressed);

            // Basili tutma destegi icin EventTrigger ekle
            var trigger = jumpButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((e) => jumpPressed = true);
            trigger.triggers.Add(pointerDown);

            var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((e) => { jumpPressed = false; jumpConsumed = false; });
            trigger.triggers.Add(pointerUp);
        }
    }

    private void OnJumpPressed() { jumpPressed = true; }

    /// <summary>PlayerController'dan cagrilir - yatay eksen degeri</summary>
    public float GetHorizontal()
    {
        // Klavye + joystick ikisini de destekle
        float keyboard = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(keyboard) > 0.1f) return keyboard;
        return joystick != null ? joystick.Horizontal : 0f;
    }

    /// <summary>PlayerController'dan cagrilir - ziplama tetiklendi mi?</summary>
    public bool GetJumpDown()
    {
        if (Input.GetButtonDown("Jump")) return true;
        if (jumpPressed && !jumpConsumed)
        {
            jumpConsumed = true;
            return true;
        }
        return false;
    }

    /// <summary>PlayerController'dan cagrilir - ziplama birakildimi?</summary>
    public bool GetJumpUp()
    {
        return Input.GetButtonUp("Jump") || (!jumpPressed && jumpConsumed);
    }
}