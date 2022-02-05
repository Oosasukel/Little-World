using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Polygon
{
    public List<int> vertices = new List<int>();
    public List<int[]> triangles = new List<int[]>();
    public int middleVertice;
    public List<Polygon> neighbors = new List<Polygon>();
    public bool verticesOrganized;

    public Polygon(int middleVertice)
    {
        this.middleVertice = middleVertice;
    }

    public bool HasVertice(int vertice)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            var currentVertice = vertices[i];

            if (currentVertice == vertice) return true;
        }

        return false;
    }

    public void GetNeighborsByVertice(int vertice, out Polygon neighborA, out Polygon neighborB)
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            var currentNeghbor = neighbors[i];

            if (currentNeghbor.HasVertice(vertice))
            {
                var nextNeighbor = i + 1 == neighbors.Count ? neighbors[0] : neighbors[i + 1];

                if (nextNeighbor.HasVertice(vertice))
                {
                    neighborA = currentNeghbor;
                    neighborB = nextNeighbor;
                    return;
                }
                else
                {
                    var prevousNeighbor = i - 1 < 0 ? neighbors[neighbors.Count - 1] : neighbors[i - 1];

                    neighborA = prevousNeighbor;
                    neighborB = currentNeghbor;
                    return;
                }
            }
        }

        neighborA = null;
        neighborB = null;
    }

    public bool ContainsEdge(int verticeA, int verticeB)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            var myVerticeA = vertices[i];
            var myVerticeB = i + 1 == vertices.Count ? vertices[0] : vertices[i + 1];

            if (myVerticeA == verticeB && myVerticeB == verticeA) return true;
        }

        return false;
    }

    public void OrganizeVertices()
    {
        if (verticesOrganized || vertices.Count == 6) return;

        var newVertices = new List<int>();
        newVertices.Add(vertices[0]);
        newVertices.Add(vertices[1]);

        var edges = vertices.Count / 2;

        var verticeB = vertices[1];

        while (newVertices.Count != vertices.Count)
        {
            for (int i = 0; i < edges; i++)
            {
                var verticeA = vertices[i * 2];

                if (verticeB == verticeA)
                {
                    verticeB = vertices[i * 2 + 1];
                    newVertices.Add(verticeB);
                    break;
                }
            }
        }

        vertices = newVertices.Distinct().ToList();

        verticesOrganized = true;
    }
}

public class PlanetGenerator : MonoBehaviour
{
    [SerializeField]
    private Mesh planetMesh;
    List<Polygon> polygons;

    public void Test()
    {
        var normal = CalculateNormal(planetMesh.triangles[0]);

        Debug.DrawLine(planetMesh.vertices[planetMesh.triangles[0]], planetMesh.vertices[planetMesh.triangles[1]], Color.red, 40);
        Debug.DrawLine(planetMesh.vertices[planetMesh.triangles[1]], planetMesh.vertices[planetMesh.triangles[2]], Color.red, 40);
        Debug.DrawLine(planetMesh.vertices[planetMesh.triangles[2]], planetMesh.vertices[planetMesh.triangles[0]], Color.red, 40);

        DrawVector(normal.normalized, Color.blue);

        var normal2 = CalculateNormal(planetMesh.triangles[3]);
        Debug.DrawLine(planetMesh.vertices[planetMesh.triangles[3]], planetMesh.vertices[planetMesh.triangles[4]], Color.red, 40);
        Debug.DrawLine(planetMesh.vertices[planetMesh.triangles[4]], planetMesh.vertices[planetMesh.triangles[5]], Color.red, 40);
        Debug.DrawLine(planetMesh.vertices[planetMesh.triangles[5]], planetMesh.vertices[planetMesh.triangles[3]], Color.red, 40);
        DrawVector(normal2.normalized, Color.blue);
    }

    private Vector3 CalculateNormal(int firstIndex)
    {
        return Vector3.Cross(planetMesh.vertices[firstIndex + 1] - planetMesh.vertices[firstIndex], planetMesh.vertices[firstIndex + 2] - planetMesh.vertices[firstIndex]);
    }

    public void DrawVector(Vector3 vector, Color color)
    {
        Debug.DrawLine(Vector3.zero, vector, color, 40);
    }
}
