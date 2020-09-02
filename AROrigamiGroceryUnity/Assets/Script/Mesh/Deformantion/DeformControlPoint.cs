using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami
{
    public class DeformControlPoint : MonoBehaviour
    {
        [SerializeField, Range(0f, 10f)]
        private float Range = 1;
        public float Radius => Range * 0.5f;

        [Range(0f, 1f)]
        public float Interpolation = 0;

        [SerializeField]
        private Color GizmosRangeColor = Color.yellow;

        void OnDrawGizmosSelected()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = GizmosRangeColor;
            Gizmos.DrawSphere(transform.position, Range);
        }
    }
}