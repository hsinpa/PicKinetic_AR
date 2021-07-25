using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PicKinetic
{
    public class MeshObjectManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject meshPrefab;

        public MeshObject CreateMeshObj(Vector3 p_position, Quaternion p_quaternion, bool p_movable) {
            GameObject createObj = Instantiate(meshPrefab, p_position, p_quaternion, transform);
            MeshObject meshObj = createObj.GetComponent<MeshObject>();


            meshObj.copyUVTexture = p_movable;

            return meshObj;
        }

    }
}