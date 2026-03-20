using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private UIManager uiManager;
    [SerializeField] private string gameOverSceneName = "";
    [SerializeField] private string nextLevelSceneName = "";

    private int currentScore = 0;
    private bool isGameOver = false;
    public int CurrentScore => currentScore;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;
        currentScore += points;
        if (uiManager != null) uiManager.UpdateScore(currentScore);
    }

    public void UpdateHealthUI(int currentHealth) { if (uiManager != null) uiManager.UpdateHealth(currentHealth); }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (uiManager != null) uiManager.ShowGameOverPanel(currentScore);
        Time.timeScale = 0.3f;
        Invoke(nameof(RestartLevel), 2f * 0.3f);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; isGameOver = false; currentScore = 0;
        SceneManager.LoadScene(string.IsNullOrEmpty(gameOverSceneName) ? SceneManager.GetActiveScene().name : gameOverSceneName);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(nextLevelSceneName)) { SceneManager.LoadScene(nextLevelSceneName); return; }
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(next < SceneManager.sceneCountInBuildSettings ? next : 0);
    }

    public void TogglePause()
    {
        if (Time.timeScale == 0) { Time.timeScale = 1f; if (uiManager != null) uiManager.HidePausePanel(); }
        else { Time.timeScale = 0f; if (uiManager != null) uiManager.ShowPausePanel(); }
    }
}