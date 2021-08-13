using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PicKinetic.Model
{
    public class InGameModelManager : IModelManager
    {
        [SerializeField]
        private SRPTextures SRTextureRoot;

        public override void SetUp() { 
            models = new List<object> {
                new PhotoAlbumModel(SRTextureRoot)
            };
        }

        private void OnApplicationQuit()
        {
            PhotoAlbumModel t = GetModel<PhotoAlbumModel>();
            t.Dispose();
        }
    }
}