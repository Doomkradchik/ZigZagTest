using System.Collections;
using UnityEngine;

public class LevelConstructionRoot : MonoBehaviour
{
    [SerializeField]
    private GameObject _startPlatform;

    private const float START_DELTA = 4f;
    private const float DELTA = 1f;

    private float DurationDeltaDestroy
    {
        get
        {
            if (_movement == null)
                _movement = FindObjectOfType<PhysicsMovement>();

            return DELTA / _movement.Speed;
        }
    }
    private PathMeshGenerator _path;
    private PhysicsMovement _movement;

    private void Awake()
    {
        _path = FindObjectOfType<PathMeshGenerator>();
    }

    private void Start()
    {
        StartCoroutine(DestroyingBlocksRoutine());
    }

    private IEnumerator DestroyingBlocksRoutine()
    {
        yield return new WaitForSeconds(START_DELTA);
        TilesSimulationRoot.Instance.Simulate(_startPlatform);


        while (true)
        {
            yield return new WaitForSeconds(DurationDeltaDestroy);
            _path.DestroyLastBlockFromBeginning();
        }
    }
}
