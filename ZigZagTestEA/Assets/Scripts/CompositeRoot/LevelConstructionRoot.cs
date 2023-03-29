
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelConstructionRoot : MonoBehaviour
{
    [Header("Diamonds Pool")]
    [SerializeField]
    private Diamond _prefab;
    [SerializeField]
    private int _count;
    [SerializeField]
    private Transform _root;

    [SerializeField]
    private bool _expandable;

    private const float DELTA = 0.93f;

    private readonly float _diamondChance = 0.23f;

    private PoolMono<Diamond> _diamondsPool;

    private PathMeshGenerator _path;
    private PhysicsMovement _movement;

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        _path = FindObjectOfType<PathMeshGenerator>();
        _diamondsPool = new PoolMono<Diamond>(_prefab, _count, _root);
        _diamondsPool.AutoExpand = _expandable;
    }

    private void Start()
    {
      //  StartCoroutine(DestroyingBlocksRoutine());
        _path.pathPieceGenerated += SpawnDiamonds;
    }

    private void OnDestroy()
    {
        _path.pathPieceGenerated -= SpawnDiamonds;
        StopAllCoroutines();
    }

    public void SpawnDiamonds(int startIndex)
    {
        for (int i = startIndex; i < _path._blocks.Count; i++)
        {
            if (Random.value < _diamondChance)
            {
                _diamondsPool.FreeElement.transform.position =
                    _path._blocks[i].position +
                    Vector3.up * PathMeshGenerator.BLOCK_HEIGHT * 0.5f
                    + _path.gameObject.transform.position;
            }
        }
    }

}
