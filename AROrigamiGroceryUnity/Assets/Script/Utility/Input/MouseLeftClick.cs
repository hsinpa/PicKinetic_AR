using UnityEngine;
using UnityEngine.InputSystem;

namespace Hsinpa.Utility.Input
{
    public class MouseLeftClick : BtnClickInterface
    {
        public Vector2 fingerPosition => Mouse.current.position.ReadValue();

        public bool OnClick()
        {
            return Mouse.current.leftButton.isPressed;
        }

        public bool OnClickDown()
        {
            return Mouse.current.leftButton.wasPressedThisFrame;
        }

        public bool OnClickUp()
        {
            return Mouse.current.leftButton.wasReleasedThisFrame;
        }
    }
}