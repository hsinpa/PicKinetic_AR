using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hsinpa.Utility;
using Hsinpa.Utility.Input;
using PicKinetic.View;
using DG.Tweening;
using Hsinpa.View;
using PicKinetic.Model;

namespace PicKinetic.Controller
{
    public class UnitInspectorCtrl : ObserverPattern.Observer
    {

        [Header("UI")]
        [SerializeField]
        private MainCanvasView MainCanvasView;

        [SerializeField, Range(0.1f, 200)]
        private float DragThreshold = 0.1f;

        private UnitInspectModule unitInspectModule;

        private InputWrapper inputWrapper;

        private MeshObject selectedMeshObject;

        private ARInspectView arInspectView;

        private TextureModel texModel;

        public override void OnNotify(string p_event, params object[] p_objects)
        {
            switch (p_event)
            {

            }
        }

        private void Start()
        {
            Camera camera = Camera.main;
            inputWrapper = new InputWrapper();
            unitInspectModule = new UnitInspectModule(new Vector3(0, 0, 1), inputWrapper, 
                                SetCurrentSelectedObject, SetFaceInfo, ReleaseSelectObject, ProcessVertical, DragThreshold, camera);

            arInspectView = MainCanvasView.GetCanvasWithType<ARInspectView>();

            texModel = PicKineticAR.Instance.ModelManager.GetModel<TextureModel>();
        }

        private void Update()
        {
            if (unitInspectModule == null) return;
            unitInspectModule.OnUpdate();
        }

        private void SetFaceInfo(UnitInspectModule.Face p_face)
        {
        }

        private void SetInspectViewEvent()
        {

        }

        private void ProcessVertical(UnitInspectModule.DragDir dragDir, float ratio, float offset, Vector3 puff_center)
        {
            selectedMeshObject.transform.position = new Vector3(puff_center.x, puff_center.y + offset, puff_center.z);

            arInspectView.PlayHintAnimation(false);
            arInspectView.CanvasGroup.DOKill();
            arInspectView.CanvasGroup.interactable = false;
            arInspectView.CanvasGroup.blocksRaycasts = false;
            arInspectView.CanvasGroup.alpha = ratio * 0.75f;
        }

        private bool SetCurrentSelectedObject(Transform item)
        {
            selectedMeshObject = item.GetComponent<MeshObject>();

            selectedMeshObject.enabled = false;
            unitInspectModule.SetInputSelectObject(item);

            MainCanvasView.SetMainCanvasState<ARMainUIView>(false, animation: false);

            var arInsector = MainCanvasView.SetMainCanvasState<ARInspectView>(true, animation: true);
            arInsector.SetARInsector(selectedMeshObject, OnSaveBtnClick);
            arInsector.PlayHintAnimation(true);

            AutoSetActionBtnStyle();

            return true;
        }

        private void OnSaveBtnClick() {
            if (selectedMeshObject == null) return;

            bool isSaved = texModel.SRPTextureRoot.IsDataExist(selectedMeshObject.meshJsonData.id);

            if (!isSaved)
            {
                texModel.SaveTexData(selectedMeshObject.meshJsonData);
                arInspectView.SetActionBtnStyle(ParameterFlag.General.NegativeColor, TempStringTable.Scan.DialogueMeshRemoveTitle);
            }
            else {
                var dialogueModal = Modals.instance.OpenModal<DialogueModal>();
                dialogueModal.SetDialogue(TempStringTable.Scan.DialogueMeshRemoveTitle, TempStringTable.Scan.DialogueMeshRemoveContent, 
                                            new string[] { TempStringTable.Scan.DialogueGeneralConfirm, TempStringTable.Scan.DialogueGeneralReject }, 
                (x => {
                    if (x == 0)
                    {
                        texModel.RemoveTexData(selectedMeshObject.meshJsonData);
                        arInspectView.SetActionBtnStyle(ParameterFlag.General.PositiveColor, TempStringTable.Scan.DialogueMeshSaveTitle);
                    }
                }));
            }
        }

        private void AutoSetActionBtnStyle() {
            bool isSaved = texModel.SRPTextureRoot.IsDataExist(selectedMeshObject.meshJsonData.id);

            Color color = isSaved ? ParameterFlag.General.NegativeColor : ParameterFlag.General.PositiveColor;
            string btnString = isSaved ? TempStringTable.Scan.DialogueMeshRemoveTitle : TempStringTable.Scan.DialogueMeshSaveTitle;

            arInspectView.SetActionBtnStyle(color, btnString);
        }

        private void ReleaseSelectObject(UnitInspectModule.GestureEvent gestureInput)
        {
            if (gestureInput != UnitInspectModule.GestureEvent.None) {

                selectedMeshObject.enabled = true;
                selectedMeshObject = null;

                unitInspectModule.SetInputSelectObject(null);
                MainCanvasView.SetMainCanvasState<ARMainUIView>(true, animation:true);
                MainCanvasView.SetMainCanvasState<ARInspectView>(false, animation: false);

                return;
            }

            MainCanvasView.SetMainCanvasState<ARInspectView>(true, animation: true);
        }

    }
}