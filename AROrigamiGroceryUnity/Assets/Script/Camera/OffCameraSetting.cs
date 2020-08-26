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

    public Texture2D inputTex;
    private Texture2D cropTex;
    private Texture2D previewTex;
    private RenderTexture previewRenderer;

    TextureUtility TextureUtility;

    private const float degreeToRadian = Mathf.PI / 180;
    private Rect rectReadPicture;
    int textureSize = 256;

    private void Start()
    {
        TextureUtility = new TextureUtility();
        PrepareTexture();

        var scaleTex = RotateAndScaleImage(GrabTextureRadius(), 0);
        preview.texture = scaleTex;

        textureMeshPreview.CaptureEdgeBorderMesh(scaleTex);
        //textureMeshPreview.CaptureContourMesh(scaleTex);
    }

    private void PrepareTexture()
    {
        previewRenderer = TextureUtility.GetRenderTexture(textureSize);
        previewTex = new Texture2D(textureSize, textureSize);
        rectReadPicture = new Rect(0, 0, textureSize, textureSize);

        textureMeshPreview.UpdateScreenInfo((int) ((Screen.width / 2f) - (textureSize / 2f) ),
                                            (int)((Screen.height / 2f) - (textureSize / 2f)));
    }

    private Texture2D GrabTextureRadius()
    {

        var texInfo = TextureUtility.GrabTextureRadius(inputTex.width, inputTex.height, 0.6f);

        if (cropTex == null)
            cropTex = new Texture2D(texInfo.width, texInfo.height);

        cropTex.SetPixels(inputTex.GetPixels(texInfo.x, texInfo.y, texInfo.width, texInfo.height));
        cropTex.Apply();

        return cropTex;
    }

    private Texture2D RotateAndScaleImage(Texture p_texture, int degree)
    {
        float radian = degreeToRadian * degree;

        rotateMat.SetFloat("_Rotation", radian);

        Graphics.Blit(p_texture, previewRenderer, rotateMat, 0);

        RenderTexture.active = previewRenderer;
        // Read pixels
        previewTex.ReadPixels(rectReadPicture, 0, 0);
        previewTex.Apply();
        RenderTexture.active = null;

        return previewTex;
    }
}