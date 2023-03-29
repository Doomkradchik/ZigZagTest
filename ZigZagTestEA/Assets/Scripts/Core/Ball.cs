using UnityEngine;
using System;
using Block = PathMeshGenerator.Block;

[RequireComponent(typeof(Rigidbody), typeof(PhysicsMovement), typeof(SurfaceSlider))]
public class Ball : PauseHandler
{
    [SerializeField]
    private LayerMask _mask;

    protected Vector3 _direction;
    public Vector3 Direction => _direction;
    private readonly Vector3 _startDirection = PathMeshGenerator.left_dir;

    private const float RAY_DISTANCE = 2f;
    public event Action Died;

    protected InputRouter _router;
    protected IMovement _physicsMovement;

    private bool _entered = false;
    private bool _died = false;
    private float _sphereRadius;

    protected bool _blockDetected = false;
    protected Block _block;


    private void Awake()
    {
        _physicsMovement = GetComponent<PhysicsMovement>();
        _direction = _startDirection;

        _sphereRadius = GetComponent<SphereCollider>().radius;

        GameProgressScaleController.Subscribe(this);
    }

    private void OnEnable()
    {
        InputRouter.Touched += ChangeDirection;
    }
    private void OnDestroy()
    {
        InputRouter.Touched -= ChangeDirection;
        GameProgressScaleController.Unsubscribe(this);
    }

    private void FixedUpdate()
    {
        if(_mode == PauseMode.None)
           _physicsMovement.Move(_direction);
    }

    protected virtual void Update()
    {
        if(_died == false)
             DetectPath();
    }

    protected override void OnUnpause()
    {
        if (_mode == PauseMode.Start)
            OnInGameProcessStarted();
    }

    protected virtual void OnInGameProcessStarted()
    {
        ScoreSystemRoot.Instance.CurrentScore = 0;
    } 

    protected void ChangeDirection()
    {
        if (_mode != PauseMode.None) { return; }

        _direction = _direction == PathMeshGenerator.left_dir
            ? PathMeshGenerator.right_dir : PathMeshGenerator.left_dir;
        ScoreSystemRoot.Instance.CurrentScore++;
    }

    private void DetectPath()
    {
        var direction = -Vector3.up;
        var cast = Physics.Raycast(transform.position, direction, out RaycastHit hit, RAY_DISTANCE, _mask);

        if (cast == false  && _entered)
        {
            if(Physics.OverlapSphere(transform.position + direction * 0.75f, _sphereRadius * 0.5f, _mask).Length == 0)
            {
                Died?.Invoke();
                _died = true;
                GameProgressScaleController.Pause();
                return;
            }
        }
        

        if(hit.collider != null)
        {
            _entered = true;

            if(hit.collider.TryGetComponent(out PathMeshGenerator pathMeshGenerator))
            {
                _blockDetected = pathMeshGenerator.OnBlockDetected(hit.point, out PathMeshGenerator.Block block);
                if (_blockDetected)
                    _block = block;
            }
        }
    }

   

}
