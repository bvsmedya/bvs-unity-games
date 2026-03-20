using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameObject pausePanel;

    private void Start()
    {
        UpdateScore(0); UpdateHealth(3);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    private void Update() { if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance != null) GameManager.Instance.TogglePause(); }

    public void UpdateScore(int score) { if (scoreText != null) scoreText.text = "Skor: " + score; }

    public void UpdateHealth(int health)
    {
        if (healthText == null) return;
        string hearts = "";
        for (int i = 0; i < health; i++) hearts += "\u2764 ";
        healthText.text = hearts.TrimEnd();
    }

    public void ShowGameOverPanel(int finalScore)
    {
        if (gameOverPanel != null) { gameOverPanel.SetActive(true); if (gameOverScoreText != null) gameOverScoreText.text = "Son Skor: " + finalScore; }
    }

    public void ShowPausePanel() { if (pausePanel != null) pausePanel.SetActive(true); }
    public void HidePausePanel() { if (pausePanel != null) pausePanel.SetActive(false); }
    public void OnRestartButton() { if (GameManager.Instance != null) GameManager.Instance.RestartLevel(); }
    public void OnResumeButton() { if (GameManager.Instance != null) GameManager.Instance.TogglePause(); }
    public void OnMainMenuButton() { Time.timeScale = 1f; UnityEngine.SceneManagement.SceneManager.LoadScene(0); }
}