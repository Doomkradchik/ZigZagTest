using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    void Move(Vector3 direction);
}

public class PhysicsMovement : MonoBehaviour, IMovement
{
    private Rigidbody _rigidbody;

    private SurfaceSlider _slider;

    [SerializeField, Min(1f)]
    private float _startUnitsPerSecond;

    public float Speed { get; set; }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _slider = GetComponent<SurfaceSlider>();

        Speed = _startUnitsPerSecond;
    }

    public void Move(Vector3 direction)
    {
        var directionAlongSurface = _slider.Project(direction.normalized);
        var offset = directionAlongSurface * Speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + offset);
    }
}
