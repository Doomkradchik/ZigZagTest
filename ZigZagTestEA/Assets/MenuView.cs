using TMPro;
using UnityEngine;

public class MenuView : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameView;
    [SerializeField]
    private TMP_Text _score;
    [SerializeField]
    private TMP_Text _games;
    [SerializeField]
    private TMP_Text _diamonds;

    private string[] _texts;

    private void Start()
    {
        gameObject.SetActive(true);
        _gameView.SetActive(false);
        var data = StatsRepository.Load();

        _texts = new string[]
        {
            _score.text,
            _games.text,
        };

        UpdateMenuStats(data);
        GameProgressScaleController.Pause(PauseMode.Start);
        GetComponent<Animator>().SetBool("Started", false);
    }

    private void OnEnable()
    {
        InputRouter.Instance.Touched += StartGame;
    }

    private void OnDisable()
    {
        InputRouter.Instance.Touched -= StartGame;
    }

    private void UpdateMenuStats(StatsConfig config)
    {
        _score.text = $"{_texts[0]} {config._bestScore}";
        _games.text = $"{_texts[1]} {config._games}";

        _diamonds.text = config._diamonds.ToString();
    }

    public void StartGame()
    {
        var data = StatsRepository.Load();
        data._games++;
        StatsRepository.Save(data);
        UpdateMenuStats(data);

        GameProgressScaleController.Unpause();
        GetComponent<Animator>().SetBool("Started", true);
        _gameView.SetActive(true);
        enabled = false;
    }
}
