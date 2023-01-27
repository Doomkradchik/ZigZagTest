using UnityEngine;

public class ScoreSystemRoot : MonoBehaviour
{

    private int _score;
    private int _bestScore;

    public static ScoreSystemRoot Instance;

    private void Awake()
    {
        Instance = this;
        _bestScore = StatsRepository.Load()._bestScore;
    }

    public int CurrentScore
    {
        get => _score;

        set
        {
            _score = value;
            StatsView.Instance.UpdateScore(value);
            ValidateScore(value);
        }
    }

    private bool ValidateScore(int score)
    {
        if (score <= _bestScore) { return false; }

        _bestScore = score;
        var data = StatsRepository.Load();
        data._bestScore = score;
        StatsRepository.Save(data);
        return true;
    }

}
