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
        SerializedProperty ColorSet;
        SerializedProperty Predefine;

        private int _choiceIndex = 0;

        void OnEnable()
        {
            UIStylesheet = serializedObject.FindProperty("UIStylesheet");
            ColorSet = serializedObject.FindProperty("ColorSet");
            Predefine = serializedObject.FindProperty("Predefine");
        }


        public override void OnInspectorGUI()
        {
            UIStyleDefineView myTarget = (UIStyleDefineView)target;

            serializedObject.Update();
            EditorGUILayout.PropertyField(UIStylesheet);
            EditorGUILayout.PropertyField(Predefine);
            EditorGUILayout.PropertyField(ColorSet);

            if (myTarget.UIStylesheet != null) {
                var tagArray = myTarget.UIStylesheet.ColorSets.Select(x => x.tag).ToArray();
                myTarget.colorSetIndex = EditorGUILayout.Popup("Color", myTarget.colorSetIndex, tagArray);
                myTarget.ColorSet = myTarget.UIStylesheet.ColorSets[_choiceIndex];
            }

            SetStylePredefine(myTarget.Predefine);
            
            serializedObject.ApplyModifiedProperties();
        }

        private void SetStylePredefine(UIStylePredefine predefine) {
            if (predefine == null) return;

            var type = predefine.GetType();
            if (type == typeof(UIStyleText))
                SetFontPredefine((UIStyleText) predefine);
            
        }

        private void SetFontPredefine(UIStyleText uIStyleText) {
            UIStyleDefineView myTarget = (UIStyleDefineView)target;

            if (!myTarget.ColorSet.isValid) return;


            Text textObj = myTarget.GetComponent<Text>();

            if (textObj != null) {
                textObj.font = uIStyleText.Font;
                textObj.fontSize = uIStyleText.FontSize;
                textObj.color = myTarget.ColorSet.color;

                return;
            }

            TMPro.TextMeshProUGUI proText = myTarget.GetComponent<TMPro.TextMeshProUGUI>();
            if (proText != null)
            {
                proText.fontSize = uIStyleText.FontSize;
                proText.color = myTarget.ColorSet.color;
                return;
            }
        }

    }
}