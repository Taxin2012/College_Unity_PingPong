using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Balls")]
    public GameObject Ball;
    public GameObject ballPrefab;
    public float ballSpeed = 5f;
    public float ballSpeedMultiplier = 1.05f;

    [Header("Player 1")]
    public Paddle Player1Paddle;
    public Gate Player1Gate;

    [Header("Player 2")]
    public Paddle Player2Paddle;
    public Gate Player2Gate;

    [Header("Score UI")]
    public TextMeshProUGUI Player1Text;
    public TextMeshProUGUI Player2Text;
    public TextMeshProUGUI WinnerText;

    [Header("Goal Popup")]
    public GameObject scorePopup;
    public TextMeshProUGUI scorePopupText;
    public float scorePopupDuration = 3f;

    [Header("Win UI")]
    public GameObject winPanel;

    [Header("Game Settings")]
    public int winScore = 5;

    private int player1Score;
    private int player2Score;
    private Coroutine goalRoutine;

    public bool IsGameOver { get; private set; }

    public static event Action OnGameOver;
    public static event Action<Transform> OnBallSpawned;

    private void OnEnable()
    {
        Gate.OnPlayerScored += OnPlayerScored;
    }

    private void OnDisable()
    {
        Gate.OnPlayerScored -= OnPlayerScored;
    }

    private void Start()
    {
        int settingsScore = GameSettings.WinScore;
        if (settingsScore > 0)
            winScore = settingsScore;

        player1Score = 0;
        player2Score = 0;
        IsGameOver = false;

        UpdateScoreUI();

        if (WinnerText != null) WinnerText.gameObject.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
        HideGoalPopup();

        CreateBall();
    }

    private void CreateBall()
    {
        if (ballPrefab == null) return;

        GameObject go = Instantiate(ballPrefab);
        Ball = go;

        Ball ball = go.GetComponent<Ball>();
        if (ball != null)
        {
            ball.speed = ballSpeed;
            ball.speedMultiplier = ballSpeedMultiplier;
        }

        OnBallSpawned?.Invoke(go.transform);
    }

    private void OnPlayerScored(bool isSecondPlayer)
    {
        RegisterGoal(isSecondPlayer ? 2 : 1);
    }

    public void Player1Scored()
    {
        RegisterGoal(1);
    }

    public void Player2Scored()
    {
        RegisterGoal(2);
    }

    private void RegisterGoal(int player)
    {
        if (IsGameOver) return;

        if (player == 2)
            player2Score++;
        else
            player1Score++;

        UpdateScoreUI();
        ShowGoalPopup(player);

        if (goalRoutine != null)
            StopCoroutine(goalRoutine);

        goalRoutine = StartCoroutine(GoalSequenceRoutine(player));
    }

    private IEnumerator GoalSequenceRoutine(int player)
    {
        yield return new WaitForSeconds(scorePopupDuration);
        goalRoutine = null;

        HideGoalPopup();

        if (IsGameOver) yield break;

        if (winScore > 0 && (player1Score >= winScore || player2Score >= winScore))
        {
            EndGame(player1Score >= winScore ? 1 : 2);
            yield break;
        }

        CreateBall();
    }

    private void ShowGoalPopup(int player)
    {
        if (scorePopupText != null)
            scorePopupText.text = "Игрок " + player + " забил!";

        if (scorePopup != null)
            scorePopup.SetActive(true);
    }

    private void HideGoalPopup()
    {
        if (scorePopup != null)
            scorePopup.SetActive(false);
    }

    private void UpdateScoreUI()
    {
        if (Player1Text != null)
            Player1Text.text = "Игрок 1: " + player1Score;
        if (Player2Text != null)
            Player2Text.text = "Игрок 2: " + player2Score;
    }

    private void EndGame(int winner)
    {
        IsGameOver = true;

        if (goalRoutine != null)
        {
            StopCoroutine(goalRoutine);
            goalRoutine = null;
        }

        HideGoalPopup();
        OnGameOver?.Invoke();

        foreach (Ball ball in FindObjectsByType<Ball>(FindObjectsSortMode.None))
            Destroy(ball.gameObject);

        if (WinnerText != null)
        {
            WinnerText.text = "Игрок " + winner + " победил!";
            WinnerText.gameObject.SetActive(true);
        }

        if (winPanel != null)
            winPanel.SetActive(true);
    }

    /// <summary>Перезапуск раунда без перезагрузки сцены.</summary>
    public void RestartGame()
    {
        StopAllCoroutines();
        goalRoutine = null;
        IsGameOver = false;
        player1Score = 0;
        player2Score = 0;

        UpdateScoreUI();
        HideGoalPopup();

        if (WinnerText != null) WinnerText.gameObject.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (Player1Paddle != null)
        {
            Player1Paddle.enabled = true;
            Player1Paddle.ResetPaddle();
        }

        if (Player2Paddle != null)
        {
            Player2Paddle.enabled = true;
            Player2Paddle.ResetPaddle();
        }

        foreach (Ball ball in FindObjectsByType<Ball>(FindObjectsSortMode.None))
            Destroy(ball.gameObject);

        CreateBall();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
