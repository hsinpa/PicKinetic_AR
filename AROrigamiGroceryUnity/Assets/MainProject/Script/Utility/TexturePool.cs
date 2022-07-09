using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Hsinpa {
    public class TexturePool
    {
        private Dictionary<string, Texture2D> cacheTexture = new Dictionary<string, Texture2D>();
        private MonoBehaviour _mono;

        public TexturePool(MonoBehaviour mono) {
            this._mono = mono;
        }

        public void LoadTexture(string texturePath, System.Action<Texture2D> callback) {
            _mono.StartCoroutine(ConroutineLoadTexture(texturePath, callback));
        }

        public void ReleaseTextures(int length) { 
        
        }

        private IEnumerator ConroutineLoadTexture(string texturePath, System.Action<Texture2D> callback)
        {
            if (!File.Exists(texturePath))
            {
                callback(null);
                yield break;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            texturePath = "file://" + texturePath;
#endif

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(texturePath))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    callback(texture);
                }
            }
        }

    }
}

