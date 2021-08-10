using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utilities;

namespace PicKinetic.Model {
    public class TextureModel
    {
 
        public void ReadSaveTextureFromDisk() { 
        
        }

        public StructType.MeshSaveData SaveMesh(StructType.GrabTextures grabTexType) {
            var meshData = CreateMeshData();

            SaveTextureToDisk(meshData.mainTexPath, grabTexType.mainTex);
            SaveTextureToDisk(meshData.processTexPath, grabTexType.processedTex);

            return meshData;
        }

        private StructType.MeshSaveData CreateMeshData() {

            string uniqueID = UtilityMethod.GenerateUniqueRandomString(4);
            string mainTexPath = "mainTex-" + uniqueID+".jpg";
            string processTexPath = "processTex-" + uniqueID+".jpg";

            return new StructType.MeshSaveData()
            {
                mainTexPath = mainTexPath,
                processTexPath = processTexPath,
                id = uniqueID,
                name = "Pure art" //Editable
            };
        }

        private void SaveTextureToDisk(string filename, Texture2D texture) {

            string folderPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.DiskFolder);
            string fullPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.DiskFolder, filename);

            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }

            Debug.Log(fullPath);

            System.IO.File.WriteAllBytes(fullPath, texture.EncodeToJPG());
        }


    }
}