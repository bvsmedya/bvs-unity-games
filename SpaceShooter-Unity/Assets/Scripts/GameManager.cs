using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private int score = 0;
    private bool gameOver = false;
    public bool IsGameOver => gameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        UpdateScoreUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void AddScore(int points)
    {
        if (gameOver) return;
        score += points;
        UpdateScoreUI();
    }

    public void UpdateHealthUI(int health)
    {
        if (healthText == null) return;
        string h = "";
        for (int i = 0; i < health; i++) h += "\u2665 ";
        healthText.text = h.TrimEnd();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "SKOR: " + score;
    }

    public void GameOver()
    {
        if (gameOver) return;
        gameOver = true;
        Time.timeScale = 0.3f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (finalScoreText != null) finalScoreText.text = "SKOR: " + score;
        }
        Invoke(nameof(RestartLevel), 2f * 0.3f);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // UI buton fonksiyonlari
    public void OnRestartButton() { RestartLevel(); }
}