using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace AROrigami
{
    public class SpaceDeformation
    {
        Mesh _mesh;
        DeformControlPoint[] _controlPoints;


        int _controlPointCount;
        int _vertexCount;

        //Ctrl Points 
        Vector3[] _pointPos;
        Vector3[] _oriPointPos;

        //Vertices
        Vector3[] _oriVertices;
        Vector3[] _vertices;

        public void SetUpMesh(Mesh mesh)
        {
            _mesh = mesh;
            _oriVertices = _mesh.vertices;
            _vertexCount = _oriVertices.Length;
            _vertices = new Vector3[_vertexCount];
        }

        public void SetUpControllerPoint(DeformControlPoint[] controlPoints)
        {
            _controlPoints = controlPoints;
            _controlPointCount = controlPoints.Length;

            _oriPointPos = controlPoints.Select(x => x.transform.position).ToArray<Vector3>();
            _pointPos = new Vector3[_controlPointCount];
        }

        public void OnUpdate()
        {
            _pointPos = UpdateVectorPos(_pointPos);
            UpdateVerticePos();

            _mesh.SetVertices(_vertices);
        }

        private void UpdateVerticePos() {
            for (int i = 0; i < _vertexCount; i++)
            {
                _vertices[i] = _oriVertices[i] + GetCtrlPointImpact(_oriVertices[i], _controlPoints);
            }
        }

        private Vector3[] UpdateVectorPos(Vector3[] vArray)
        {
            for (int i = 0; i < _controlPointCount; i++)
            {
                DeformControlPoint cPointObj = _controlPoints[i];
                Vector3 oPoint = _oriPointPos[i];
                Vector3 currentPos = cPointObj.transform.position;
                vArray[i] = currentPos - oPoint;
            }

            return vArray;
        }

        private Vector3 GetCtrlPointImpact(Vector3 vertice, DeformControlPoint[] ctrlPoints)
        {
            Vector3 changeInDir = new Vector3();

            for (int i = 0; i < _controlPointCount; i++)
            {
                DeformControlPoint cPointObj = ctrlPoints[i];
                Vector3 changeInPos = _pointPos[i];

                float weight = (cPointObj.Radius) - Vector3.Distance(vertice, _oriPointPos[i]);
                float w_strength = Mathf.Clamp(weight, 0, weight) / cPointObj.Radius;
                int step = (w_strength > 0) ? 1 : 0;
                changeInDir += Vector3.Lerp(changeInPos * step, changeInPos * (w_strength), 1-cPointObj.Interpolation);
            }

            return changeInDir;
        }

        

    }
}