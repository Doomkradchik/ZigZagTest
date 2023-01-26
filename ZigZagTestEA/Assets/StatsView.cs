using UnityEngine;
using TMPro;

public class StatsView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _diamonds;

    [SerializeField]
    private TMP_Text _score;

    public static StatsView Instance;
    private void Start()
    {
        Instance = this;
        UpdateDiamonds(StatsRepository.Load()._diamonds);
        UpdateScore(0);
    }

    public void UpdateDiamonds(int diamonds)
    {
        _diamonds.text = diamonds.ToString();
    }

    public void UpdateScore(int score)
    {
        _score.text = score.ToString();
    }
}
