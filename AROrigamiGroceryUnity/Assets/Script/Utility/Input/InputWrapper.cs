using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hsinpa.Utility.Input {
    public class InputWrapper
    {
        public Vector2 mousePosition => _primaryBtnClick.fingerPosition;

        private BtnClickInterface _primaryBtnClick;
        public BtnClickInterface primaryBtnClick => _primaryBtnClick;

        public InputWrapper() {
#if UNITY_EDITOR
            _primaryBtnClick = new MouseLeftClick();
#else
            _primaryBtnClick = new TouchClick();
#endif
        }
    }
}
