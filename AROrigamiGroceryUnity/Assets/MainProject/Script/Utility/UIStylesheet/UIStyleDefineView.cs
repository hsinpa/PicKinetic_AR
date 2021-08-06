using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.UIStyle
{
    public class UIStyleDefineView : MonoBehaviour
    {
        public UIStyle.UIStylesheet UIStylesheet;

        public UIStylesheet.ColorSet ColorSet;
        public int colorSetIndex;

        public UIStylePredefine Predefine;

        private void OnEnable()
        {

        }
    }
}