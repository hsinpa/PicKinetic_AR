using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Study {
    public class CubeMesh : MonoBehaviour
    {

        [SerializeField]
        private MeshFilter meshFilter;


        private Vector3[] vertices;
        private int[] triangle;
        private Vector2[] uv;

        private void Start()
        {
            meshFilter.mesh = DrawQuad();
        }

        private Mesh DrawQuad() {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[6];
            int[] triangle = new int[6];
            Vector2[] uv = new Vector2[6];

            vertices[0] = new Vector3(-0.5f, 0, 0); // A
            vertices[1] = new Vector3(-0.5f, 1, 0); // B
            vertices[2] = new Vector3(0.5f, 0, 0); // C

            vertices[3] = new Vector3(0.5f, 0, 0); // A
            vertices[4] = new Vector3(-0.5f, 1, 0); // B
            vertices[5] = new Vector3(0.5f, 1, 0); // D

            for (int i = 0; i < 6; i++)
                triangle[i] = i;

            for (int i = 0; i < 6; i++)
            {
                uv[i] = new Vector2(vertices[i].x + 0.5f, vertices[i].y);
            }

            mesh.vertices = vertices;
            mesh.triangles = triangle;
            mesh.uv = uv;

            mesh.RecalculateNormals();

            return mesh;
        }

        

    }
}
