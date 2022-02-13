using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    [HideInInspector]
    public GameObject groundParent;
    [SerializeField]
    private GameObject ground;
    [SerializeField]
    [Range(1, 10)]
    private int groundHeight = 1;
    [SerializeField]
    private Material defaultMaterial;
    [HideInInspector]
    public int[] face;
    [HideInInspector]
    public Vector3[] generatorVertices;
    [HideInInspector]
    public int index;
    [HideInInspector]
    public WorldConfigSO worldConfig;

    void Start()
    {
        if (worldConfig.PolygonIsActive(index))
        {
            ShowGround();
        }
        else
        {
            HideGround();
        }
    }

    public void GenerateGround()
    {
        if (ground) DestroyImmediate(ground);
        ground = new GameObject($"Ground {index}");
        ground.transform.parent = groundParent.transform;

        MeshRenderer surfaceRenderer = ground.AddComponent<MeshRenderer>();
        surfaceRenderer.material = defaultMaterial;

        var groundMesh = GenerateGroundMesh();
        MeshFilter meshFilter = ground.AddComponent<MeshFilter>();
        meshFilter.mesh = groundMesh;

        worldConfig.SetPolygonActive(index, true);
    }

    public void DeleteGround()
    {
        if (ground)
        {
            DestroyImmediate(ground);
            worldConfig.SetPolygonActive(index, false);
        }
    }

    public void HideGround()
    {
        if (ground)
        {
            ground.SetActive(false);
            worldConfig.SetPolygonActive(index, false);
        };
    }

    public void ShowGround()
    {
        if (ground)
        {
            ground.SetActive(true);
            worldConfig.SetPolygonActive(index, true);
        }
    }

    private void CalculateGroundTriangles(out Vector3[] newVertices, out int[][] triangles)
    {
        triangles = new int[(face.Length - 2) + face.Length][];
        newVertices = new Vector3[face.Length + 1];

        for (int i = 0; i < generatorVertices.Length; i++)
        {
            // @TODO colocar o 0.1f em um lugar melhor
            newVertices[i] = generatorVertices[i].normalized * (1 + (groundHeight * 0.1f));
        }
        newVertices[face.Length] = Vector3.zero;

        // Top Triangles
        if (face.Length > 3)
        {
            for (int j = 0; j < face.Length - 2; j++)
            {
                var vertice2 = face[j + 1];
                var vertice3 = face[j + 2];

                triangles[j] = new int[3] { face[0], vertice2, vertice3 };
            }
        }
        else
        {
            triangles[0] = new int[3] { face[0], face[1], face[2] };
        }

        // Side Triangles
        for (int i = 0; i < face.Length; i++)
        {
            var previousVertice = i == 0 ? face[face.Length - 1] : face[i - 1];
            var currentVertice = face[i];

            triangles[face.Length - 2 + i] = new int[3] { currentVertice, previousVertice, face.Length };
        }
    }

    private Mesh GenerateGroundMesh()
    {
        Vector3[] meshVertices;
        int[][] meshTriangles;

        CalculateGroundTriangles(out meshVertices, out meshTriangles);

        Mesh groundMesh = new Mesh();

        int vertexCount = meshTriangles.Length * 3;

        int[] indices = new int[vertexCount];

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        // Color32[] colors = new Color32[vertexCount];

        for (int j = 0; j < meshTriangles.Length; j++)
        {
            var triangle = meshTriangles[j];

            indices[j * 3 + 0] = j * 3 + 0;
            indices[j * 3 + 1] = j * 3 + 1;
            indices[j * 3 + 2] = j * 3 + 2;

            vertices[j * 3 + 0] = meshVertices[triangle[0]];
            vertices[j * 3 + 1] = meshVertices[triangle[1]];
            vertices[j * 3 + 2] = meshVertices[triangle[2]];

            // colors[j * 3 + 0] = triangle.color;
            // colors[j * 3 + 1] = triangle.color;
            // colors[j * 3 + 2] = triangle.color;

            normals[j * 3 + 0] = meshVertices[triangle[0]];
            normals[j * 3 + 1] = meshVertices[triangle[1]];
            normals[j * 3 + 2] = meshVertices[triangle[2]];
        }

        groundMesh.vertices = vertices;
        groundMesh.normals = normals;
        // terrainMesh.colors32 = colors;

        groundMesh.SetTriangles(indices, 0);

        groundMesh.RecalculateNormals();

        return groundMesh;
    }
}
