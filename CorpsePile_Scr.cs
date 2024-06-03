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

        pileMesh.RecalculateNormals();

        transform.GetComponent<MeshFilter>().mesh = pileMesh;
        transform.GetComponent<MeshRenderer>().material = material;
    }


    private Vector3[] GenerateVertices()
    {
        int totalVertCount = 16 * meshSizeX * meshSizeZ;
        Vector3[] vertices = new Vector3[totalVertCount];

        for (int k = 0; k < meshSizeZ; k++)
        {
            for (int i = 0; i < meshSizeX; i++)
            {
                OneCellVerticesGen(i, k).CopyTo(vertices, k * meshSizeX * 16 + i * 16);
            }
        }

        return vertices;
    }
    private Vector3[] OneCellVerticesGen(int x, int z)
    {
        Vector3[] cellVertices = new Vector3[16];

        cellVertices[0] = new Vector3(x - 0.5f, 0, meshSizeZ - z - 0.5f);
        cellVertices[1] = new Vector3(x - 0.25f, 0, meshSizeZ - z - 0.5f);
        cellVertices[2] = new Vector3(x + 0.25f, 0, meshSizeZ - z - 0.5f);
        cellVertices[3] = new Vector3(x + 0.5f, 0, meshSizeZ - z - 0.5f);
        cellVertices[4] = new Vector3(x - 0.5f, 0, meshSizeZ - z - 0.75f);
        cellVertices[5] = new Vector3(x - 0.25f, 0, meshSizeZ - z - 0.75f);
        cellVertices[6] = new Vector3(x + 0.25f, 0, meshSizeZ - z - 0.75f);
        cellVertices[7] = new Vector3(x + 0.5f, 0, meshSizeZ - z - 0.75f);
        cellVertices[8] = new Vector3(x - 0.5f, 0, meshSizeZ - z - 1.25f);
        cellVertices[9] = new Vector3(x - 0.25f, 0, meshSizeZ - z - 1.25f);
        cellVertices[10] = new Vector3(x + 0.25f, 0, meshSizeZ - z - 1.25f);
        cellVertices[11] = new Vector3(x + 0.5f, 0, meshSizeZ - z - 1.25f);
        cellVertices[12] = new Vector3(x - 0.5f, 0, meshSizeZ - z - 1.5f);
        cellVertices[13] = new Vector3(x - 0.25f, 0, meshSizeZ - z - 1.5f);
        cellVertices[14] = new Vector3(x + 0.25f, 0, meshSizeZ - z - 1.5f);
        cellVertices[15] = new Vector3(x + 0.5f, 0, meshSizeZ - z - 1.5f);

        return cellVertices;
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
        int totalTriCount = 54 * meshSizeX * meshSizeZ;
        int[] triangles = new int[totalTriCount];

        for (int k = 0; k < meshSizeZ; k++)
        {
            for (int i = 0; i < meshSizeX; i++)
            {
                TrianglesForCell(i, k).CopyTo(triangles, k * meshSizeX * 54 + i * 54);
            }
        }

        return triangles;
    }
    private int[] TrianglesForCell(int x, int z)
    {
        int[] cellTris = new int[54];
        int startingIndex = meshSizeZ * 54 * z + 54 * x;


        cellTris.AddTriangle(0, startingIndex + 0, startingIndex + 1, startingIndex + 5);
        cellTris.AddTriangle(3, startingIndex + 0, startingIndex + 5, startingIndex + 4);
        cellTris.AddTriangle(6, startingIndex + 1, startingIndex + 2, startingIndex + 5);
        cellTris.AddTriangle(9, startingIndex + 5, startingIndex + 2, startingIndex + 6);
        cellTris.AddTriangle(12, startingIndex + 2, startingIndex + 3, startingIndex + 6);
        cellTris.AddTriangle(15, startingIndex + 6, startingIndex + 3, startingIndex + 7);
        cellTris.AddTriangle(18, startingIndex + 4, startingIndex + 5, startingIndex + 8);
        cellTris.AddTriangle(21, startingIndex + 8, startingIndex + 5, startingIndex + 9);
        cellTris.AddTriangle(24, startingIndex + 5, startingIndex + 6, startingIndex + 9);
        cellTris.AddTriangle(27, startingIndex + 9, startingIndex + 6, startingIndex + 10);
        cellTris.AddTriangle(30, startingIndex + 6, startingIndex + 7, startingIndex + 10);
        cellTris.AddTriangle(33, startingIndex + 10, startingIndex + 7, startingIndex + 11);
        cellTris.AddTriangle(36, startingIndex + 8, startingIndex + 9, startingIndex + 12);
        cellTris.AddTriangle(39, startingIndex + 12, startingIndex + 9, startingIndex + 13);
        cellTris.AddTriangle(42, startingIndex + 9, startingIndex + 10, startingIndex + 13);
        cellTris.AddTriangle(45, startingIndex + 13, startingIndex + 10, startingIndex + 14);
        cellTris.AddTriangle(48, startingIndex + 10, startingIndex + 15, startingIndex + 14);
        cellTris.AddTriangle(51, startingIndex + 10, startingIndex + 11, startingIndex + 15);

        return cellTris;
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
