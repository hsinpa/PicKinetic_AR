using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class UnitMovementCtrl : MonoBehaviour
{
    //Only detect plane collider
    private static RaycastHit[] m_Results = new RaycastHit[1];

    private float _sPerlinX, _sPerlinY;

    private void SetUp() {
        int minP = 0, maxP = 30000;
        _sPerlinX = UtilityMethod.GetRandomNumber(minP, maxP);
        _sPerlinY = UtilityMethod.GetRandomNumber(minP, maxP);
    }





}
