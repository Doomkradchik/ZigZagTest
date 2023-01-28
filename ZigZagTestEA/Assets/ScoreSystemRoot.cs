using System;
using UnityEngine;

public class ScoreSystemRoot : MonoBehaviour
{
    private int _score;
    private int _bestScore;

    public event Action RecordBroken;
    private const int RECORD_OFFSET = 20;
    private int _record;

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

            if(_score > _record + RECORD_OFFSET)
            {
                _record = _score;
                RecordBroken?.Invoke();
            }
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
