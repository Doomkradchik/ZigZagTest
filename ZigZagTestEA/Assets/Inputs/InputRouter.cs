using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRouter : MonoBehaviour
{
    private PlayerInputs _input;

    public event Action Touched;

    public static InputRouter Instance;
    private void Awake()
    {
        Instance = this;
        _input = new PlayerInputs();
    }
    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Touch.performed += OnTouched;
    }

    private void OnTouched(InputAction.CallbackContext obj)
    {
        Touched?.Invoke();
    }

    private void OnDisable()
    {
        _input.Disable();
    }
}
