using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PicKinetic
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
        private Animator meshAnimator;

        // Start is called before the first frame update
        void Start()
        {
            spaceDeformation = new SpaceDeformation();
            meshFilter.mesh = SpriteToMesh(targetSprite);
            meshAnimator = meshFilter.GetComponent<Animator>();
            meshRenderer.material.SetTexture("_MainTex", targetSprite.texture);

            spaceDeformation.SetUpMesh(meshFilter.mesh);
            spaceDeformation.SetUpControllerPoint(controllerPoints);
        }

        private void Update()
        {
            spaceDeformation.OnUpdate();

            if (Input.GetMouseButtonDown(0)) {
                meshAnimator.gameObject.SetActive(false);
                meshAnimator.gameObject.SetActive(true);
            }
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