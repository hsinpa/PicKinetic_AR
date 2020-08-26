using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami {
    public class MeshObject : MonoBehaviour
    {
        
        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField, Range(1, 10)]
        private float rotateSensibility = 1;

        public bool copyUVTexture = false;

        private Texture2D dstTexture;

        private MaterialPropertyBlock m_PropertyBlock;

        private Quaternion _ori_quaterion;

        private void Awake()
        {
            _ori_quaterion = transform.rotation;
        }

        public void SetMesh(Mesh mesh, Texture2D uvTexture) {
            if (m_PropertyBlock == null)
                m_PropertyBlock = new MaterialPropertyBlock();

            dstTexture = uvTexture;

            if (copyUVTexture) {
                dstTexture = new Texture2D(uvTexture.width, uvTexture.height);
                Graphics.CopyTexture(uvTexture, dstTexture);
            }

            transform.rotation = _ori_quaterion;

            meshFilter.mesh = mesh;

            m_PropertyBlock.SetTexture("_MainTex", dstTexture);
            meshRenderer.SetPropertyBlock(m_PropertyBlock);
        }

        public void Rotate(Vector3 direction) {
            var currentAngle = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler( currentAngle + (direction * rotateSensibility) );   
        }
        
    }
}