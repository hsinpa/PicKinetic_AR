using PicKinetic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class DeviceCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;

    public RawImage scalePreview;
    public RawImage linePreview;

    public Material rotateMat;

    private RenderTexture modelTexRenderer;
    private RenderTexture imageProcessRenderer;
    private RenderTexture arBackgroundRenderer;

    [SerializeField, Range(0, 0.05f)]
    private float _SizeStrength = 1;

    [SerializeField, Range(0, 1)]
    private float _CropSize = 1;

    [SerializeField]
    private ARRaycastManager _arRaycastManager;

    [SerializeField]
    private ARCameraBackground _arCameraBG;

    [SerializeField]
    private ARSession _arSession;

    [SerializeField]
    TextureMeshManager texturePreivew;

    [SerializeField]
    MeshObject meshBorder;

    [SerializeField]
    private MeshObjectManager meshObjManager;

    [SerializeField]
    private Button shotBtn;

    [SerializeField]
    private MeshIndicator meshIndicator;

    private Camera _camera;
    TextureUtility TextureUtility;
    TextureUtility.TextureStructure _textureStructure;
    TextureUtility.RaycastResult _raycastResult = new TextureUtility.RaycastResult();
    private Vector3 placementOffset = new Vector3(0, 0.01f, 0);
    List<ARRaycastHit> aRRaycastHits = new List<ARRaycastHit>();
    private CommandBuffer commandBuffer;
    int textureSize = 512;
    //bool _applicationPause = false;

    float timer;
    float timer_step = 0.1f;

    Texture cameraTex {
        get {

            //If AR Foundation is available
            if (_arSession.enabled && arBackgroundRenderer != null)
            {
                return arBackgroundRenderer;
            }

            //If Webcam is available
            if (backCam != null)
                return backCam;

            return null;
        }
    }

    private IEnumerator Start()
    {
        _camera = Camera.main;
        Init();

        if ((ARSession.state == ARSessionState.None) ||
            (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported)
        {
            // Start some fallback experience for unsupported devices
            _arSession.enabled = false;
        }
        else
        {
            // Start the AR session
            _arSession.enabled = true;
        }
    }

    private void Init() {
        Debug.Log("Init");

        texturePreivew.SetUp();
        //AccessToFrontCamera();

        TextureUtility = new TextureUtility();

        PrepareTexture();

        shotBtn.onClick.AddListener(TakeAPhoto);

        scalePreview.texture = modelTexRenderer;

        meshIndicator.SetUp(_camera, GetRaycastResult);

        commandBuffer = new CommandBuffer();
        commandBuffer.name = "AR Camera Background Blit Pass";
    }

    //Fallback function, if ar foundation is not support
    private void AccessToFrontCamera() {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No Camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        //
        if (backCam == null)
        {
            Debug.Log("No back Camera found");

            return;
        }

        backCam.Play();
    }

    private void UpdateCameraTex() {
        if (arBackgroundRenderer != null && _arCameraBG.material != null && commandBuffer != null) {
            //var commandBuffer = new CommandBuffer();
            //commandBuffer.name = "AR Camera Background Blit Pass";
            commandBuffer.Clear();

            var texture = !_arCameraBG.material.HasProperty("_MainTex") ? null : _arCameraBG.material.GetTexture("_MainTex");
            Graphics.SetRenderTarget(arBackgroundRenderer.colorBuffer, arBackgroundRenderer.depthBuffer);
            commandBuffer.ClearRenderTarget(true, false, Color.clear);
            commandBuffer.Blit(texture, BuiltinRenderTextureType.CurrentActive, _arCameraBG.material);
            Graphics.ExecuteCommandBuffer(commandBuffer);
        }
    }

    private void PrepareTexture() {
        modelTexRenderer = TextureUtility.GetRenderTexture(textureSize);
        imageProcessRenderer = TextureUtility.GetRenderTexture((int) (textureSize * 0.5f));
        arBackgroundRenderer = TextureUtility.GetRenderTexture(Screen.width, Screen.height, 24);

        linePreview.texture = texturePreivew.edgeLineTex;
    }

    private void Update()
    {
        if (cameraTex == null) return;

        UpdateCameraTex();

        //PlaceObjectOnARPlane(new Vector2(0.5f, 0.5f), _objMappingVisualQueue.transform);
        //Resize, and rotate to right direction
        //-backCam.videoRotationAngle
        _textureStructure = GrabTextureRadius(Screen.width, Screen.height);
        meshIndicator.DisplayOnScreenPos(_textureStructure, _CropSize);

        TextureUtility.RotateAndScaleImage(cameraTex, modelTexRenderer, rotateMat, _textureStructure, 0);
        TextureUtility.RotateAndScaleImage(cameraTex, imageProcessRenderer, rotateMat, _textureStructure, 0);

        StartCoroutine(texturePreivew.ExecEdgeProcessing(imageProcessRenderer));

        if (timer > Time.time) return;

        PreviewEdgeMesh();

        timer = timer_step + Time.time;
    }

    private TextureUtility.TextureStructure GrabTextureRadius(int width, int height) {
        return TextureUtility.GrabTextureRadius(width, height, _CropSize);
    }

    private void OnMeshDone(TextureMeshManager.MeshLocData meshResult) {
        if (!meshResult.isValid) return;

        MeshIndicator.IndictatorData indictatorData = meshIndicator.GetRelativePosRot(meshResult.screenPoint);

        float sizeMagnitue = (_camera.transform.position - meshResult.meshObject.transform.position).magnitude * _SizeStrength;
        meshResult.meshObject.transform.localScale = new Vector3(sizeMagnitue, sizeMagnitue, sizeMagnitue);

        meshResult.meshObject.SetPosRotation(indictatorData.position, indictatorData.rotation);
    }

    private TextureUtility.RaycastResult GetRaycastResult(Vector2 screenPos)
    {
        var screenCenter = _camera.ViewportToScreenPoint(screenPos);
        bool hasHitSomething = _arRaycastManager.Raycast(screenCenter, aRRaycastHits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        _raycastResult.hasHit = hasHitSomething;

        if (hasHitSomething) {
            _raycastResult.hitPoint = aRRaycastHits[0].pose.position + placementOffset;
            _raycastResult.hitRotation = aRRaycastHits[0].pose.rotation;
        }

        return _raycastResult;
    }

    private async void PreviewEdgeMesh()
    {
        texturePreivew.ProcessCSTextureColor();

        OnMeshDone(await texturePreivew.CaptureEdgeBorderMesh(imageProcessRenderer.width, meshBorder, _textureStructure));
    }

    private async void TakeAPhoto() {
        MeshObject meshObject = meshObjManager.CreateMeshObj(meshBorder.transform.position, meshBorder.transform.rotation, true);

        OnMeshDone(await texturePreivew.CaptureContourMesh(modelTexRenderer, meshObject, _textureStructure));
    }

}
