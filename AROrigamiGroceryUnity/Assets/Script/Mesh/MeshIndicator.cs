using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami
{
    public class MeshIndicator : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter meshFilter;

        private MeshRenderer _meshRender;

        //[SerializeField, Range(0.001f, 1f)]
        //private float IndicatorSizeStr = 0.1f;

        public delegate TextureUtility.RaycastResult RaycastMethod(Vector2 screenPos);
        private RaycastMethod geneticRaycast;

        private Camera _camera;

        private MeshCube _meshCube;
        private Vector2 leftBottom = new Vector2(),
                        leftTop = new Vector2(),
                        rightBottom = new Vector2(),
                        rightTop = new Vector2(),
                        centerPoint = new Vector2();

        private Vector2[] screenPosArray;
        private Vector3[] raycastResultArray;
        private TextureUtility.RaycastResult helpOrientPoint;

        private RaycastOverallResult raycastOverallResult;
        private IndictatorData indictatorData = new IndictatorData();
        private float finalSizeValue;
        private Vector3 localScaleValue = new Vector3();

        public void SetUp(Camera camera, RaycastMethod raycastMethod)
        {
            this.raycastResultArray = new Vector3[4];
            this.screenPosArray = new Vector2[4];
            this.raycastOverallResult = new RaycastOverallResult();
            this.raycastOverallResult.raycastResults = this.raycastResultArray;

            this._meshCube = new MeshCube();
            this._camera = camera;
            this.geneticRaycast = raycastMethod;

            this.meshFilter.mesh = this._meshCube.mesh;
            this._meshRender = this.GetComponent<MeshRenderer>();

        }

        public IndictatorData GetRelativePosRot(Vector2 positionRatio)
        {

            var cameraDir = (helpOrientPoint.hitPoint - transform.position).normalized;
            cameraDir.y = 0;

            if (cameraDir != ParameterFlag.General.VectorZero)
                indictatorData.rotation = Quaternion.LookRotation(cameraDir);

            //if (raycastOverallResult.allHits)
            //Times 5f, for red border
            var bound = _meshRender.bounds.extents * 0.2f;
            var front = _meshRender.transform.forward;
            var right = _meshRender.transform.right;

            var boundX = Mathf.Lerp(-bound.x, bound.x, positionRatio.x);
            var boundZ = Mathf.Lerp(-bound.z, bound.z, positionRatio.y);

            //Debug.Log("boundX " + boundX + ", boundZ " + boundZ);
            //Debug.Log("Extend " + bound);

            Vector3 xAxis = boundZ * right;
            Vector3 zAxis = boundX * front;
            Vector3 finalAxis = xAxis + zAxis + transform.position;

            //Debug.Log(string.Format("positionRatio {0}, x {1}, z {2}", positionRatio, finalAxis.x, finalAxis.z));

            indictatorData.position.Set(finalAxis.x, transform.position.y, finalAxis.z);

            return indictatorData;
        }

        public void DisplayOnScreenPos(TextureUtility.TextureStructure textureStructure, float indicatorSizeStr)
        {

            int width = Screen.width, height = Screen.height;

            leftBottom.Set(width * textureStructure.xResidualRatio, height * textureStructure.yResidualRatio);
            leftTop.Set(width * textureStructure.xResidualRatio, height * textureStructure.yRatio);
            rightBottom.Set(width - (width * textureStructure.xResidualRatio), height * textureStructure.yResidualRatio);
            rightTop.Set(width - (width * textureStructure.xResidualRatio),
                        height - (height * textureStructure.yResidualRatio));

            //Debug.Log(string.Format("yResidualRatio {0}, yRatio {1}", textureStructure.yResidualRatio, textureStructure.yRatio));

            //Debug.Log(string.Format("LeftBotm {0}, LeftTop {1}, RightBottom {2}, RightTop {3}", leftBottom, leftTop, rightBottom, rightTop));

            screenPosArray[0] = leftBottom;
            screenPosArray[1] = leftTop;
            screenPosArray[2] = rightTop;
            screenPosArray[3] = rightBottom;

            RaycastOverallResult raycastR = FindFourCornerOfIndicator(screenPosArray);

            Mesh meshResult = _meshCube.CreateMesh(raycastR.raycastResults);
            meshFilter.mesh = meshResult;

            centerPoint.Set(0.5f, 0.5f);

            var centerResult = geneticRaycast(centerPoint);

            if (centerResult.hasHit)
            {
                meshFilter.transform.position = centerResult.hitPoint;

                finalSizeValue = (_camera.transform.position - meshFilter.transform.position).magnitude * (indicatorSizeStr * 0.74f);
                localScaleValue.Set(finalSizeValue, finalSizeValue, finalSizeValue);
                meshFilter.transform.localScale = localScaleValue;

                var cameraForward = _camera.transform.forward;
                cameraForward.Normalize();
                cameraForward.y = 0;
                meshFilter.transform.rotation = Quaternion.LookRotation(cameraForward);
            }

            centerPoint.y = 0.51f;
            helpOrientPoint = geneticRaycast(centerPoint);
        }

        private RaycastOverallResult FindFourCornerOfIndicator(Vector2[] screenPosArray)
        {
            int cornerLen = screenPosArray.Length;
            raycastOverallResult.allHits = true;

            for (int i = 0; i < cornerLen; i++)
            {
                var r = this.geneticRaycast(screenPosArray[i]);
                raycastOverallResult.raycastResults[i] = r.hitPoint;

                if (!r.hasHit)
                {
                    raycastOverallResult.allHits = false;

                    break;
                }
            }

            return raycastOverallResult;
        }

        private struct RaycastOverallResult
        {
            public Vector3[] raycastResults;
            public bool allHits;
        }

        public struct IndictatorData
        {
            public Vector3 position;
            public Quaternion rotation;
        }
    }
}