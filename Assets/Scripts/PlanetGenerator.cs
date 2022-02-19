using System;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class Polyhedron
{
    public int[][] Faces { get; set; }
    public Vector3[] Vertices { get; set; }
}

public class PlanetGenerator : MonoBehaviour
{
    private Polyhedron polyhedron;
    [SerializeField]
    private WorldConfigSO worldConfig;
    private GameObject planet;
    [SerializeField]
    private Material polygonGeneratorMaterial;
    [SerializeField]
    private Material waterMaterial;

    public void GeneratePlanet()
    {
        InitPolyhedronFromJson();
        InstantiatePolygons();
        GenerateWater();
    }

    private void InitPolyhedronFromJson()
    {
        string polyJsonPath = Path.Combine(Application.streamingAssetsPath, "polyhedronisme-tdD.json");
        string polyJsonString;

        using (var reader = new StreamReader(polyJsonPath))
        {
            polyJsonString = reader.ReadToEnd();
        }

        JSONNode polyJson = SimpleJSON.JSON.Parse(polyJsonString);

        var faces = polyJson["faces"];
        var vertices = polyJson["vertices"];

        polyhedron = new Polyhedron();

        polyhedron.Faces = new int[faces.Count][];
        for (int i = 0; i < faces.Count; i++)
        {
            var currentFace = faces[i];
            var newFace = new int[currentFace.Count];
            for (int j = 0; j < currentFace.Count; j++)
            {
                newFace[j] = currentFace[j].AsInt;
            }
            polyhedron.Faces[i] = newFace;
        }

        polyhedron.Vertices = new Vector3[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            var currentVertice = vertices[i];
            // @TODO mudar o tamanho aqui proporcional a quantidade de poligonos para sempre manter o mesmo tamanho
            polyhedron.Vertices[i] = new Vector3(currentVertice[0].AsFloat, currentVertice[1].AsFloat, currentVertice[2].AsFloat);
        }
    }

    private void InstantiatePolygons()
    {
        if (planet) DestroyImmediate(planet);
        // @TODO mudar o scale do planet com base no tamanho de um polígono
        planet = new GameObject("Planet");
        planet.AddComponent<GravityAttractor>();

        var polygonsParent = new GameObject("Polygons");
        polygonsParent.transform.parent = planet.transform;
        var groundParent = new GameObject("Ground");
        groundParent.transform.parent = planet.transform;

        for (int i = 0; i < polyhedron.Faces.Length; i++)
        {
            var face = polyhedron.Faces[i];
            CreatePolygon(face, i, polygonsParent, groundParent);
        }

        worldConfig.SetPolygonsLength(polyhedron.Faces.Length);
    }

    private void CreatePolygon(int[] face, int index, GameObject polygonsParent, GameObject groundParent)
    {
        var polygon = new GameObject($"Polygon {index}");
        polygon.transform.parent = polygonsParent.transform;

        int[] polyFace;
        Vector3[] polyVertices;
        int[][] polyTriangles;

        CalculateFaceTriangles(face, polyhedron.Vertices, out polyVertices, out polyTriangles, out polyFace);

        MeshRenderer polygonRenderer = polygon.AddComponent<MeshRenderer>();
        polygonRenderer.material = polygonGeneratorMaterial;

        var polygonMesh = GenerateMesh(polyTriangles, polyVertices);
        polygonMesh.RecalculateNormals();
        MeshFilter meshFilter = polygon.AddComponent<MeshFilter>();
        meshFilter.mesh = polygonMesh;

        var groundGenerator = polygon.AddComponent<GroundGenerator>();
        groundGenerator.face = polyFace;
        groundGenerator.generatorVertices = polyVertices;
        groundGenerator.groundParent = groundParent;
        groundGenerator.worldConfig = worldConfig;
        groundGenerator.index = index;
    }

    private void CalculateFaceTriangles(int[] face, Vector3[] originalVertices, out Vector3[] newVertices, out int[][] triangles, out int[] newFace)
    {
        newFace = new int[face.Length];

        newVertices = new Vector3[face.Length];
        triangles = new int[face.Length - 2][];

        for (int j = 0; j < face.Length; j++)
        {
            var vertice = face[j];

            newVertices[j] = originalVertices[vertice];
            newFace[j] = j;
        }

        if (newFace.Length > 3)
        {
            for (int j = 0; j < newFace.Length - 2; j++)
            {
                var vertice2 = newFace[j + 1];
                var vertice3 = newFace[j + 2];

                triangles[j] = new int[3] { newFace[0], vertice2, vertice3 };
            }
        }
        else
        {
            triangles[0] = new int[3] { newFace[0], newFace[1], newFace[2] };
        }
    }

    private Mesh GenerateMesh(int[][] meshTriangles, Vector3[] meshVertices)
    {
        Mesh mesh = new Mesh();

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

        mesh.vertices = vertices;
        mesh.normals = normals;
        // terrainMesh.colors32 = colors;

        mesh.SetTriangles(indices, 0);

        return mesh;
    }

    private int[][] CalculatePolyhedronTriangles()
    {
        int[][] triangles;

        int numberOfTriangles = 0;
        foreach (var face in polyhedron.Faces)
        {
            numberOfTriangles += face.Length - 2;
        }

        triangles = new int[numberOfTriangles][];
        int currentTriangleIndex = 0;

        for (int i = 0; i < polyhedron.Faces.Length; i++)
        {
            var face = polyhedron.Faces[i];

            if (face.Length > 3)
            {
                for (int j = 0; j < face.Length - 2; j++)
                {
                    var vertice2 = face[j + 1];
                    var vertice3 = face[j + 2];

                    triangles[currentTriangleIndex++] = new int[3] { face[0], vertice2, vertice3 };
                }
            }
            else
            {
                triangles[currentTriangleIndex++] = new int[3] { face[0], face[1], face[2] };
            }
        }


        return triangles;
    }

    private void GenerateWater()
    {
        var waterParent = new GameObject("Water");
        waterParent.transform.parent = planet.transform;
        var waterSurface = new GameObject("Surface");
        waterSurface.transform.parent = waterParent.transform;
        var waterGround = new GameObject("Ground");
        waterGround.transform.parent = waterParent.transform;
        // @TODO levar em consideração o tamanho do planeta para ficar sempre a mesma altura independente do tamanho do planeta
        waterSurface.transform.localScale *= 1.01f;
        waterGround.transform.localScale *= 0.95f;

        var polyhedronTriangles = CalculatePolyhedronTriangles();

        var waterMesh = GenerateMesh(polyhedronTriangles, polyhedron.Vertices);

        MeshRenderer waterRenderer = waterSurface.AddComponent<MeshRenderer>();
        waterRenderer.material = waterMaterial;
        MeshFilter waterMeshFilter = waterSurface.AddComponent<MeshFilter>();
        waterMeshFilter.mesh = waterMesh;
        MeshCollider waterMeshCollider = waterSurface.AddComponent<MeshCollider>();
        waterMeshCollider.convex = true;
        waterMeshCollider.sharedMesh = waterMesh;
        waterMeshCollider.isTrigger = true;

        MeshRenderer waterGroundRenderer = waterGround.AddComponent<MeshRenderer>();
        waterGroundRenderer.material = polygonGeneratorMaterial;
        MeshFilter waterGroundMeshFilter = waterGround.AddComponent<MeshFilter>();
        waterGroundMeshFilter.mesh = waterMesh;
        MeshCollider waterGroundMeshCollider = waterGround.AddComponent<MeshCollider>();
        waterGroundMeshCollider.convex = true;
        waterGroundMeshCollider.sharedMesh = waterMesh;
    }

    // private void DrawFaces()
    // {
    //     foreach (var face in polyhedron.Faces)
    //     {
    //         for (int i = 0; i < face.Length; i++)
    //         {
    //             var currentVertice = face[i];
    //             var previousVertice = i == 0 ? face[face.Length - 1] : face[i - 1];

    //             Debug.DrawLine(polyhedron.Vertices[currentVertice], polyhedron.Vertices[previousVertice], Color.red, 20);
    //         }
    //     }
    // }
}