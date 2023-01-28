using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class PathMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _startPlatform;

    [SerializeField, Header("Diagnostics: (do not modify)")]
    private int _blocksCount = 0;

    public IEnumerable<Vector3> BlockPositions => _blocks.Select(b => b.position);

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Mesh _customMesh;

    public static readonly Vector3 right_dir = Vector3.right;
    public static readonly Vector3 left_dir = Vector3.forward;

    public const float BLOCK_WIDTH = 1f;
    public const float BLOCK_HEIGHT = 5f;
    private const int PULL_BLOCK_LENGTH = 40;

    public readonly List<Block> _blocks = new List<Block>();

    private Material _pathMaterial;

    public event Action<int> pathPieceGenerated;
    private Camera _camera;

    private Vector3 GetDirectionOffset(Vector3 position)
    {
        var screenPos = _camera.WorldToViewportPoint(position);
        if (screenPos.x < 0.15f) { return right_dir; }
        if (screenPos.x > 0.85f) { return left_dir; }

        return Random.value < 0.5f ? right_dir : left_dir;
    }


    public struct Block 
    {
        public Vector3 direction;
        public Vector3 position;
    }


    private void Awake()
    {
        _camera = Camera.main;
        _pathMaterial = GetComponent<MeshRenderer>().material;
        _blocksCount = 0;
    }

    private void Start()
    {
        SpawnBlocks(PULL_BLOCK_LENGTH);
    }

    private void RegenerateMesh()
    {
        _vertices.Clear();
        _triangles.Clear();

        for (int i = 0; i < _blocks.Count; i++)
        {
            GenerateBlock(_blocks[i], i);
        }

        _customMesh = new Mesh();
        _customMesh.vertices = _vertices.ToArray();
        _customMesh.triangles = _triangles.ToArray();

        _customMesh.Optimize();
        _customMesh.RecalculateBounds();
        _customMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = _customMesh;
        GetComponent<MeshCollider>().sharedMesh = _customMesh;
    }


    public void SpawnBlocks(int length)
    {
        Vector3 lastPosition;
        int startPoint = _blocks.Count;
        if (_blocks.Count == 0)
            lastPosition = Vector3.zero;
        else
            lastPosition = _blocks[_blocks.Count - 1].position;

        for (int i = 0; i < length; i++)
        {
            var dir = GetDirectionOffset(lastPosition);
            _blocks.Add(new Block { position = lastPosition, direction = dir});
            _blocksCount = _blocks.Count;
            lastPosition += dir * BLOCK_WIDTH;
        }

        pathPieceGenerated?.Invoke(startPoint);
        RegenerateMesh();
    }

    public void DestroyLastBlockFromBeginning()
    {
        if (_blocks.Count == 0) { return; }

        var tileMesh = new Mesh();

        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        GenerateBackSideBlock(triangles, vertices, Vector3.zero);
        GenerateLeftSideBlock(triangles, vertices, Vector3.zero);
        GenerateRightSideBlock(triangles, vertices, Vector3.zero);
        GenerateFrontSideBlock(triangles, vertices, Vector3.zero);
        GenerateUpperSideBlock(triangles, vertices, Vector3.zero);

        tileMesh.vertices = vertices.ToArray();
        tileMesh.triangles = triangles.ToArray();

        tileMesh.Optimize();
        tileMesh.RecalculateBounds();
        tileMesh.RecalculateNormals();

        GameObject tile = new GameObject();
        tile.transform.position = _blocks[0].position + transform.position;
        tile.AddComponent<MeshFilter>().mesh = tileMesh;
        tile.AddComponent<MeshRenderer>().material = _pathMaterial;

        TilesSimulationRoot.Instance.Simulate(new Entity() {_prefab = tile, _speed = TileConfig._speed });

        _blocks.RemoveAt(0);

        RegenerateMesh();
    }

    public bool TryGetBlockIndexByPointHit(Vector3 point, out int index)
    {
        index = 0;
        if (_blocks == null || _blocks.Count == 0)
            return false;

        float min_distance = GetDistanceToBlock(0, point);

        for (int i = 1; i < _blocks.Count; i++)
        {
            var distance = GetDistanceToBlock(i, point);
            if (distance < min_distance)
            {
                min_distance = distance;
                index = i;
            }
        }

        return true;
    }

    private float GetDistanceToBlock(int index, Vector3 point)
        => (transform.position + _blocks[index].position.ToXZPlane() - point.ToXZPlane()).magnitude;

    public bool OnBlockDetected(Vector3 point, out Block block)
    {
        block = new Block();

        if (TryGetBlockIndexByPointHit(point, out int index) == false) { return false; }

        if (index == 1)
        {
            TilesSimulationRoot.Instance.Simulate(
                new Entity() { _prefab = _startPlatform, _speed = TileConfig._mainTileSpeed });
        }

        if (index + PULL_BLOCK_LENGTH >= _blocks.Count)
        {
            SpawnBlocks(PULL_BLOCK_LENGTH);
        }

        if (index > 3)
        {
            DestroyLastBlockFromBeginning();
        }

        block = _blocks[index];
        return true;
    }

    private void GenerateBlock(Block block, int index)
    {
        var blockPos = block.position;
        if(TryGetBlockAt(index - 1, out Block previous))
        {
            if (previous.direction != left_dir)
                GenerateBackSideBlock(_triangles, _vertices, blockPos);

            if (previous.direction != right_dir)
                GenerateLeftSideBlock(_triangles, _vertices, blockPos);
        }
        else
        {
            GenerateBackSideBlock(_triangles, _vertices, blockPos);
            GenerateLeftSideBlock(_triangles, _vertices, blockPos);
        }
   
        if(TryGetBlockAt(index + 1, out Block next))
        {
            if (block.direction != right_dir)
                GenerateRightSideBlock(_triangles, _vertices, blockPos);

            if (block.direction != left_dir)
                GenerateFrontSideBlock(_triangles, _vertices, blockPos);
        }
        else
        {
            GenerateRightSideBlock(_triangles, _vertices, blockPos);
            GenerateFrontSideBlock(_triangles, _vertices, blockPos); ;
        }

        GenerateUpperSideBlock(_triangles, _vertices, blockPos);
    }

    private bool TryGetBlockAt(int index, out Block blockPosition)
    {
        blockPosition = new Block();

        if (index < _blocks.Count && index > -1)
        {
            blockPosition = _blocks[index];
            return true;
        }
        else
            return false;
    }

    private void GenerateRightSideBlock(List<int> triangles, List<Vector3> vertices, Vector3 blockPosition)
    {
        var v1 = new Vector3(BLOCK_WIDTH * 0.5f, -BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v2 = new Vector3(BLOCK_WIDTH * 0.5f, -BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);
        var v3 = new Vector3(BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v4 = new Vector3(BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);

        vertices.Add(v1 + blockPosition);
        vertices.Add(v3 + blockPosition);
        vertices.Add(v2 + blockPosition);
        vertices.Add(v4 + blockPosition);

        CalculateTrianglesForth(triangles, vertices.Count);
    }
    private void GenerateLeftSideBlock(List<int> triangles, List<Vector3> vertices, Vector3 blockPosition)
    {
        var v1 = new Vector3(-BLOCK_WIDTH * 0.5f, -BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v2 = new Vector3(-BLOCK_WIDTH * 0.5f, -BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);
        var v3 = new Vector3(-BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v4 = new Vector3(-BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);

        vertices.Add(v1 + blockPosition);
        vertices.Add(v3 + blockPosition);
        vertices.Add(v2 + blockPosition);
        vertices.Add(v4 + blockPosition);

        CalculateTrianglesBack(triangles, vertices.Count);
    }

    private void GenerateUpperSideBlock(List<int> triangles, List<Vector3> vertices, Vector3 blockPosition)
    {
        var v1 = new Vector3(-BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v2 = new Vector3(-BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);
        var v3 = new Vector3(BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v4 = new Vector3(BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);

        vertices.Add(v1 + blockPosition);
        vertices.Add(v2 + blockPosition);
        vertices.Add(v3 + blockPosition);
        vertices.Add(v4 + blockPosition);

        CalculateTrianglesForth(triangles, vertices.Count);
    }

    private void GenerateFrontSideBlock(List<int> triangles, List<Vector3> vertices, Vector3 blockPosition)
    {
        var v1 = new Vector3(-BLOCK_WIDTH * 0.5f, -BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);
        var v2 = new Vector3(-BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);
        var v3 = new Vector3(BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);
        var v4 = new Vector3(BLOCK_WIDTH * 0.5f, -BLOCK_HEIGHT * 0.5f, BLOCK_WIDTH * 0.5f);

        vertices.Add(v3 + blockPosition);
        vertices.Add(v4 + blockPosition);
        vertices.Add(v2 + blockPosition);
        vertices.Add(v1 + blockPosition);


        CalculateTrianglesBack(triangles, vertices.Count);
    }

    private void GenerateBackSideBlock(List<int> triangles, List<Vector3> vertices, Vector3 blockPosition)
    {
        var v1 = new Vector3(-BLOCK_WIDTH * 0.5f, -BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v2 = new Vector3(-BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v3 = new Vector3(BLOCK_WIDTH * 0.5f, BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);
        var v4 = new Vector3(BLOCK_WIDTH * 0.5f, -BLOCK_HEIGHT * 0.5f, -BLOCK_WIDTH * 0.5f);

        vertices.Add(v3 + blockPosition);
        vertices.Add(v4 + blockPosition);
        vertices.Add(v2 + blockPosition);
        vertices.Add(v1 + blockPosition);


        CalculateTrianglesForth(triangles, vertices.Count);
    }

    private void CalculateTrianglesForth(List<int> triangles, int verticesCount)
    {
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);

        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 1);
        triangles.Add(verticesCount - 2);
    }

    private void CalculateTrianglesBack(List<int> triangles, int verticesCount)
    {
        triangles.Add(verticesCount - 4);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 3);

        triangles.Add(verticesCount - 3);
        triangles.Add(verticesCount - 2);
        triangles.Add(verticesCount - 1);
    }

}


public static class Vector3Extension
{
    public static Vector3 ToXZPlane(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
}