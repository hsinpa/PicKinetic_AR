using AROrigami;
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

    public RawImage scalePreview;
    public RawImage linePreview;

    public Material rotateMat;

    private Texture2D cropTex;
    private Texture2D previewTex;

    private RenderTexture previewRenderer;

    [SerializeField]
    TextureMeshPreview texturePreivew;

    [SerializeField]
    MeshObject meshBorder;

    [SerializeField]
    MeshObject meshObject;

    [SerializeField]
    private Button shotBtn;

    private Vector3 backgroundScale = new Vector3();

    TextureUtility TextureUtility;

    private const float degreeToRadian = Mathf.PI / 180;
    private Rect rectReadPicture;
    int textureSize = 512;

    private Camera _camera;

    float timer;
    float timer_step = 0.1f;

    private void Start()
    {
        _camera = Camera.main;
        defaultBackground = background.texture;
        TextureUtility = new TextureUtility();
        PrepareTexture();

        shotBtn.onClick.AddListener(TakeAPhoto);

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

        texturePreivew.OnEdgeTexUpdate += OnEdgeImageUpdate;
    }

    private void PrepareTexture() {
        previewRenderer = TextureUtility.GetRenderTexture(textureSize);

        previewTex = new Texture2D(textureSize, textureSize);
        rectReadPicture = new Rect(0, 0, textureSize, textureSize);

        texturePreivew.UpdateScreenInfo((int)((Screen.width / 2f) - (textureSize / 2f)),
                                    (int)((Screen.height / 2f) - (textureSize / 2f)));
    }

    private void Update()
    {
        if (!camAvailable) return;

        if (timer > Time.time) return;

        float ratio = (float)backCam.width / (float)backCam.height;

        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;

        backgroundScale.Set(1f, scaleY, 1f);
        background.rectTransform.localScale = backgroundScale;

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        var scaleTex = RotateAndScaleImage(backCam, GrabTextureRadius(), -backCam.videoRotationAngle);

        scalePreview.texture = scaleTex;

        texturePreivew.CaptureEdgeBorderMesh(scaleTex, meshBorder);

        timer = timer_step + Time.time;
    }

    private TextureUtility.TextureStructure GrabTextureRadius() {

        return TextureUtility.GrabTextureRadius(backCam.width, backCam.height, 0.6f);
    }

    private Texture2D RotateAndScaleImage(Texture p_texture, TextureUtility.TextureStructure textureSetting, int degree) {
        float radian = degreeToRadian * degree;

        rotateMat.SetFloat("_EnlargeX", textureSetting.xRatio);
        rotateMat.SetFloat("_EnlargeY", textureSetting.yRatio);
        rotateMat.SetFloat("_Rotation", radian);

        Graphics.Blit(p_texture, previewRenderer, rotateMat, 0);

        RenderTexture.active = previewRenderer;
        // Read pixels
        previewTex.ReadPixels(rectReadPicture, 0, 0);
        previewTex.Apply();
        RenderTexture.active = null;

        return previewTex;
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

            texturePreivew.CaptureContourMesh(scalePreview.texture as Texture2D, meshObject);
        }
    }

    private void OnDestroy()
    {
        texturePreivew.OnEdgeTexUpdate -= OnEdgeImageUpdate;
    }

}
