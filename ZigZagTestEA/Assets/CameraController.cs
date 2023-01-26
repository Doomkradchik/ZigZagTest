using UnityEngine;

public class CameraController : MonoBehaviour, IPauseHandler
{
    private readonly Vector3 _direction = new Vector3(1f, 0, 1f);
    private PhysicsMovement _movement;
    private Transform p_transform;

    private const float OFFSET = 17f;

    private bool _stopped = false;
    private Vector3 CalculatePosition
    {
        get
        {
            var length = _movement.transform.position.ToXZPlane().magnitude 
                * Mathf.Cos(Vector3.Angle(_movement.transform.position.ToXZPlane(), _direction) * Mathf.Deg2Rad);

            return (length - OFFSET) * _direction.normalized;
        }
    }

    private void Awake()
    {
        _movement = FindObjectOfType<PhysicsMovement>();
        GameProgressScaleController.Subscribe(this);
        p_transform = transform.parent;
    }
    private void Start()
    {
        p_transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
    }
    private void LateUpdate()
    {
        if(_stopped == false)
        p_transform.position = new Vector3(CalculatePosition.x, p_transform.position.y, CalculatePosition.z);
    }

    public void Pause() => _stopped = true;

    public void Unpause() => _stopped = false;
}
