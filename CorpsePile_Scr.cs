using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CorpsePile_Scr : MonoBehaviour
{
    public static CorpsePile_Scr instance;

    public Material material;

    Mesh pileMesh;

    int meshSizeX = 0, meshSizeZ = 0;
    Vector3[] vertices = new Vector3[0];
    Vector2[] uv = new Vector2[0];
    int[] triangles = new int[0];

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public void GenerateMesh()
    {
        meshSizeX = Field_Scr.mapWidth;
        meshSizeZ = Field_Scr.mapHeight;

        pileMesh = new Mesh();

        vertices = GenerateVertices();
        uv = GenerateUV();
        triangles = GenerateTriangles();


        pileMesh.vertices = vertices;
        pileMesh.uv = uv;
        pileMesh.triangles = triangles;

        transform.GetComponent<MeshFilter>().mesh = pileMesh;
        transform.GetComponent<MeshRenderer>().material = material;
    }


    private Vector3[] GenerateVertices()
    {
        int totalVertCount = 9*meshSizeX*meshSizeZ + 3*meshSizeX + 3*meshSizeZ + 1;
        Vector3[] vertices = new Vector3[totalVertCount];

        for (int k = 0; k < meshSizeZ; k++)
        {
            for (int i = 0; i < meshSizeX; i++)
            {
                OneCellVerticesGen(i, k).CopyTo(vertices, k*meshSizeX*9 + 3*k + 9*i);
            }
            LastColumnVerticesGen(k).CopyTo(vertices, (k + 1)*meshSizeX*9 + 3*k);
        }
        LastRowVerticesGen().CopyTo(vertices, meshSizeZ * (9 * meshSizeX + 3));

        return vertices;

    }
    private Vector3[] OneCellVerticesGen(int x, int z)
    {
        Vector3[] cellVertices = new Vector3[9];

        cellVertices[0] = new Vector3(x - 0.5f, 0, meshSizeZ - z - 0.5f);
        cellVertices[1] = new Vector3(x - 0.25f, 0, meshSizeZ - z - 0.5f);
        cellVertices[2] = new Vector3(x + 0.25f, 0, meshSizeZ - z - 0.5f);
        cellVertices[3] = new Vector3(x - 0.5f, 0, meshSizeZ - z - 0.75f);
        cellVertices[4] = new Vector3(x - 0.25f, 0, meshSizeZ - z - 0.75f);
        cellVertices[5] = new Vector3(x + 0.25f, 0, meshSizeZ - z - 0.75f);
        cellVertices[6] = new Vector3(x - 0.5f, 0, meshSizeZ - z - 1.25f);
        cellVertices[7] = new Vector3(x - 0.25f, 0, meshSizeZ - z - 1.25f);
        cellVertices[8] = new Vector3(x + 0.25f, 0, meshSizeZ - z - 1.25f);

        return cellVertices;
    }
    private Vector3[] LastColumnVerticesGen(int z)
    {
        Vector3[] clmnVertices = new Vector3[3];

        clmnVertices[0] = new Vector3(meshSizeX - 0.5f, 0, meshSizeZ - z - 0.5f);
        clmnVertices[1] = new Vector3(meshSizeX - 0.5f, 0, meshSizeZ - z - 0.75f);
        clmnVertices[2] = new Vector3(meshSizeX - 0.5f, 0, meshSizeZ - z - 1.25f);

        return clmnVertices;
    }
    private Vector3[] LastRowVerticesGen()
    {
        Vector3[] rowVerices = new Vector3[meshSizeX * 3 + 1];

        for (int i = 0; i < meshSizeX; i++)
        {
            rowVerices[i * 3] = new Vector3(i - 0.5f, 0, -0.5f);
            rowVerices[i * 3 + 1] = new Vector3(i - 0.25f, 0, -0.5f);
            rowVerices[i * 3 + 2] = new Vector3(i + 0.25f, 0, -0.5f);
        }
        rowVerices[rowVerices.Length - 1] = new Vector3(meshSizeX - 0.5f, 0, -0.5f);

        return rowVerices;
    }

    private Vector2[] GenerateUV()
    {
        int verticesCount = vertices.Length;
        Vector2[] uv = new Vector2[verticesCount];
        
        for (int i = 0; i < verticesCount; i++)
        {
            uv[i] = new Vector2(vertices[i].x / (meshSizeX + 1), vertices[i].z / (meshSizeZ + 1));
        }

        return uv;
    }
    private int[] GenerateTriangles()
    {
        int[] triangles = new int[meshSizeX * meshSizeZ * 54];

        triangles.AddTriangle(0, 0, 1, 4);
        triangles.AddTriangle(3, 0, 4, 3);
        triangles.AddTriangle(6, 1, 2, 4);
        triangles.AddTriangle(9, 2, 5, 4);
        triangles.AddTriangle(12, 2, 9, 5);
        triangles.AddTriangle(15, 9, 12, 5);
        triangles.AddTriangle(18, 3, 7, 6);
        triangles.AddTriangle(21, 3, 4, 7);
        triangles.AddTriangle(24, 4, 5, 8);
        triangles.AddTriangle(27, 4, 8, 7);
        triangles.AddTriangle(30, 5, 12, 15);
        triangles.AddTriangle(33, 5, 15, 8);
        triangles.AddTriangle(36, 6, 7, 93);
        triangles.AddTriangle(39, 7, 94, 93);
        triangles.AddTriangle(42, 7, 8, 94);
        triangles.AddTriangle(45, 8, 95, 94);
        triangles.AddTriangle(48, 8, 15, 102);
        triangles.AddTriangle(51, 8, 102, 95);

        return triangles;
    }
    private void TrianglesForCell(int[] triangles, int x, int z)
    {

    }


    private void OnDrawGizmosSelected()
    {
        DebugVertices();
    }
    private void DebugVertices()
    {
        foreach (Vector3 verticy in vertices)
        {
            Gizmos.DrawSphere(verticy + transform.position, 0.1f);
        }
    }
}

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static void AddTriangle(this int[] triangles, int startingIndex, int firstVerticy, int secndVerticy, int thirdVerticy)
        {
            triangles[startingIndex] = firstVerticy;
            triangles[startingIndex + 1] = secndVerticy;
            triangles[startingIndex + 2] = thirdVerticy;
        }
    }
};
