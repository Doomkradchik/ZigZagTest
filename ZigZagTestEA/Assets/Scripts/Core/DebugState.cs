using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IState
{
    void Enter();
    void Exit();
}

public sealed class DebugState : Ball
{
    private void OnEnable()
    {
        _router.enabled = false;
    }

    private void OnDisable()
    {
        _router.enabled = true;
    }
    //public void Enter()
    //{
    //    _router.enabled = false;
    //}

    //public void Exit()
    //{
    //    _router.enabled = true;
    //}

    protected override void Update()
    {
        base.Update();

    }

    private void TryArrangeDirection()
    {
        if (_blockDetected == false)
            return;

        if (_block.direction == _direction)
            return;

        ChangeDirection();
    }
}
