using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody), typeof(PhysicsMovement), typeof(SurfaceSlider))]
public class Ball : MonoBehaviour
{
    [SerializeField]
    private LayerMask _mask;

    private Vector3 _direction;
    private readonly Vector3 _startDirection = PathMeshGenerator.forward_dir;

    private const float RAY_DISTANCE = 2f;
    public event Action Died;

    private InputRouter _router;
    private IMovement _physicsMovement;
    private void Awake()
    {
        _physicsMovement = GetComponent<PhysicsMovement>();
        _router = new InputRouter(ChangeDirection);
        _direction = _startDirection;
    }

    private void OnEnable() => _router.OnEnable();
    private void OnDisable() => _router.OnDisable();

    private void FixedUpdate()
    {
        _physicsMovement.Move(_direction);
    }

    private void Update()
    {
        DetectPath();
    }

    private void ChangeDirection()
    {
        _direction = _direction == PathMeshGenerator.forward_dir 
            ? PathMeshGenerator.right_dir : PathMeshGenerator.forward_dir;
    }

    private void DetectPath()
    { 
        var ray = new Ray(transform.position, -Vector3.up);
        if(Physics.Raycast(ray, out RaycastHit hit, RAY_DISTANCE, _mask) == false)
        {
            
            Died?.Invoke();
            // enabled = false;
            return;
        }

        if(hit.collider.TryGetComponent(out PathMeshGenerator pathMeshGenerator))
        {
            pathMeshGenerator.TryGenerateNextBlocks(hit.point);
        }
    }
}
