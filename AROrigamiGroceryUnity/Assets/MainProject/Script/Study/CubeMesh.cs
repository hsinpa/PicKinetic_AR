using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PicKinetic;

namespace Hsinpa.Study {
    public class CubeMesh : MonoBehaviour
    {

        //[SerializeField]
        //private MeshFilter meshFilter;

        [SerializeField]
        MeshObject meshObject;

        private Vector3[] vertices;
        private int[] triangle;
        private Vector2[] uv;

        private Mesh2DTo3D mesh2DTo3D;

        private void Start()
        {
            mesh2DTo3D = new Mesh2DTo3D();
            Form3DMesh();
            //meshFilter.mesh = mesh2DTo3D.Convert(meshData, meshData.borderVertices);
        }

        private void Form3DMesh()
        {
            Mesh mesh = new Mesh();
            var meshData = DrawQuad();

            var d = mesh2DTo3D.Convert(meshData, meshData.borderVertices);

            mesh.vertices = d.vertices;
            mesh.triangles = d.triangles;
            mesh.uv = d.uv;
            mesh.colors = d.colors;
            

            mesh.RecalculateNormals();

            meshObject.SetMesh(mesh, null, size: 512);
            meshObject.SetControlPoint(d.topVertice, d.bottomVertice);
            meshObject.GenerateControlPoints();
        }

        private MarchingCube.MarchingCubeResult DrawQuad() {
            MarchingCube.MarchingCubeResult marchingCubeResult = new MarchingCube.MarchingCubeResult();
            Vector3[] vertices = new Vector3[4];
            int[] triangle = new int[6];
            Vector2[] uv = new Vector2[4];

            vertices[0] = new Vector3(-0.5f, 0, 0); // A
            vertices[1] = new Vector3(-0.5f, 0, 1); // B
            vertices[2] = new Vector3(0.5f, 0, 1); // C
            vertices[3] = new Vector3(0.5f, 0, 0); // D

            //vertices[3] = new Vector3(0.5f, 0, 0); // A
            //vertices[4] = new Vector3(-0.5f, 1, 0); // B
            //vertices[5] = new Vector3(0.5f, 1, 0); // D

            triangle[0] = 0;
            triangle[1] = 1;
            triangle[2] = 2;
            triangle[3] = 2;
            triangle[4] = 3;
            triangle[5] = 0;

            for (int i = 0; i < 4; i++)
            {
                uv[i] = new Vector2(vertices[i].x + 0.5f, vertices[i].z);
            }

            marchingCubeResult.vertices = vertices;
            marchingCubeResult.triangles = triangle;
            marchingCubeResult.uv = uv;
            marchingCubeResult.borderVertices = vertices;
            

            return marchingCubeResult;
        }

        

    }
}
