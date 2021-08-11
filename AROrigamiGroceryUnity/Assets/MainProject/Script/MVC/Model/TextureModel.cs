using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utilities;

namespace PicKinetic.Model {
    public class TextureModel
    {
        private string saveTexDirPath;
        private string tempTexDirPath; // Only after confirm, switch to saveTex; Save rom memory

 #region Public API
        public TextureModel() {
            saveTexDirPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.DiskFolder);
            tempTexDirPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.TempFolder);
        }
            
        public void ReadSaveTextureFromDisk() { 
        
        }

        public StructType.MeshSaveData SaveMesh(Texture2D mainTex, Texture2D edgeProcessTex) {
            var meshData = CreateMeshData();

            SaveTextureToDisk(meshData.mainTexPath, mainTex);
            SaveTextureToDisk(meshData.processTexPath, edgeProcessTex);

            return meshData;
        }

        //Remove temporary memory 
        public void Dispose() {
            DeleteDirectory(tempTexDirPath);
        }

        #endregion

        #region Private 
        private StructType.MeshSaveData CreateMeshData()
        {
            string uniqueID = UtilityMethod.GenerateUniqueRandomString(4);
            string mainTexPath = "mainTex-" + uniqueID + ".jpg";
            string processTexPath = "processTex-" + uniqueID + ".jpg";

            return new StructType.MeshSaveData()
            {
                mainTexPath = mainTexPath,
                processTexPath = processTexPath,
                id = uniqueID,
                name = "Pure art" //Editable
            };
        }

        private void SaveTextureToDisk(string filename, Texture2D texture)
        {

            string folderPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.DiskFolder);
            string fullPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.DiskFolder, filename);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            Debug.Log(fullPath);

            System.IO.File.WriteAllBytes(fullPath, texture.EncodeToJPG());
        }

        private bool DeleteDirectory(string absDirectoryPath) {

            if (!Directory.Exists(absDirectoryPath)) return false;

            string[] files = Directory.GetFiles(absDirectoryPath);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            Directory.Delete(absDirectoryPath, false);

            return true;
        }
    #endregion
    }
}