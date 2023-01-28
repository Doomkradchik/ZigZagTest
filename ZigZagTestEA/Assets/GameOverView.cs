using TMPro;
using UnityEngine;

public class GameOverView : MonoBehaviour
{
    [SerializeField]
    private Ball _ball;

    [SerializeField]
    private TMP_Text _score;

    [SerializeField]
    private TMP_Text _bestScore;

    [SerializeField]
    private Animator _animator;

    private void Start()
    {
        _animator.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _ball.Died += OnDie;
    }

    private void OnDestroy()
    {
        _ball.Died -= OnDie;
    }


    private void OnDie()
    {
        _animator.gameObject.SetActive(true);
        _animator.SetBool("Died", true);
        _score.text = ScoreSystemRoot.Instance.CurrentScore.ToString();
        _bestScore.text = ScoreSystemRoot.Instance.BestScore.ToString();
    }
}
