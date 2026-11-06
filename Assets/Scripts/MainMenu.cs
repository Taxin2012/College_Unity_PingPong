using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Управление меню: главное меню -> выбор режима -> выбор карты и количества очков.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject modePanel;
    public GameObject mapPanel;

    [Header("Win Score Selection")]
    public TextMeshProUGUI winScoreLabel;
    public Slider winScoreSlider;

    public const int MinWinScore = 3;
    public const int MaxWinScore = 10;

    private void Start()
    {
        GameSettings.WinScore = Mathf.Clamp(GameSettings.WinScore, MinWinScore, MaxWinScore);
        SetupWinScoreSlider();
        ShowMain();
        RefreshWinScoreUI();
    }

    private void SetupWinScoreSlider()
    {
        if (winScoreSlider == null)
            return;

        winScoreSlider.minValue = MinWinScore;
        winScoreSlider.maxValue = MaxWinScore;
        winScoreSlider.wholeNumbers = true;

        int savedScore = GameSettings.WinScore;
        winScoreSlider.SetValueWithoutNotify(savedScore);
        winScoreSlider.onValueChanged.AddListener(OnWinScoreSliderChanged);
    }

    private void OnDestroy()
    {
        if (winScoreSlider != null)
            winScoreSlider.onValueChanged.RemoveListener(OnWinScoreSliderChanged);
    }

    public void ShowMain()
    {
        SetPanels(true, false, false);
    }

    public void ShowMode()
    {
        SetPanels(false, true, false);
    }

    public void ShowMaps()
    {
        SetPanels(false, false, true);
        RefreshWinScoreUI();
    }

    private void SetPanels(bool showMain, bool showMode, bool showMaps)
    {
        if (mainPanel != null) mainPanel.SetActive(showMain);
        if (modePanel != null) modePanel.SetActive(showMode);
        if (mapPanel != null) mapPanel.SetActive(showMaps);
    }

    public void StartAIGame()
    {
        GameSettings.Mode = GameMode.VsAI;
        ShowMaps();
    }

    public void StartTwoPlayerGame()
    {
        GameSettings.Mode = GameMode.TwoPlayers;
        ShowMaps();
    }

    public void OnWinScoreSliderChanged(float value)
    {
        int score = Mathf.Clamp(Mathf.RoundToInt(value), MinWinScore, MaxWinScore);
        GameSettings.WinScore = score;
        RefreshWinScoreUI();
    }

    /// <summary>Устанавливает количество очков для победы и синхронизирует слайдер.</summary>
    public void SelectWinScore(int value)
    {
        value = Mathf.Clamp(value, MinWinScore, MaxWinScore);
        GameSettings.WinScore = value;

        if (winScoreSlider != null)
            winScoreSlider.SetValueWithoutNotify(value);

        RefreshWinScoreUI();
    }

    public void StartGameWithBackground(int index)
    {
        GameSettings.BackgroundIndex = index;
        SceneManager.LoadScene("Game");
    }

    public void PlayGame(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void RefreshWinScoreUI()
    {
        int selectedScore = Mathf.Clamp(GameSettings.WinScore, MinWinScore, MaxWinScore);

        if (winScoreLabel != null)
            winScoreLabel.text = "До победы: " + selectedScore + " " + PointsWord(selectedScore);

        if (winScoreSlider != null && !Mathf.Approximately(winScoreSlider.value, selectedScore))
            winScoreSlider.SetValueWithoutNotify(selectedScore);
    }

    private static string PointsWord(int value)
    {
        int mod100 = value % 100;
        if (mod100 >= 11 && mod100 <= 14) return "очков";

        int mod10 = value % 10;
        if (mod10 == 1) return "очко";
        if (mod10 >= 2 && mod10 <= 4) return "очка";
        return "очков";
    }
}
