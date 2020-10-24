using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace AROrigami
{
    public class UnitMovementCtrl : MonoBehaviour
    {
        //Only detect plane collider
        private static RaycastHit[] m_Results = new RaycastHit[1];

        private float _sPerlinX, _sPerlinY;
        private float raycastLength = 10;

        private void SetUp()
        {
            int minP = 0, maxP = 30000;
            _sPerlinX = UtilityMethod.GetRandomNumber(minP, maxP);
            _sPerlinY = UtilityMethod.GetRandomNumber(minP, maxP);
        }

        private void Update() {
            DetectIsFloorFront();
        }

        private bool DetectIsFloorFront() {

            Vector3 direciton = transform.forward + new Vector3(0, -0.6f, 0);

            int hits = Physics.RaycastNonAlloc(transform.position, direciton, m_Results, raycastLength, ParameterFlag.ColliderLayer.FloorLayer);

            //Debug.Log("Forward "+ direciton + ", hits " + hits);

            Debug.DrawRay(transform.position, direciton, Color.blue);
            return true;
        }
    }
}