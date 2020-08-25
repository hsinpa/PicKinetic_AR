using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture defaultBackground;

    public RawImage background;
    public AspectRatioFitter fit;
    public RawImage preview;
    public Material rotateMat;

    private Texture2D cropTex;
    private Texture2D previewTex;
    private RenderTexture previewRenderer;

    private Vector3 backgroundScale = new Vector3();

    TextureUtility TextureUtility;

    private const float degreeToRadian = Mathf.PI / 180;
    private Rect rectReadPicture;
    int textureSize = 256;

    [SerializeField, Range(0, 3.14f)]
    private float cameraTexRot;

    private void Start()
    {
        defaultBackground = background.texture;
        TextureUtility = new TextureUtility();
        PrepareTexture();

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0) {
            Debug.Log("No Camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++) {
            if (!devices[i].isFrontFacing) {
                backCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        //
        if (backCam == null) {
            Debug.Log("No back Camera found");

            return;
        }

        backCam.Play();

        background.gameObject.SetActive(true);
        background.texture = backCam;

        camAvailable = true;
    }

    private void PrepareTexture() {
        previewRenderer = TextureUtility.GetRenderTexture(textureSize);
        previewTex = new Texture2D(textureSize, textureSize);
        rectReadPicture = new Rect(0, 0, textureSize, textureSize);

    }

    private void Update()
    {
        if (!camAvailable) return;

        float ratio = (float)backCam.width / (float)backCam.height;

        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;

        backgroundScale.Set(1f, scaleY, 1f);
        background.rectTransform.localScale = backgroundScale;

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        preview.texture = RotateAndScaleImage( GrabTextureRadius(), -backCam.videoRotationAngle);
    }

    private Texture2D GrabTextureRadius() {

        var texInfo = TextureUtility.GrabTextureRadius(backCam.width, backCam.height, 0.6f);

        if (cropTex == null)
            cropTex = new Texture2D(texInfo.width, texInfo.height);

        cropTex.SetPixels(backCam.GetPixels(texInfo.x, texInfo.y, texInfo.width, texInfo.height));
        cropTex.Apply();

        return cropTex;
    }

    private Texture2D RotateAndScaleImage(Texture p_texture, int degree) {
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
