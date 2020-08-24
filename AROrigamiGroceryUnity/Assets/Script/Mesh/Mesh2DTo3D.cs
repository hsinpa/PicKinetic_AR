using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami {
    public class Mesh2DTo3D
    {
        public Mesh Convert(Mesh targetMesh) {
            targetMesh.vertices = VerticesTo3D(targetMesh.vertices);


            return targetMesh;
        }

        public Vector3[] VerticesTo3D(Vector3[] vertices) {
            int originCount = vertices.Length;
            int newCount = originCount * 2;
            Vector3[] newVertices = new Vector3[newCount];

            for (int i = 0; i < newCount; i++) {

            }

            return newVertices;
        }

    }
}
