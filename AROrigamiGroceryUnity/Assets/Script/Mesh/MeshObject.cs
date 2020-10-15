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
        private BoxCollider _collider;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private MeshRenderer _meshRenderer;

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


        #region Control Point Variable
        private ControlPoints ctrlPoints;
        private const int controlPointCount = 3;

        private Vector4[] originalPoints = new Vector4[controlPointCount];
        private Vector4[] shaderCtrlPoints = new Vector4[controlPointCount];
        private GameObject[] worldControlPoint = new GameObject[controlPointCount];
        #endregion

        private void Awake()
        {
            _mesh = new Mesh();
            _ori_quaterion = transform.rotation;
        }

        public ControlPoints SetControlPoint(Vector3 topVertice, Vector3 bottomVertice) {
            ctrlPoints = new ControlPoints();

            ctrlPoints.top_control_point = topVertice;
            ctrlPoints.bottom_control_point = bottomVertice;
            ctrlPoints.center_control_point = new Vector3(0,0, (topVertice.z + bottomVertice.z) * 0.5f);

            return ctrlPoints;
        }

        public void GenerateControlPoints() {
            UtilityMethod.ClearChildObject(controlPointsHolder);

            GameObject c_point1 = UtilityMethod.CreateObjectToParent(controlPointsHolder, controlPointPrefab);
            c_point1.transform.localPosition = ctrlPoints.top_control_point;
            c_point1.transform.name = "top_control_point";
            worldControlPoint[0] = c_point1;
            originalPoints[0] = ctrlPoints.top_control_point;

            GameObject c_point2 = UtilityMethod.CreateObjectToParent(controlPointsHolder, controlPointPrefab);
            c_point2.transform.localPosition = ctrlPoints.bottom_control_point;
            c_point2.transform.name = "bottom_control_point";
            worldControlPoint[1] = c_point2;
            originalPoints[1] = ctrlPoints.bottom_control_point;

            GameObject c_point3 = UtilityMethod.CreateObjectToParent(controlPointsHolder, controlPointPrefab);
            c_point3.transform.localPosition = ctrlPoints.center_control_point;
            c_point3.transform.name = "center_control_point";
            worldControlPoint[2] = c_point3;
            originalPoints[2] = ctrlPoints.center_control_point;

            UpdateShader("_OriControlPoints", originalPoints);
        }

        public void SetMesh(Mesh mesh, RenderTexture uvTexture, int size) {
            if (_meshRenderer == null) return;

            if (dstTexture == null) {
                dstTexture = TextureUtility.GetRenderTexture(size);
            }

            if (uvTexture != null) {

                if (copyUVTexture)
                {
                    Graphics.Blit(uvTexture, dstTexture);
                    //Graphics.CopyTexture(uvTexture, 0, 0, (int)0, (int)0, size, size, dstTexture, 0, 0, 0, 0);
                }
                else
                {
                    dstTexture = uvTexture;
                }

                m_PropertyBlock.SetTexture("_MainTex", dstTexture);
                m_PropertyBlock.SetInt("_ShowSideTex", (copyUVTexture) ? 1 : 0);
                _meshRenderer.SetPropertyBlock(m_PropertyBlock);
            }
            transform.rotation = _ori_quaterion;

            _meshFilter.mesh = mesh;
        }

        public void SetPosRotation(Vector3 p_position, Quaternion p_quaternion) {
            this.transform.position = p_position;
            this.transform.rotation = p_quaternion;

            this._collider.enabled = copyUVTexture;

            if (copyUVTexture) {
                this._collider.size = _meshRenderer.bounds.size / transform.localScale.x;
                this._collider.center = new Vector3(0, -_collider.size.y * 0.5f, 0);

                this.transform.Rotate(new Vector3(-90, 0, 0), Space.Self);

                this.transform.position = p_position + new Vector3(0, _meshRenderer.bounds.size.y*0.5f, 0);
            }

            this._rigidbody.isKinematic = !copyUVTexture;
        }

        public void Rotate(Vector3 direction) {
            var currentAngle = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler( currentAngle + (direction * rotateSensibility) );   
        }

        private void Update()
        {
            UpdateControlPoint();
            UpdateShader("_ControlPoints", shaderCtrlPoints);
        }

        private void UpdateControlPoint() {
            if (worldControlPoint == null) return;

            for (int i = 0; i < controlPointCount; i++) {
                if (worldControlPoint[i] == null) continue;

                shaderCtrlPoints[i] = worldControlPoint[i].transform.localPosition;
            }
        }

        private void UpdateShader(string variableName, Vector4[] ctrlPoints) {
            if (m_PropertyBlock == null)
                m_PropertyBlock = new MaterialPropertyBlock();

            m_PropertyBlock.SetVectorArray(variableName, ctrlPoints);

            _meshRenderer.SetPropertyBlock(m_PropertyBlock);
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