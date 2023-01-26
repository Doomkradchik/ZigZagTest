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

    public Vector3 DirectionAlongSurface { get; set; }

    public float Speed { get; set; }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _slider = GetComponent<SurfaceSlider>();

        Speed = _startUnitsPerSecond;
    }

    public void Move(Vector3 direction)
    {
        DirectionAlongSurface = _slider.Project(direction.normalized);
        var offset = DirectionAlongSurface * Speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + offset);
    }
}
