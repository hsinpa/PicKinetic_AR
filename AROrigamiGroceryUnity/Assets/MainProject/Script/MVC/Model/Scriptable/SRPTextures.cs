using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PicKinetic.Model {
    [CreateAssetMenu(fileName = "SRPOperation", menuName = "SRPOperation/TextureDirectory[SRP]", order = 1)]
    public class SRPTextures : ScriptableObject
    {
        [SerializeField]
        private List<StructType.MeshJsonData> _TextureData = new List<StructType.MeshJsonData>();
        public List<StructType.MeshJsonData> TextureData => this._TextureData;

        public void Insert(StructType.MeshJsonData meshJsonData)
        {
            _TextureData.Add(meshJsonData);
        }

        public void Remove(string id) {
            int index = this._TextureData.FindIndex(x => x.id == id);

            if (index >= 0)
                _TextureData.RemoveAt(index);
        }

        public StructType.MeshJsonData GetDataByID(string id) {
            return this._TextureData.Find(x => x.id == id);
        }

        public bool IsDataExist(string id) {
            return this._TextureData.FindIndex(x => x.id == id) >= 0;
        }
    }
}
