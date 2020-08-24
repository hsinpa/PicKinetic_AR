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
    private Texture2D previewTex;

    private Vector3 backgroundScale = new Vector3();

    TextureUtility TextureUtility;

    private void Start()
    {
        defaultBackground = background.texture;
        TextureUtility = new TextureUtility();

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

        GrabTextureRadius();
    }

    private void GrabTextureRadius() {

        var texInfo = TextureUtility.GrabTextureRadius(backCam.width, backCam.height, 0.6f);

        if (previewTex == null)
            previewTex = new Texture2D(texInfo.width, texInfo.height);

        previewTex.SetPixels(backCam.GetPixels(texInfo.x, texInfo.y, texInfo.width, texInfo.height));
        previewTex.Apply();

        preview.texture = previewTex;
    }


}
