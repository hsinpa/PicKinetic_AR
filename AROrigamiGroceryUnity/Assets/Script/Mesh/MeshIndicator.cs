using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshIndicator : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;

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
    private RaycastOverallResult raycastOverallResult;
    private IndictatorData indictatorData = new IndictatorData();
    private float finalSizeValue;

    public void SetUp(Camera camera, RaycastMethod raycastMethod) {
        this.raycastResultArray = new Vector3[4];
        this.screenPosArray = new Vector2[4];
        this.raycastOverallResult = new RaycastOverallResult();
        this.raycastOverallResult.raycastResults = this.raycastResultArray;

        this._meshCube = new MeshCube();
        this._camera = camera;
        this.geneticRaycast = raycastMethod;

        this.meshFilter.mesh = this._meshCube.mesh;

    }

    public IndictatorData GetRelativePosRot(Vector2 positionRatio) {
        indictatorData.rotation = transform.rotation;

        float x = (Mathf.Lerp(leftBottom.x, rightBottom.x, positionRatio.x) * (finalSizeValue / Screen.width)) + transform.position.x;
        float z = (Mathf.Lerp(leftTop.y, leftBottom.y,positionRatio.y) *  (finalSizeValue / Screen.height)) + transform.position.z;

        Debug.Log(string.Format("leftBottom.x {0}, rightBottom.x {1}", leftBottom.x, rightBottom.x));

        Debug.Log(string.Format("leftBottom.y {0}, leftTop.y {1}", leftBottom.y, leftTop.y));

        indictatorData.position.Set(x, transform.position.y, z);

        return indictatorData;
    }

    public void DisplayOnScreenPos(TextureUtility.TextureStructure textureStructure, float indicatorSizeStr) {

        int width = Screen.width, height = Screen.height;

        leftBottom.Set(width * textureStructure.xResidualRatio, height * textureStructure.yResidualRatio);
        leftTop.Set(width * textureStructure.xResidualRatio, height * textureStructure.yRatio);
        rightBottom.Set(width - (width * textureStructure.xResidualRatio), height * textureStructure.yResidualRatio);
        rightTop.Set(width - (width * textureStructure.xResidualRatio),
                    height - (height * textureStructure.yResidualRatio));

        Debug.Log(string.Format("yResidualRatio {0}, yRatio {1}", textureStructure.yResidualRatio, textureStructure.yRatio));

        Debug.Log(string.Format("LeftBotm {0}, LeftTop {1}, RightBottom {2}, RightTop {3}", leftBottom, leftTop, rightBottom, rightTop));

        screenPosArray[0] = leftBottom;
        screenPosArray[1] = leftTop;
        screenPosArray[2] = rightTop;
        screenPosArray[3] = rightBottom;

        RaycastOverallResult raycastR = FindFourCornerOfIndicator(screenPosArray);

        Mesh meshResult = _meshCube.CreateMesh(raycastR.raycastResults);
        meshFilter.mesh = meshResult;

        centerPoint.Set(0.5f, 0.5f);

        var centerResult = geneticRaycast(centerPoint);
        if (centerResult.hasHit) {
            meshFilter.transform.position = centerResult.hitPoint;

            finalSizeValue = (indicatorSizeStr * 0.75f);

            float sizeMagnitue = (_camera.transform.position - meshFilter.transform.position).magnitude * finalSizeValue;
            meshFilter.transform.localScale = new Vector3(sizeMagnitue, sizeMagnitue, sizeMagnitue);

            var cameraForward = _camera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z);
            meshFilter.transform.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private RaycastOverallResult FindFourCornerOfIndicator(Vector2[] screenPosArray) {
        int cornerLen = screenPosArray.Length;
        raycastOverallResult.allHits = true;

        for (int i = 0; i < cornerLen; i++) {
            var r = this.geneticRaycast(screenPosArray[i]);
            raycastOverallResult.raycastResults[i] = r.hitPoint;

            if (!r.hasHit) {
                raycastOverallResult.allHits = false;

                break;
            }
        }

        return raycastOverallResult;
    }

    private struct RaycastOverallResult {
        public Vector3[] raycastResults;
        public bool allHits;
    }

    public struct IndictatorData {
        public Vector3 position;
        public Quaternion rotation;
    }
}
