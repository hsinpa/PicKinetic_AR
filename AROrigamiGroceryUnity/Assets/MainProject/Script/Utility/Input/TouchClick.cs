using UnityEngine;
using UnityEngine.InputSystem;


namespace Hsinpa.Utility.Input
{
    public class TouchClick : BtnClickInterface
    {
        bool isPressLastFrame = false;

        public Vector2 fingerPosition => Touchscreen.current.primaryTouch.position.ReadValue();

        public bool OnClick()
        {
            return Touchscreen.current.primaryTouch.press.isPressed;
        }

        public bool OnClickDown()
        {
            bool currentState = Touchscreen.current.primaryTouch.press.isPressed;
            bool isClickDown = !isPressLastFrame && currentState;
            isPressLastFrame = currentState;

            return isClickDown;
        }

        public bool OnClickUp()
        {
            bool currentState = Touchscreen.current.primaryTouch.press.isPressed;
            bool isClickUp = isPressLastFrame && !currentState;
            isPressLastFrame = currentState;

            return isClickUp;
        }
    }
}