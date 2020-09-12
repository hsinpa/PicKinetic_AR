﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami {
    public class MeshObject : MonoBehaviour
    {

        [SerializeField]
        private MeshRenderer _meshRenderer;
        public MeshRenderer meshRenderer => _meshRenderer;

        [SerializeField]
        private MeshFilter _meshFilter;

        [SerializeField, Range(1, 10)]
        private float rotateSensibility = 1;

        public bool copyUVTexture = false;

        private RenderTexture dstTexture;

        private MaterialPropertyBlock m_PropertyBlock;

        private Quaternion _ori_quaterion;

        private Mesh _mesh;
        public Mesh mesh => _mesh;

        private void Awake()
        {
            _mesh = new Mesh();
            _ori_quaterion = transform.rotation;
        }

        public void SetMesh(Mesh mesh, RenderTexture uvTexture, int size) {
            if (gameObject == null) return;

            if (m_PropertyBlock == null)
                m_PropertyBlock = new MaterialPropertyBlock();

            if (dstTexture == null) {
                dstTexture = TextureUtility.GetRenderTexture(size);
            }

            //Graphics.CopyTexture(uvTexture, dstTexture);
            if (copyUVTexture) {
                Graphics.Blit(uvTexture, dstTexture);
                //Graphics.CopyTexture(uvTexture, 0, 0, (int)0, (int)0, size, size, dstTexture, 0, 0, 0, 0);
            } else
            {
                dstTexture = uvTexture;
            }

            transform.rotation = _ori_quaterion;

            _meshFilter.mesh = mesh;

            m_PropertyBlock.SetTexture("_MainTex", dstTexture);
            m_PropertyBlock.SetInt("_ShowSideTex", (copyUVTexture) ? 1 : 0);
            _meshRenderer.SetPropertyBlock(m_PropertyBlock);
        }

        public void Rotate(Vector3 direction) {
            var currentAngle = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler( currentAngle + (direction * rotateSensibility) );   
        }
        
    }
}