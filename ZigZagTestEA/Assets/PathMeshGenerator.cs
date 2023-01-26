using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class PathMeshGenerator : MonoBehaviour
{
    [SerializeField, Header("Diagnostics: (do not modify)")]
    private int _blocksCount = 0;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private Mesh _customMesh;

    public static readonly Vector3 right_dir = Vector3.right;
    public static readonly Vector3 forward_dir = Vector3.forward;

    private const float BLOCK_WIDTH = 1f;
    private const float BLOCK_HEIGHT = 5f;
    private const int PULL_BLOCK_LENGTH = 20;

    private readonly int _startCount = 10;

    private List<Block> _blocks = new List<Block>();

    private Material _pathMaterial;

    private Vector3 getDirectionOffset
    {
        get
        {
            // TO DO
            if(_blocksCount < _startCount)
            {
                return right_dir;
            }    


            return Random.value < 0.5f ? right_dir : forward_dir;
        }
    }


    public struct Block 
    {
        public Vector3 direction;
        public Vector3 position;
    }


    private void Awake()
    {
        _pathMaterial = GetComponent<MeshRenderer>().material;
        _blocksCount = 0;
    }

    private void Start()
    {
        SpawnBlocks(PULL_BLOCK_LENGTH);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
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
        if (_blocks.Count == 0)
            lastPosition = Vector3.zero;
        else
            lastPosition = _blocks[_blocks.Count - 1].position;

        for (int i = 0; i < length; i++)
        {
            var dir = getDirectionOffset;
            _blocks.Add(new Block { position = lastPosition, direction = dir});
            _blocksCount = _blocks.Count;
            lastPosition += dir * BLOCK_WIDTH;
        }

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

        TilesSimulationRoot.Instance.Simulate(tile);

        _blocks.RemoveAt(0);

        RegenerateMesh();
    }

    public bool TryGetBlockIndexByPointHit(Vector3 point, out int index)
    {
        index = 0;
        if (_blocks == null || _blocks.Count == 0)
            return false;

        float min_distance = (_blocks[0].position.ToXZPlane() - point.ToXZPlane()).magnitude;

        for (int i = 1; i < _blocks.Count; i++)
        {
            var distance = (_blocks[i].position.ToXZPlane() - point.ToXZPlane()).magnitude;
            if (distance < min_distance)
            {
                min_distance = distance;
                index = i;
            }
        }

        return true;
    }

    public void TryGenerateNextBlocks(Vector3 point)
    {
        if(TryGetBlockIndexByPointHit(point, out int index))
        {
            if(index + PULL_BLOCK_LENGTH * 0.3 >= _blocks.Count)
            {
                SpawnBlocks(PULL_BLOCK_LENGTH);
            }
        }
    }

    private void GenerateBlock(Block block, int index)
    {
        var blockPos = block.position;
        if(TryGetBlockAt(index - 1, out Block previous))
        {
            if (previous.direction != forward_dir)
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

            if (block.direction != forward_dir)
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