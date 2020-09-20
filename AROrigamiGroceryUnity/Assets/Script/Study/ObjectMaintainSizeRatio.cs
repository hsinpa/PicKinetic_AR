using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMaintainSizeRatio : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;

    [SerializeField, Range(0, 20)]
    private float _SizeStrength = 1;

    [SerializeField, Range(-2, 2)]
    private float _HeightStrength = 1;

    [SerializeField]
    private Collider _ObjectCollider;

    private Vector3 _OriginalPos;    

    private void Start()
    {
        _OriginalPos = transform.position;
        Debug.Log(_ObjectCollider.bounds.size);
    }

    private void Update()
    {
        float size = _ObjectCollider.bounds.size.y;
        float sizeMagnitue = (_camera.transform.position - transform.position).magnitude * _SizeStrength;
        transform.localScale = new Vector3(sizeMagnitue, sizeMagnitue, sizeMagnitue);
    }


}
