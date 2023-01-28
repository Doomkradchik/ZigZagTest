using System;
using UnityEngine;

public class Initable<T> : MonoBehaviour
{
    public T Model { get; private set; }

    private bool _inited;

    public void Init(T model)
    {
        if (_inited)
            throw new InvalidOperationException();

        Model = model;
        _inited = true;
        enabled = true;
    }

    private void OnEnable()
    {
        if (_inited == false)
        {
            enabled = false;
            return;
        }
    }
}
