using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Hsinpa.Utility.Input;

namespace Hsinpa.Utility
{
    public class UnitInspectModule
    {
        private bool hasHitOnPuffObj = false;
        private Vector2 lastTouchPoint;
        private Vector2 currentTouchPoint;
        private Transform SelectedObject;

        private GestureEvent gestureEvent = GestureEvent.None;
        private Face currentFace = Face.Front;
        private int rotDir = 1;
        private float recordRotationY;
        private Quaternion lerpQuaterion;
        private Vector3 modelRotateDir;

        public enum DragDir { VerticalUp, VerticalDown, Horizontal, None };
        public enum Face { Front, RightSide, Back, LeftSide };
        public enum GestureEvent { Release, Save, None };

        private float moveXDist => (currentTouchPoint - lastTouchPoint).x;
        private float moveYDist => (currentTouchPoint - lastTouchPoint).y;
        private float absX => Mathf.Abs(moveXDist);
        private float absY => Mathf.Abs(moveYDist);

#if UNITY_EDITOR
        private float zDepth = 2.8f;
#else
        private float zDepth = 0.1f;
#endif

        private Vector3 centerPosition => _camera.transform.position + (_camera.transform.forward * zDepth);

        private float DragThreshold = 0.1f;
        private Camera _camera;
        private RaycastHit[] raycastHits = new RaycastHit[1];

        private PointerEventData eventData;
        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        private System.Func<Transform, bool> SetCurrentSelectedObjectCallback;
        private System.Action<Face> SetFaceCallback;
        private System.Action<GestureEvent> ReleaseObjectCallback;
        private System.Action<DragDir, float, float, Vector3> ProcessVerticalCallback;
        private InputWrapper inputWrapper;

        private DragDir dragMode = DragDir.None;

        public UnitInspectModule(
             Vector3 modelRotateDir,
                                InputWrapper inputWrapper,
                                System.Func<Transform, bool> SetCurrentSelectedObjectCallback,
                                System.Action<Face> SetFaceCallback,
                                System.Action<GestureEvent> ReleaseObjectCallback,
                                System.Action<DragDir, float, float, Vector3> ProcessVerticalCallback, float dragThreshold, Camera camera) {
            this.modelRotateDir = modelRotateDir;
            this.inputWrapper = inputWrapper;
            this.SetCurrentSelectedObjectCallback = SetCurrentSelectedObjectCallback;
            this.SetFaceCallback = SetFaceCallback;
            this.ReleaseObjectCallback = ReleaseObjectCallback;
            this.ProcessVerticalCallback = ProcessVerticalCallback;
            this.DragThreshold = dragThreshold;
            this._camera = camera;

            this.eventData = new PointerEventData(EventSystem.current);
        }

        public void SetInputSelectObject(Transform puffItem) {
            this.SelectedObject = puffItem;

            if (this.SelectedObject == null)
                gestureEvent = GestureEvent.None;
        }

        private void PlaySmoothAnimation() {
            if (SelectedObject == null) return;

            GraduallyRotateToFace(currentFace);
            GraudaulyFlyToCenter();
        }

        public void OnUpdate()
        {
            currentTouchPoint = inputWrapper.mousePosition;

            if (HasHitUIComponent()) {
                PlaySmoothAnimation();
                return;
            }

            if (!hasHitOnPuffObj && inputWrapper.primaryBtnClick.OnClickDown())
            {
                hasHitOnPuffObj = HasHitPuffObject();

                if (hasHitOnPuffObj)
                {
                    lastTouchPoint = currentTouchPoint;

                    if (SelectedObject == null)
                        this.SetCurrentSelectedObjectCallback(raycastHits[0].transform);
                }
            }

            if (SelectedObject == null) return;

            if (!hasHitOnPuffObj)
            {
                PlaySmoothAnimation();
                return;
            }

            DragDir dragDirection = FindDragDirection();

            //if (dragDirection == DragDir.Horizontal && dragMode != DragDir.VerticalDown) {
            //    dragMode = DragDir.Horizontal;
            //    ProcessRotation();
            //}

            if ((dragDirection == DragDir.VerticalDown || dragDirection == DragDir.VerticalUp) && dragMode != DragDir.Horizontal )
            {
                dragMode = DragDir.VerticalDown;
                ProcessVertical(dragDirection);
            }

            if (inputWrapper.primaryBtnClick.OnClickUp())
            {
                dragMode = DragDir.None;
                hasHitOnPuffObj = false;
                recordRotationY = SelectedObject.transform.eulerAngles.y;
                UnitInspectorStatic.SharedVectorUnit.Set(SelectedObject.transform.eulerAngles.x, recordRotationY, SelectedObject.transform.eulerAngles.z);
                lerpQuaterion = Quaternion.Euler(UnitInspectorStatic.SharedVectorUnit);
                currentFace = FindTheBestFace();
                this.SetFaceCallback(currentFace);

                rotDir = ((int)currentFace) * 90;

                if (rotDir == 0 && recordRotationY >= 180)
                    rotDir = 360;

                this.ReleaseObjectCallback(gestureEvent);
            }
        }

        private DragDir FindDragDirection()
        {
            //Vertical check first
            if (absY > DragThreshold && dragMode != DragDir.Horizontal)
            {
                return (moveYDist > 0) ? DragDir.VerticalUp : DragDir.VerticalDown;
            }

            if (absX > DragThreshold && dragMode != DragDir.VerticalDown)
                return DragDir.Horizontal;

            return DragDir.None;
        }

        private bool HasHitPuffObject()
        {
            Ray ray = _camera.ScreenPointToRay(currentTouchPoint);

            int hitCount = Physics.RaycastNonAlloc(ray, raycastHits, 100, UnitInspectorStatic.TargetLayer);

            return hitCount > 0;
        }

        private void ProcessVertical(DragDir dragDir)
        {

            float offset = Mathf.Clamp((absY - DragThreshold) * 0.003f, -1, 1);
            float ratio = 1 - (Mathf.Abs(offset * 2f) / 5f);
            if (dragDir == DragDir.VerticalDown) offset *= -1;

            ProcessVerticalCallback(dragDir, ratio, offset, centerPosition);

            if (ratio <= 0.85f)
            {
                gestureEvent = dragDir == DragDir.VerticalDown ? GestureEvent.Save : GestureEvent.Release;
            }
            else
            {
                gestureEvent = GestureEvent.None;
            }
        }

        private int ProcessRotation()
        {
            Vector2 currentStandPoint = currentTouchPoint;
            float direction = (currentStandPoint - lastTouchPoint).x;
            direction = Mathf.Clamp(direction, -5, 5);

            //Rotation
            UnitInspectorStatic.SharedVectorUnit.Set(direction * modelRotateDir.x, direction * modelRotateDir.y, direction * modelRotateDir.z);
            SelectedObject.transform.Rotate(UnitInspectorStatic.SharedVectorUnit, Space.Self);

            return (direction > 0) ? 1 : -1;
        }

        private Face FindTheBestFace()
        {
            float angle = GetAngle((_camera.transform.forward));

            float rawYRot = (SelectedObject.transform.rotation.eulerAngles.y + angle) % 360;

            int face = Mathf.RoundToInt(rawYRot / 90f) % 4;

            if (face < 0)
                face = 4 + face;

            return (Face)face;
        }

        private void GraudaulyFlyToCenter()
        {
            Vector3 frontPosition = _camera.transform.position + (_camera.transform.forward * zDepth);

            SelectedObject.transform.position = Vector3.Lerp(SelectedObject.transform.position, frontPosition, 0.1f);
        }

        private void GraduallyRotateToFace(Face face)
        {
            float angle = GetAngle( (_camera.transform.forward));
            var objectEulerVector = SelectedObject.transform.rotation.eulerAngles;
            UnitInspectorStatic.SharedVectorUnit.Set(objectEulerVector.x, rotDir - angle, objectEulerVector.z);
            lerpQuaterion = Quaternion.Lerp(lerpQuaterion, Quaternion.Euler(UnitInspectorStatic.SharedVectorUnit), 0.1f);

            SelectedObject.transform.rotation = lerpQuaterion;
        }

        private bool HasHitUIComponent() {
            raycastResults.Clear();

            eventData.position = currentTouchPoint;
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults.Count > 0;
        }

        private float GetAngle(Vector3 p_direction) {
            var angle = Mathf.Atan2(p_direction.z, p_direction.x);   //radians
                                                      // you need to devide by PI, and MULTIPLY by 180:
            float degrees = 180 * (angle / Mathf.PI);  //degrees
            return degrees -90; //round number, avoid decimal fragments
        }
    }
}