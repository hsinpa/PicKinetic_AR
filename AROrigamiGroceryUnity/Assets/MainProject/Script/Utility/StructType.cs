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
    public struct MeshSaveData {
        public string mainTexPath;
        public string processTexPath;
        public string id;
        public string name;
    }

    [System.Serializable]
    public struct MeshSaveType {
        public MeshSaveData meshSaveData;

        public Texture2D mainTex;
        public Texture2D processedTex;
    }
}
