﻿using PicKinetic;
using Hsinpa.Study;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Utilities;

public class OffCameraPreview : MonoBehaviour
{

    public RawImage rawPreview;
    public RawImage edgePreview;

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
        rawPreview.texture = imageProcessRenderer;

        captureBtn.onClick.AddListener(() => { TakeAPhoto(); });
    }

    private void Update()
    {
        _meshIndicator.DisplayOnScreenPos(_textureStructure, _CropSize);
        TextureUtility.RotateAndScaleImage(inputTex, modelTexRenderer, rotateMat, _textureStructure, Rotation);
        TextureUtility.RotateAndScaleImage(inputTex, imageProcessRenderer, rotateMat, _textureStructure, Rotation);

        StartCoroutine(textureMeshPreview.ExecEdgeProcessing(imageProcessRenderer));

        if (timer > Time.time) return;

        PreviewEdgeMesh();

        timer = timer_step + Time.time;
    }

    private void OnMeshDone(TextureMeshManager.MeshLocData meshResult)
    {
        if (!meshResult.isValid) return;

        MeshIndicator.IndictatorData indictatorData = _meshIndicator.GetRelativePosRot(meshResult.screenPoint);

        float sizeMagnitue = (_camera.transform.position - meshResult.meshObject.transform.position).magnitude * _SizeStrength;
        meshResult.meshObject.transform.localScale = new Vector3(sizeMagnitue, sizeMagnitue, sizeMagnitue);

        meshResult.meshObject.SetPosRotation(indictatorData.position, indictatorData.rotation);
    }

    private TextureUtility.RaycastResult GetRaycastResult(Vector2 screenPos) {
        screenPos.Set(screenPos.x * Screen.width, screenPos.y * Screen.height);
        Ray ray = _camera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        _raycastResult.hasHit = Physics.Raycast(ray, out hit, 100.0f, ParameterFlag.ColliderLayer.FloorLayer);
        _raycastResult.hitPoint = hit.point + new Vector3(0, 0.01f, 0);

        return _raycastResult;
    }

    private async void PreviewEdgeMesh() {
        textureMeshPreview.ProcessCSTextureColor();

        OnMeshDone(await textureMeshPreview.CaptureEdgeBorderMesh(imageProcessRenderer.width, p_meshOutline, _textureStructure));
    }

    private void PrepareTexture()
    {
        modelTexRenderer = TextureUtility.GetRenderTexture(textureSize);
        imageProcessRenderer = TextureUtility.GetRenderTexture((int)(textureSize * 0.5f));

        edgePreview.texture = textureMeshPreview.edgeLineTex;
    }

    private async void TakeAPhoto()
    {
        MeshObject meshObject = meshObjManager.CreateMeshObj(p_meshOutline.transform.position, p_meshOutline.transform.rotation, true);

        OnMeshDone(await textureMeshPreview.CaptureContourMesh(modelTexRenderer, meshObject, _textureStructure));
    }

    private TextureUtility.TextureStructure GrabTextureRadius()
    {
        var texInfo = _textureUtility.GrabTextureRadius(inputTex.width, inputTex.height, _CropSize);

        //Debug.Log("inputTex.width " + inputTex.width +", inputTex.height " + inputTex.height);
        //Debug.Log("texInfo " + texInfo.width + ", texInfo " + texInfo.height);

        return texInfo;
    }
}