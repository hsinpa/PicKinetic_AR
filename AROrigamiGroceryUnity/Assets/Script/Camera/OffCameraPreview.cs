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

    [SerializeField, Range(0, 1)]
    private float _CropSize = 1;

    [SerializeField, Range(0, 360)]
    private int Rotation = 0;

    [SerializeField]
    private Button captureBtn;

    [SerializeField]
    private TextureMeshManager textureMeshPreview;

    [SerializeField]
    private MeshIndicator _meshIndicator;

    [SerializeField]
    private MeshObject p_meshOutline;

    [SerializeField]
    private MeshObjectManager meshObjManager;

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

        captureBtn.onClick.AddListener(() => { TakeAPhoto(); });

        //var scaleTex = RotateAndScaleImage(inputTex, GrabTextureRadius(), 0);
        //preview.texture = scaleTex;

        //textureMeshPreview.CaptureContourMesh(scaleTex, p_meshObject);
        //_ = UtilityMethod.DoDelayWork(1, Preview3DObject);
    }

    private void Update()
    {
        _meshIndicator.DisplayOnScreenPos(_textureStructure, _CropSize);
        TextureUtility.RotateAndScaleImage(inputTex, modelTexRenderer, rotateMat, _textureStructure, Rotation);
        TextureUtility.RotateAndScaleImage(inputTex, imageProcessRenderer, rotateMat, _textureStructure, Rotation);

        StartCoroutine(textureMeshPreview.ExecEdgeProcessing(imageProcessRenderer));

        if (timer > Time.time) return;

        textureMeshPreview.ProcessCSTextureColor();

        textureMeshPreview.CaptureEdgeBorderMesh(imageProcessRenderer.width, p_meshOutline, _textureStructure);

        //textureMeshPreview.CaptureContourMesh(modelTexRenderer, p_meshObject);

        timer = timer_step + Time.time;
    }

    private void OnMeshDone(TextureMeshManager.MeshCalResult meshResult)
    {
        MeshIndicator.IndictatorData indictatorData = _meshIndicator.GetRelativePosRot(meshResult.screenPoint);

        float sizeMagnitue = (_camera.transform.position - meshResult.meshObject.transform.position).magnitude * _SizeStrength;
        meshResult.meshObject.transform.localScale = new Vector3(sizeMagnitue, sizeMagnitue, sizeMagnitue);

        meshResult.meshObject.SetPosRotation(indictatorData.position, indictatorData.rotation);
    }

    private TextureUtility.RaycastResult GetRaycastResult(Vector2 screenPos) {
        screenPos.Set(screenPos.x * Screen.width, screenPos.y * Screen.height);
        Ray ray = _camera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        _raycastResult.hasHit = Physics.Raycast(ray, out hit, 100.0f);
        _raycastResult.hitPoint = hit.point + new Vector3(0, 0.01f, 0);

        return _raycastResult;
    }

    private void Preview3DObject() {
        textureMeshPreview.ProcessCSTextureColor();

        textureMeshPreview.CaptureContourMesh(modelTexRenderer, p_meshOutline, _textureStructure);
    }

    private void PrepareTexture()
    {
        modelTexRenderer = TextureUtility.GetRenderTexture(textureSize);
        imageProcessRenderer = TextureUtility.GetRenderTexture((int)(textureSize * 0.5f));

        textureMeshPreview.UpdateScreenInfo((int) ((Screen.width / 2f) - (textureSize / 2f) ),
                                            (int)((Screen.height / 2f) - (textureSize / 2f)));
    }

    private void TakeAPhoto()
    {
        MeshObject meshObject = meshObjManager.CreateMeshObj(p_meshOutline.transform.position, p_meshOutline.transform.rotation, true);

        textureMeshPreview.CaptureContourMesh(modelTexRenderer, meshObject, _textureStructure);
    }

    private TextureUtility.TextureStructure GrabTextureRadius()
    {
        var texInfo = _textureUtility.GrabTextureRadius(inputTex.width, inputTex.height, _CropSize);

        //Debug.Log("inputTex.width " + inputTex.width +", inputTex.height " + inputTex.height);
        //Debug.Log("texInfo " + texInfo.width + ", texInfo " + texInfo.height);

        return texInfo;
    }
}