using AROrigami;
using Hsinpa.Study;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilities;

public class OffCameraPreview : MonoBehaviour
{
    public RawImage preview;
    public Material rotateMat;

    [SerializeField, Range(0, 1)]
    private float _SizeStrength = 1;

    [SerializeField]
    private TextureMeshManager textureMeshPreview;

    [SerializeField]
    private MeshIndicator _meshIndicator;

    [SerializeField]
    private MeshObject p_meshObject;

    public Texture2D inputTex;
    private Texture2D cropTex;
    private Texture2D previewTex;

    private RenderTexture modelTexRenderer;
    private RenderTexture imageProcessRenderer;

    TextureUtility _textureUtility;
    TextureUtility.TextureStructure _textureStructure;
    TextureUtility.RaycastResult _raycastResult;
    private Camera _camera;

    private const float degreeToRadian = Mathf.PI / 180;

    int textureSize = 512;

    float timer;
    float timer_step = 0.05f;

    private void Start()
    {
        _camera = Camera.main;
        _raycastResult = new TextureUtility.RaycastResult();
        _textureUtility = new TextureUtility();
        _textureStructure = GrabTextureRadius();
        _meshIndicator.SetUp(_camera, GetRaycastResult);
        PrepareTexture();
        preview.texture = imageProcessRenderer;

        textureMeshPreview.OnMeshCalculationDone += OnMeshDone;

        //var scaleTex = RotateAndScaleImage(inputTex, GrabTextureRadius(), 0);
        //preview.texture = scaleTex;

        //textureMeshPreview.CaptureContourMesh(scaleTex, p_meshObject);
        //_ = UtilityMethod.DoDelayWork(1, Preview3DObject);
    }

    private void Update()
    {
        _meshIndicator.DisplayOnScreenPos(_textureStructure);
        TextureUtility.RotateAndScaleImage(inputTex, modelTexRenderer, rotateMat, _textureStructure, 0);
        TextureUtility.RotateAndScaleImage(inputTex, imageProcessRenderer, rotateMat, _textureStructure, 0);

        StartCoroutine(textureMeshPreview.ExecEdgeProcessing(imageProcessRenderer));

        if (timer > Time.time) return;

        textureMeshPreview.ProcessCSTextureColor();

        textureMeshPreview.CaptureEdgeBorderMesh(imageProcessRenderer.width, p_meshObject, _textureStructure);

        //textureMeshPreview.CaptureContourMesh(modelTexRenderer, p_meshObject);

        timer = timer_step + Time.time;
    }

    private void OnMeshDone(TextureMeshManager.MeshCalResult meshResult)
    {
        meshResult.screenPoint.Set(meshResult.screenPoint.x * Screen.width, meshResult.screenPoint.y * Screen.height);

        TextureUtility.RaycastResult raycast = GetRaycastResult(meshResult.screenPoint);

        if (!raycast.hasHit) return;

        meshResult.meshObject.transform.position = raycast.hitPoint;

        float sizeMagnitue = (_camera.transform.position - meshResult.meshObject.transform.position).magnitude * _SizeStrength;
        meshResult.meshObject.transform.localScale = new Vector3(sizeMagnitue, sizeMagnitue, sizeMagnitue);

        var cameraForward = _camera.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x, 0 , cameraForward.z);

        meshResult.meshObject.transform.rotation = Quaternion.LookRotation(cameraBearing);
    }

    private TextureUtility.RaycastResult GetRaycastResult(Vector2 screenPos) {
        Ray ray = _camera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        _raycastResult.hasHit = Physics.Raycast(ray, out hit, 100.0f);
        _raycastResult.hitPoint = hit.point + new Vector3(0, 0.02f, 0);

        return _raycastResult;
    }

    private void Preview3DObject() {
        textureMeshPreview.ProcessCSTextureColor();

        textureMeshPreview.CaptureContourMesh(modelTexRenderer, p_meshObject, _textureStructure);
    }

    private void PrepareTexture()
    {
        modelTexRenderer = TextureUtility.GetRenderTexture(textureSize);
        imageProcessRenderer = TextureUtility.GetRenderTexture((int)(textureSize * 0.5f));

        textureMeshPreview.UpdateScreenInfo((int) ((Screen.width / 2f) - (textureSize / 2f) ),
                                            (int)((Screen.height / 2f) - (textureSize / 2f)));
    }

    private TextureUtility.TextureStructure GrabTextureRadius()
    {
        var texInfo = _textureUtility.GrabTextureRadius(inputTex.width, inputTex.height, 0.6f);

        //Debug.Log("inputTex.width " + inputTex.width +", inputTex.height " + inputTex.height);
        //Debug.Log("texInfo " + texInfo.width + ", texInfo " + texInfo.height);

        return texInfo;
    }
}