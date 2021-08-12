using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PicKinetic.Model {
    [CreateAssetMenu(fileName = "SRPOperation", menuName = "SRPOperation/TextureDirectory[SRP]", order = 1)]
    public class SRPTextures : ScriptableObject
    {
        [SerializeField]
        private List<StructType.MeshSaveData> _TextureData = new List<StructType.MeshSaveData>();
        public List<StructType.MeshSaveData> TextureData => this._TextureData;

        public StructType.MeshSaveData GetDataByID(string id) {
            return this._TextureData.Find(x => x.id == id);
        }
    }
}
