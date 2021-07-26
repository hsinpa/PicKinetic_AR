using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PicKinetic.Model {
    public class TextureModel
    {

        public void SaveTextureToDisk(string relativePath, Texture2D texture) {

            string rootPath = Application.persistentDataPath;
            string fullPath = Path.Combine(rootPath, relativePath);

            System.IO.File.WriteAllBytes(fullPath, texture.EncodeToJPG());
        }


    }
}