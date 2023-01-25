using System;
using UnityEngine.InputSystem;

public class InputRouter
{
    private PlayerInputs _input;

    public Action Touched { get; }

    public InputRouter(Action touched)
    {
        _input = new PlayerInputs();
        Touched = touched;
    }

    public void OnEnable()
    {
        _input.Enable();
        _input.Player.Touch.performed += OnTouched;
    }

    private void OnTouched(InputAction.CallbackContext obj)
    {
        Touched?.Invoke();
    }

    public void OnDisable()
    {
        _input.Disable();
    }
}
