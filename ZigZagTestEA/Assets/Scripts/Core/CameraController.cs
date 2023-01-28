using UnityEngine;

public class CameraController : MonoBehaviour, IPauseHandler
{
    private readonly Vector3 _direction = new Vector3(1f, 0, 1f);
    private PhysicsMovement _movement;
    private Transform p_transform;

    private const float OFFSET = 17f;
    private Vector3 CalculatePosition
    {
        get
        {
            var length = _movement.transform.position.ToXZPlane().magnitude 
                * Mathf.Cos(Vector3.Angle(_movement.transform.position.ToXZPlane(), _direction) * Mathf.Deg2Rad);

            var position =  (length - OFFSET) * _direction.normalized;

            return new Vector3(position.x, p_transform.position.y, position.z);
        }
    }

    private void Awake()
    {
        _movement = FindObjectOfType<PhysicsMovement>();
        GameProgressScaleController.Subscribe(this);
        p_transform = transform.parent;

        p_transform.position = CalculatePosition;
    }

    private void Start()
    {
        p_transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
    }
    private void LateUpdate()
    {
        p_transform.position = CalculatePosition;
    }

    public void Pause(PauseMode pauseMode) => enabled = false;

    public void Unpause() => enabled = true;
}
