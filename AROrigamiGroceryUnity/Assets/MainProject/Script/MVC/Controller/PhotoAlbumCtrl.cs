using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PicKinetic.Model;


namespace PicKinetic.Controller
{
    public class PhotoAlbumCtrl : ObserverPattern.Observer
    {

        PhotoAlbumModel textureModel;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {

                case EventFlag.Event.GameStart:
                    textureModel = PicKineticAR.Instance.ModelManager.GetModel<PhotoAlbumModel>();
                    break;

                case EventFlag.Event.OnPhotoAlbumOpen:
                    OnPhotoAlbumOpen();
                    break;

            }
        }


        #region UI Event
        private void OnPhotoAlbumOpen() {
            //textureModel.SRPTextureRoot.TextureData
        }

        #endregion


    }
}