using UnityEngine.InputSystem;


namespace Hsinpa.Utility.Input
{
    public class TouchClick : BtnClickInterface
    {
        bool isPressLastFrame = false;

        public bool OnClick()
        {
            UnityEngine.Debug.Log("touch OnClick");
            return Touchscreen.current.primaryTouch.press.isPressed;
        }

        public bool OnClickDown()
        {
            UnityEngine.Debug.Log("touch OnClickDown");

            bool currentState = Touchscreen.current.primaryTouch.press.isPressed;
            bool isClickDown = !isPressLastFrame && currentState;
            isPressLastFrame = currentState;

            return isClickDown;
        }

        public bool OnClickUp()
        {
            UnityEngine.Debug.Log("touch OnClickUp");

            bool currentState = Touchscreen.current.primaryTouch.press.isPressed;
            bool isClickUp = isPressLastFrame && !currentState;
            isPressLastFrame = currentState;

            return isClickUp;
        }
    }
}