using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AROrigami
{
    public class TestDeformation : MonoBehaviour
    {
        [SerializeField]
        private Sprite targetSprite;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private Renderer meshRenderer;

        [SerializeField]
        private DeformControlPoint[] controllerPoints;

        private SpaceDeformation spaceDeformation;

        // Start is called before the first frame update
        void Start()
        {
            spaceDeformation = new SpaceDeformation();
            meshFilter.mesh = SpriteToMesh(targetSprite);
            meshRenderer.material.SetTexture("_MainTex", targetSprite.texture);

            spaceDeformation.SetUpMesh(meshFilter.mesh);
            spaceDeformation.SetUpControllerPoint(controllerPoints);
        }

        private void Update()
        {
            spaceDeformation.OnUpdate();
        }

        private void SetSpriteControlPoint()
        {

        }

        private Mesh SpriteToMesh(Sprite sprite)
        {
            Mesh mesh = new Mesh();
            mesh.SetVertices(Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList());
            mesh.SetUVs(0, sprite.uv.ToList());
            mesh.SetTriangles(Array.ConvertAll(sprite.triangles, i => (int)i), 0);

            return mesh;
        }
    }
}