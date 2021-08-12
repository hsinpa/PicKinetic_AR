using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class StructType
{
    public struct GrabTextures
    {
        public PicKinetic.MeshObject meshObject;
        public Texture2D mainTex;
        public Texture2D processedTex;
    }

    [System.Serializable]
    public struct MeshJsonData {
        public string mainTexPath;
        public string processTexPath;
        public string id;
        public string name;

        public bool isValid => !string.IsNullOrEmpty(mainTexPath);
    }

    [System.Serializable]
    public struct MeshSaveType {
        public MeshJsonData meshSaveData;

        public Texture2D mainTex;
        public Texture2D processedTex;
    }


}
