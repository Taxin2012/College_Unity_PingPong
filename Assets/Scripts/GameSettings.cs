using UnityEngine;

public enum GameMode
{
    VsAI = 0,
    TwoPlayers = 1
}

/// <summary>
/// Настройки, выбранные в меню и переживающие загрузку сцены.
/// </summary>
public static class GameSettings
{
    private const string ModeKey = "PingPong.Mode";
    private const string BackgroundKey = "PingPong.Background";
    private const string WinScoreKey = "PingPong.WinScore";

    public static GameMode Mode
    {
        get => (GameMode)PlayerPrefs.GetInt(ModeKey, (int)GameMode.VsAI);
        set
        {
            PlayerPrefs.SetInt(ModeKey, (int)value);
            PlayerPrefs.Save();
        }
    }

    public static int BackgroundIndex
    {
        get => Mathf.Max(0, PlayerPrefs.GetInt(BackgroundKey, 0));
        set
        {
            PlayerPrefs.SetInt(BackgroundKey, Mathf.Max(0, value));
            PlayerPrefs.Save();
        }
    }

    /// <summary>Количество очков, необходимое для победы.</summary>
    public static int WinScore
    {
        get
        {
            int value = PlayerPrefs.GetInt(WinScoreKey, 5);
            return value > 0 ? value : 5;
        }
        set
        {
            PlayerPrefs.SetInt(WinScoreKey, Mathf.Max(1, value));
            PlayerPrefs.Save();
        }
    }
}
