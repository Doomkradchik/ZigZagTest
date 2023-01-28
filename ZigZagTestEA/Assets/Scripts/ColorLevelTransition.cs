using System.Collections;
using UnityEngine;

public class ColorLevelTransition : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    private float _duration;

    [SerializeField]
    private Color32[] _colors;

    [SerializeField]
    private ScoreSystemRoot _scoreSystem;

    private int _index;
    private Material _material;


    private void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable()
    {
        _scoreSystem.RecordBroken += LinearTranslateToColor;
    }

    private void OnDestroy()
    {
        _scoreSystem.RecordBroken -= LinearTranslateToColor;
        StopAllCoroutines();
    }

    private Color32 Peek(int offset)
    {
        _index += offset;

        if (_index < 0)
            throw new System.InvalidOperationException();

        if (_index >= _colors.Length)
            _index = 0;

        return _colors[_index];
    }

    private void LinearTranslateToColor() 
        => StartCoroutine(LinearTranslateToColorRoutine(Peek(1)));

    private IEnumerator LinearTranslateToColorRoutine(Color32 color)
    {
        float progress = 0f;
        float expiredSeconds = 0f;

        var start = _material.color;

        while (progress < 1f)
        {
            expiredSeconds += Time.deltaTime;
            progress = expiredSeconds / _duration;

            _material.color = Color32.Lerp(start, color, progress);
            yield return null;
        }
    }
}
