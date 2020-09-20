using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRaycaster : MonoBehaviour
{

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private GameObject SpawnObjectPrefab;
   
    // Update is called once per frame
    void Update()
    {
        CheckScreenRaycast();
    }

    private void CheckScreenRaycast() {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Debug.Log(string.Format( "You selected the {0}, Position {1}", hit.transform.name, hit.point)); // ensure you picked right object

                GameObject.Instantiate(SpawnObjectPrefab, hit.point + Vector3.up, SpawnObjectPrefab.transform.rotation, transform);
            }
        }
    }
}
