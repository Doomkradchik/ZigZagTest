using System;
using UnityEngine;

public class ScoreSystemRoot : MonoBehaviour
{
    private int _score;
    public int BestScore { get; protected set; }

    public event Action RecordBroken;
    private const int RECORD_OFFSET = 20;
    private int _record;

    public static ScoreSystemRoot Instance;

    private void Awake()
    {
        Instance = this;
        BestScore = StatsRepository.Load()._bestScore;
    }

    public int CurrentScore
    {
        get => _score;

        set
        {
            _score = value;
            StatsView.Instance.UpdateScore(value);
            ValidateScore(value);

            if(_score > _record + RECORD_OFFSET)
            {
                _record = _score;
                RecordBroken?.Invoke();
            }
        }
    }

    private bool ValidateScore(int score)
    {
        if (score <= BestScore) { return false; }

        BestScore = score;
        var data = StatsRepository.Load();
        data._bestScore = score;
        StatsRepository.Save(data);
        return true;
    }

}
