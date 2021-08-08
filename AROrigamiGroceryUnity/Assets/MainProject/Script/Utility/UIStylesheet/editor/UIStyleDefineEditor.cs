using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;

namespace Hsinpa.UIStyle
{
    [CustomEditor(typeof(UIStyleDefineView))]
    public class UIStyleDefineEditor : Editor
    {
        SerializedProperty UIStylesheet;
        SerializedProperty Predefine;

        private int _choiceIndex = 0;

        UIStyleDefineView myTarget;

        UIStyle.UIStylesheet.ColorSet ColorSet => myTarget.UIStylesheet.ColorSets[myTarget.colorSetIndex];

        Image ImageUI;

        void OnEnable()
        {
            myTarget = (UIStyleDefineView)target;
            ImageUI = myTarget.gameObject.GetComponent<Image>();
            UIStylesheet = serializedObject.FindProperty("UIStylesheet");
            Predefine = serializedObject.FindProperty("Predefine");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(UIStylesheet);
            EditorGUILayout.PropertyField(Predefine);

            if (myTarget.UIStylesheet != null) {
                var tagArray = myTarget.UIStylesheet.ColorSets.Select(x => x.tag).ToArray();
                myTarget.colorSetIndex = EditorGUILayout.Popup("Color", myTarget.colorSetIndex, tagArray);
            }

            SetImageColor();
            SetStylePredefine(myTarget.Predefine);

            EditorUtility.SetDirty(myTarget);
            serializedObject.ApplyModifiedProperties();
        }

        private void SetImageColor() {
            if (ImageUI != null && ColorSet.isValid) {
                ImageUI.color = ColorSet.color;
            }
        }

        private void SetStylePredefine(UIStylePredefine predefine) {
            if (predefine == null) return;

            var type = predefine.GetType();
            if (type == typeof(UIStyleText))
                SetFontPredefine((UIStyleText) predefine);
            
        }

        private void SetFontPredefine(UIStyleText uIStyleText) {
            if (!ColorSet.isValid) return;

            Text textObj = myTarget.GetComponent<Text>();

            if (textObj != null) {
                textObj.font = uIStyleText.Font;
                textObj.fontSize = uIStyleText.FontSize;
                textObj.color = ColorSet.color;

                return;
            }

            TMPro.TextMeshProUGUI proText = myTarget.GetComponent<TMPro.TextMeshProUGUI>();
            if (proText != null)
            {
                proText.fontSize = uIStyleText.FontSize;
                proText.color = ColorSet.color;
                return;
            }
        }

    }
}