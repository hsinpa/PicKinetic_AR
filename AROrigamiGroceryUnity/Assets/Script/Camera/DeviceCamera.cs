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

    [SerializeField]
    private ARCameraManager _arCameraManager;

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

    TextureUtility TextureUtility;

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
        TextureUtility.RotateAndScaleImage(cameraTex, modelTexRenderer, rotateMat, GrabTextureRadius(), 0);
        TextureUtility.RotateAndScaleImage(cameraTex, imageProcessRenderer, rotateMat, GrabTextureRadius(), 0);

        StartCoroutine(texturePreivew.ExecEdgeProcessing(imageProcessRenderer));

        if (timer > Time.time) return;

        texturePreivew.ProcessCSTextureColor();

        texturePreivew.CaptureEdgeBorderMesh(imageProcessRenderer.width, meshBorder);

        timer = timer_step + Time.time;
    }

    private TextureUtility.TextureStructure GrabTextureRadius() {
        return TextureUtility.GrabTextureRadius(Screen.width, Screen.height, 1f);
    }

    private void OnEdgeImageUpdate(Texture2D tex)
    {
        linePreview.texture = tex;
    }

    private void TakeAPhoto() {
        if (meshObject.meshRenderer.enabled) {
            meshObject.meshRenderer.enabled = false;
            return;
        }

        if (scalePreview != null && scalePreview.texture != null) {
            meshObject.meshRenderer.enabled = true;

            texturePreivew.CaptureContourMesh(modelTexRenderer, meshObject);
        }
    }

    private void OnDestroy()
    {
        texturePreivew.OnEdgeTexUpdate -= OnEdgeImageUpdate;
    }
}
