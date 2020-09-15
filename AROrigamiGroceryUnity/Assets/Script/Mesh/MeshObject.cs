using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;


namespace AROrigami {
    public class MeshObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject controlPointPrefab;

        [SerializeField]
        private Transform controlPointsHolder;

        [SerializeField]
        private Animator _animator;

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

        private ControlPoints ctrlPoints;

        private void Awake()
        {
            _mesh = new Mesh();
            _ori_quaterion = transform.rotation;
        }

        public ControlPoints SetControlPoint(Vector3 topVertice, Vector3 bottomVertice) {
            ctrlPoints = new ControlPoints();

            ctrlPoints.top_control_point = topVertice;
            ctrlPoints.bottom_control_point = bottomVertice;
            ctrlPoints.center_control_point = new Vector3(0, (topVertice.y + bottomVertice.y) * 0.5f ,0);

            return ctrlPoints;
        }

        public void GenerateControlPoints() {
            UtilityMethod.ClearChildObject(controlPointsHolder);

            GameObject c_point1 = UtilityMethod.CreateObjectToParent(controlPointsHolder, controlPointPrefab);
            c_point1.transform.localPosition = ctrlPoints.top_control_point;
            c_point1.transform.name = "top_control_point";

            GameObject c_point2 = UtilityMethod.CreateObjectToParent(controlPointsHolder, controlPointPrefab);
            c_point2.transform.localPosition = ctrlPoints.bottom_control_point;
            c_point2.transform.name = "bottom_control_point";

            GameObject c_point3 = UtilityMethod.CreateObjectToParent(controlPointsHolder, controlPointPrefab);
            c_point3.transform.localPosition = ctrlPoints.center_control_point;
            c_point3.transform.name = "center_control_point";
        }


        public void SetMesh(Mesh mesh, RenderTexture uvTexture, int size) {
            if (_meshRenderer == null) return;

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

        private void OnDestroy()
        {
            _meshRenderer = null;
        }


        public struct ControlPoints {
            public Vector3 top_control_point;
            public Vector3 bottom_control_point;
            public Vector3 center_control_point;
        }
    }
}