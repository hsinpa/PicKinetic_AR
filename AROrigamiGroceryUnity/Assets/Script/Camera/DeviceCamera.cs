using AROrigami;
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
    MeshObject meshObject;

    [SerializeField]
    private Button shotBtn;

    private Camera _camera;
    TextureUtility TextureUtility;
    TextureUtility.TextureStructure _textureStructure;
    List<ARRaycastHit> aRRaycastHits = new List<ARRaycastHit>();

    int textureSize = 512;


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
        AccessToFrontCamera();

        TextureUtility = new TextureUtility();

        PrepareTexture();

        shotBtn.onClick.AddListener(TakeAPhoto);

        scalePreview.texture = modelTexRenderer;

        texturePreivew.OnEdgeTexUpdate += OnEdgeImageUpdate;
        texturePreivew.OnMeshCalculationDone += OnMeshDone;
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
        if (arBackgroundRenderer != null && _arCameraBG.material != null) {

            var commandBuffer = new CommandBuffer();
            commandBuffer.name = "AR Camera Background Blit Pass";
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

        Debug.Log("backCam.width "+ backCam.width + ", backCam.height " + backCam.height);
        arBackgroundRenderer = TextureUtility.GetRenderTexture(Screen.width, Screen.height, 24);

        texturePreivew.UpdateScreenInfo((int)((Screen.width / 2f) - (textureSize / 2f)),
                                    (int)((Screen.height / 2f) - (textureSize / 2f)));
    }

    private void Update()
    {
        UpdateCameraTex();

        if (cameraTex == null) return;

        //Resize, and rotate to right direction
        //-backCam.videoRotationAngle
        _textureStructure = GrabTextureRadius();
        TextureUtility.RotateAndScaleImage(cameraTex, modelTexRenderer, rotateMat, _textureStructure, 0);
        TextureUtility.RotateAndScaleImage(cameraTex, imageProcessRenderer, rotateMat, _textureStructure, 0);

        StartCoroutine(texturePreivew.ExecEdgeProcessing(imageProcessRenderer));

        if (timer > Time.time) return;

        texturePreivew.ProcessCSTextureColor();

        texturePreivew.CaptureEdgeBorderMesh(imageProcessRenderer.width, meshBorder, _textureStructure);

        timer = timer_step + Time.time;
    }

    private TextureUtility.TextureStructure GrabTextureRadius() {
        return TextureUtility.GrabTextureRadius(Screen.width, Screen.height, 1f);
    }

    private void OnEdgeImageUpdate(Texture2D tex)
    {
        linePreview.texture = tex;
    }

    private void OnMeshDone(TextureMeshManager.MeshCalResult meshResult) {

        var screenCenter = _camera.ViewportToScreenPoint(meshResult.screenPoint);
        bool hasHitSomething = _arRaycastManager.Raycast(screenCenter, aRRaycastHits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hasHitSomething)
        {
            //Debug.Log(string.Format("You selected the {0}, Position {1}", hit.transform.name, hit.point)); // ensure you picked right object

            meshResult.meshObject.transform.position = aRRaycastHits[0].pose.position + new Vector3(0, 0.01f, 0);

            float sizeMagnitue = (_camera.transform.position - meshResult.meshObject.transform.position).magnitude * _SizeStrength;
            meshResult.meshObject.transform.localScale = new Vector3(sizeMagnitue, sizeMagnitue, sizeMagnitue);

            var cameraForward = _camera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            meshResult.meshObject.transform.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void TakeAPhoto() {
        if (meshObject.meshRenderer.enabled) {
            meshObject.meshRenderer.enabled = false;
            return;
        }

        if (scalePreview != null && scalePreview.texture != null) {
            meshObject.meshRenderer.enabled = true;

            texturePreivew.CaptureContourMesh(modelTexRenderer, meshObject, _textureStructure);
        }
    }

    private void OnDestroy()
    {

        texturePreivew.OnEdgeTexUpdate -= OnEdgeImageUpdate;
        texturePreivew.OnMeshCalculationDone -= OnMeshDone;
    }
}
