using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hsinpa.Utility.Input {
    public class InputWrapper
    {
        public Vector2 cacheMousePosition;
        public Vector2 mousePosition => Mouse.current.position.ReadValue();

        private BtnClickInterface _primaryBtnClick;
        public BtnClickInterface primaryBtnClick => _primaryBtnClick;

        public InputWrapper() {
#if UNITY_EDITOR
            _primaryBtnClick = new MouseLeftClick();
#else
            primaryBtnClick = new TouchClick();
#endif
        }

        public void OnUpdate() {
            cacheMousePosition = Mouse.current.position.ReadValue();
        }
    }
}
