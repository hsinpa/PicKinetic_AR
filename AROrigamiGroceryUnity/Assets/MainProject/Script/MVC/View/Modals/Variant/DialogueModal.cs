using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Hsinpa.View
{
    public class DialogueModal : Modal
    {
        [SerializeField]
        private Text titleText;

        [SerializeField]
        private Text contentText;

        [SerializeField]
        private Transform buttonContainer;

        [SerializeField]
        private Button[] buttonPrefab;

        public void SetDialogue(string title, string content, string[] allowBtns, System.Action<int> btnEvent)
        {
            //ResetContent();

            titleText.text = title;
            contentText.text = content;

            RegisterButtons(allowBtns, btnEvent);
        }

        private void RegisterButtons(string[] allowBtns, System.Action<int> btnEvent)
        {
            int btnlength = buttonPrefab.Length;

            for (int i = 0; i < btnlength; i++)
            {
                int index = i;

                Button button = buttonPrefab[i];
                Text textObj = button.GetComponentInChildren<Text>();

                button.gameObject.SetActive( i < allowBtns.Length );

                if (i >= allowBtns.Length) continue;

                textObj.text = allowBtns[i];

                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    Modals.instance.Close();

                    if (btnEvent != null)
                        btnEvent(index);
                });
            }
        }

        private void ResetContent()
        {
            UtilityMethod.ClearChildObject(buttonContainer);
        }


    }
}