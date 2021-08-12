using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utilities;

namespace PicKinetic.Model {
    public class TextureModel
    {
        private SRPTextures srpTextureRoot;
        private string saveTexDirPath;
        private string tempTexDirPath; // Only after confirm, switch to saveTex; Save rom memory

        public enum Operation {TempFolder, PermanentFolder };

 #region Public API
        public TextureModel(SRPTextures srpTextureRoot) {
            this.srpTextureRoot = srpTextureRoot;
            saveTexDirPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.DiskFolder);
            tempTexDirPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.TempFolder);
        }
            
        public void ReadSaveTextureFromDisk() { 
        
        }

        /// <summary>
        /// Save textures to temporary folder, which will be destroy during app life end
        /// </summary>
        /// <param name="mainTex"></param>
        /// <param name="edgeProcessTex"></param>
        /// <returns></returns>
        public StructType.MeshSaveData SaveTempMesh(Texture2D mainTex, Texture2D edgeProcessTex) {
            var meshData = CreateMeshData();

            SaveTextureToDisk(meshData.mainTexPath, mainTex);
            SaveTextureToDisk(meshData.processTexPath, edgeProcessTex);

            return meshData;
        }

        public void ExecFileOperation(StructType.MeshSaveData meshSaveData, Operation operation) {

            string srcMainTexPath = GetFullPath(meshSaveData.mainTexPath, (operation == Operation.TempFolder) ? false : true );
            string targetMainTexPath = GetFullPath(meshSaveData.mainTexPath, (operation == Operation.TempFolder) ? true : false);

            string srcProcessTexPath = GetFullPath(meshSaveData.processTexPath, (operation == Operation.TempFolder) ? false : true);
            string targetProcessTexPath = GetFullPath(meshSaveData.processTexPath, (operation == Operation.TempFolder) ? true : false);

            MoveFile(srcMainTexPath, targetMainTexPath);
            MoveFile(srcProcessTexPath, targetProcessTexPath);
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

        private bool MoveFile(string targetFullPath, string landingFullPath) {

            if (File.Exists(targetFullPath) && !File.Exists(landingFullPath)) {
                File.Move(targetFullPath, landingFullPath);
            }

            return false;
        }

        private void SaveTextureToDisk(string filename, Texture2D texture)
        {
            string folderPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.TempFolder);
            string fullPath = Path.Combine(Application.persistentDataPath, ParameterFlag.SaveSystem.TempFolder, filename);

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

        private string GetFullPath(string filename, bool isTempFolder) {
            return Path.Combine((isTempFolder) ? tempTexDirPath : saveTexDirPath, filename);
        }
        #endregion
    }
}