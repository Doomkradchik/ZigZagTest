using UnityEngine;

public class CameraController : MonoBehaviour
{
    private readonly Vector3 _direction = new Vector3(1f, 0, 1f);
    private Ball _ball;
    private PhysicsMovement _movement;
    private Transform p_transform;

    private const float RATIO = 0.60f;
    private float CalculateSpeed
    {
        get
        {
            return _movement.Speed * Mathf.Cos(Mathf.Deg2Rad * 45f)* RATIO;
        }
    }

    private void Awake()
    {
        _ball = FindObjectOfType<Ball>();
        _movement = _ball.GetComponent<PhysicsMovement>();
        p_transform = transform.parent;
    }
    private void Start()
    {
        p_transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
    }
    private void LateUpdate()
    {
        p_transform.position += CalculateSpeed * new Vector3(1f, 0, 1f) * Time.deltaTime;
    }
}
