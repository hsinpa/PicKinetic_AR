using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace AROrigami
{
    public class UnitMovementCtrl
    {
        private float move_speed = 2;

        //Only detect plane collider
        private static RaycastHit[] m_Results = new RaycastHit[1];

        private float _sPerlinX, _sPerlinY;
        private float raycastLength = 10;
        private Vector3 moveDirection = Vector3.zero;

        private Transform transform;

        private enum State { 
            Idle, Rotate, Move
        }

        private State state;

        public UnitMovementCtrl(Transform targetTransform)
        {
            this.transform = targetTransform;
            state = State.Rotate;
            int minP = 0, maxP = 30000;
            _sPerlinX = UtilityMethod.GetRandomNumber(minP, maxP);
            _sPerlinY = UtilityMethod.GetRandomNumber(minP, maxP);
            ChangeForwardDir();
        }

        public void OnUpdate() {

            _sPerlinX += 0.001f;
            _sPerlinY += 0.001f;
            ProcessBahavior();
        }

        private void ProcessBahavior() {
            switch (this.state) {
                case State.Idle:
                    Idle();
                break;

                case State.Rotate:
                    MoveRotate();
                break;

                case State.Move:
                    ObjectMove();
                break;
            }
        }

        private void Idle() { 
            
        }

        private void MoveRotate() {
            Vector3 currentFace = transform.rotation.eulerAngles;
            float faceAngle = GetAngle(currentFace.y);

            float diff = Mathf.Abs( GetAngle(moveDirection.y) - faceAngle);

            if (diff < 1f)
            {
                state = State.Move; 
            }
            else {
                Vector3 miniRotate = Vector3.Lerp(currentFace, moveDirection, 0.1f);
                miniRotate.x = -90;
                miniRotate.z = 0;

                transform.rotation = Quaternion.Euler(miniRotate);
            }
        }

        private void ChangeForwardDir() {

            float randomNum = UtilityMethod.GetRandomNumber(-60, 60);

            randomNum += (moveDirection.y - 180);

            moveDirection.Set(0, Mathf.Abs(GetAngle(randomNum)), 0);
        }

        private void ObjectMove() {

            bool canWalk = DetectIsFloorFront();

            if (canWalk)
            {
                float perlinNoise = (Mathf.PerlinNoise(_sPerlinX, _sPerlinY) * 2) - 1;
                float yDir = (perlinNoise * 360);
                yDir = Mathf.Lerp(moveDirection.y, yDir, 0.1f) * 0.001f;
                
                moveDirection.Set(-90, GetAngle(moveDirection.y + (yDir * 0.0035f)), 0);

                transform.rotation = Quaternion.Euler(moveDirection);
                transform.position = transform.position + (transform.right * Time.deltaTime * (move_speed + (perlinNoise * 2)));
            }
            else
            {
                ChangeForwardDir();
                state = State.Rotate;
            }
        }

        private float GetAngle(float angle) {
            return angle % 360f;
        }

        private bool DetectIsFloorFront() {

            Vector3 direciton = (transform.right) + new Vector3(0, -0.9f, 0);

            int hits = Physics.RaycastNonAlloc(transform.position, direciton, m_Results, raycastLength, ParameterFlag.ColliderLayer.FloorLayer);

            //Debug.Log("Forward "+ direciton + ", hits " + hits);

            Debug.DrawRay(transform.position, direciton, Color.blue);
            return hits > 0;
        }
    }
}