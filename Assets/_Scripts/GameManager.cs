using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TextMeshProUGUI scoreText;
    private int score = 0;
    private int totalCoins = 4;

    public bool IsWinConditionMet { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        IsWinConditionMet = false;
    }

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
        CheckWinCondition();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void CheckWinCondition()
    {
        if (score >= totalCoins)
        {
            IsWinConditionMet = true;
            Debug.Log("You Win! All coins collected.");
        }
    }

    // Метод для тестов!
    public int GetCurrentScore()
    {
        return score;
    }

    public void ResetGame()
    {
        score = 0;
        IsWinConditionMet = false;
        UpdateScoreText();
    }
}
