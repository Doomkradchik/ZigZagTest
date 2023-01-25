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

    [SerializeField]
    private float _unitsPerSecond;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _slider = GetComponent<SurfaceSlider>();
    }

    public void Move(Vector3 direction)
    {
        var directionAlongSurface = _slider.Project(direction.normalized);
        var offset = directionAlongSurface * _unitsPerSecond * Time.fixedDeltaTime;
        _rigidbody.MovePosition(_rigidbody.position + offset);
    }
}
