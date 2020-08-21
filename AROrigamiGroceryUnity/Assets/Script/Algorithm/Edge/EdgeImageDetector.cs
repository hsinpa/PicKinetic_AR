using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeImageDetector : MonoBehaviour
{

    [SerializeField]
    private Material EdgeMaterial;


    [SerializeField]
    private Texture2D inputImage;

    // Start is called before the first frame update
    void Start()
    {
        BlueImage(inputImage);    
    }

    void BlueImage(Texture2D input) {
        Texture aTexture;
        RenderTexture rTex;


    }



}
