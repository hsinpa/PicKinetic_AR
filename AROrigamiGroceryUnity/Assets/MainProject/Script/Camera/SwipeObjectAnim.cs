using PicKinetic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeObjectAnim : MonoBehaviour
{
    [SerializeField]
    private MeshObject targetObject;

    private void Update()
    {
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && targetObject != null)
        {
            // Get movement of the finger since last frame
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            targetObject.Rotate(new Vector3(0, touchDeltaPosition.x, 0));
        }
    }

}