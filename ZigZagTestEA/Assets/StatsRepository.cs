using UnityEngine;

public struct StatsConfig
{
    public int _bestScore;
    public int _diamonds;
}

public static class StatsRepository
{
    private const string DIAMONDS_KEY = "DIAMONDS";
    private const string BEST_SCORE_KEY = "B_SCORE";
    public static void Save(StatsConfig config)
    {
        PlayerPrefs.SetInt(DIAMONDS_KEY, config._diamonds);
        PlayerPrefs.SetInt(BEST_SCORE_KEY, config._bestScore);
    }

    public static StatsConfig Load()
    {
        return new StatsConfig
        {
            _bestScore = PlayerPrefs.GetInt(BEST_SCORE_KEY),
            _diamonds = PlayerPrefs.GetInt(DIAMONDS_KEY),
        };
    }
}
