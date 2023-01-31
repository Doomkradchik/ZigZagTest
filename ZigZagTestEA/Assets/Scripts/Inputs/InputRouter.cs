using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRouter : MonoBehaviour
{
    private PlayerInputs _input;

    public static event Action Touched;
    private void Awake()
    {
         DontDestroyOnLoad(gameObject);
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
