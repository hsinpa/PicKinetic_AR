using AROrigami;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OffCameraPreview : MonoBehaviour
{
    public RawImage preview;
    public Material rotateMat;

    [SerializeField]
    private TextureMeshManager textureMeshPreview;

    [SerializeField]
    private MeshObject p_meshObject;

    public Texture2D inputTex;
    private Texture2D cropTex;
    private Texture2D previewTex;

    private RenderTexture modelTexRenderer;
    private RenderTexture imageProcessRenderer;

    TextureUtility TextureUtility;

    private const float degreeToRadian = Mathf.PI / 180;

    int textureSize = 512;

    float timer;
    float timer_step = 0.05f;

    private void Start()
    {
        TextureUtility = new TextureUtility();
        PrepareTexture();
        preview.texture = imageProcessRenderer;

        //var scaleTex = RotateAndScaleImage(inputTex, GrabTextureRadius(), 0);
        //preview.texture = scaleTex;

        //textureMeshPreview.CaptureContourMesh(scaleTex, p_meshObject);
    }

    private void Update()
    {
        RotateAndScaleImage(inputTex, modelTexRenderer, GrabTextureRadius(), 0);
        RotateAndScaleImage(inputTex, imageProcessRenderer, GrabTextureRadius(), 0);
        StartCoroutine(textureMeshPreview.ExecEdgeProcessing(imageProcessRenderer));

        if (timer > Time.time) return;

        textureMeshPreview.ProcessCSTextureColor();

        textureMeshPreview.CaptureEdgeBorderMesh(imageProcessRenderer.width, p_meshObject);

        timer = timer_step + Time.time;
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
        var texInfo = TextureUtility.GrabTextureRadius(inputTex.width, inputTex.height, 0.6f);

        //Debug.Log("inputTex.width " + inputTex.width +", inputTex.height " + inputTex.height);
        //Debug.Log("texInfo " + texInfo.width + ", texInfo " + texInfo.height);

        return texInfo;
    }

    private RenderTexture RotateAndScaleImage(Texture texture, RenderTexture renderer, TextureUtility.TextureStructure textureSetting, int degree)
    {
        float radian = degreeToRadian * degree;

        //rotateMat.SetFloat("_Rotation", radian);
        rotateMat.SetFloat("_EnlargeX", textureSetting.xRatio);
        rotateMat.SetFloat("_EnlargeY", textureSetting.yRatio);

        Graphics.Blit(texture, renderer, rotateMat, 0);

        //RenderTexture.active = previewRenderer;
        //// Read pixels
        //previewTex.ReadPixels(rectReadPicture, 0, 0);
        //previewTex.Apply();
        //RenderTexture.active = null;

        return renderer;
    }
}