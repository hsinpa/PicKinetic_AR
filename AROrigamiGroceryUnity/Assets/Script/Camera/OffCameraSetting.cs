﻿using AROrigami;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffCameraSetting : MonoBehaviour
{
    public RawImage preview;
    public Material rotateMat;

    [SerializeField]
    private TextureMeshPreview textureMeshPreview;

    [SerializeField]
    private MeshObject p_meshObject;

    public Texture2D inputTex;
    private Texture2D cropTex;
    private Texture2D previewTex;
    private RenderTexture tempRenderer;

    private RenderTexture previewRenderer;

    TextureUtility TextureUtility;

    private const float degreeToRadian = Mathf.PI / 180;
    private Rect rectReadPicture;
    int textureSize = 512;

    float timer;
    float timer_step = 0.1f;

    private void Start()
    {
        TextureUtility = new TextureUtility();
        PrepareTexture();

    }

    private void Update()
    {
        if (timer > Time.time) return;

        var scaleTex = RotateAndScaleImage(inputTex, GrabTextureRadius(), 0);
        preview.texture = scaleTex;

        textureMeshPreview.CaptureEdgeBorderMesh(scaleTex, p_meshObject);

        timer = timer_step + Time.time;
    }

    private void PrepareTexture()
    {
        previewRenderer = TextureUtility.GetRenderTexture(textureSize);
        tempRenderer = TextureUtility.GetRenderTexture(textureSize);
        previewTex = new Texture2D(textureSize, textureSize);
        rectReadPicture = new Rect(0, 0, textureSize, textureSize);

        textureMeshPreview.UpdateScreenInfo((int) ((Screen.width / 2f) - (textureSize / 2f) ),
                                            (int)((Screen.height / 2f) - (textureSize / 2f)));
    }

    private TextureUtility.TextureStructure GrabTextureRadius()
    {
        var texInfo = TextureUtility.GrabTextureRadius(inputTex.width, inputTex.height, 0.6f);

        //Debug.Log("inputTex.width " + inputTex.width +", inputTex.height " + inputTex.height);
        //Debug.Log("texInfo " + texInfo.width + ", texInfo " + texInfo.height);

        return texInfo;
    }

    private Texture2D RotateAndScaleImage(Texture texture, TextureUtility.TextureStructure textureSetting, int degree)
    {
        float radian = degreeToRadian * degree;

        //rotateMat.SetFloat("_Rotation", radian);
        rotateMat.SetFloat("_EnlargeX", textureSetting.xRatio);
        rotateMat.SetFloat("_EnlargeY", textureSetting.yRatio);

        Graphics.Blit(texture, tempRenderer, rotateMat, 0);
        Graphics.Blit(tempRenderer, previewRenderer, rotateMat, 1);

        RenderTexture.active = previewRenderer;
        // Read pixels
        previewTex.ReadPixels(rectReadPicture, 0, 0);
        previewTex.Apply();
        RenderTexture.active = null;

        return previewTex;
    }
}