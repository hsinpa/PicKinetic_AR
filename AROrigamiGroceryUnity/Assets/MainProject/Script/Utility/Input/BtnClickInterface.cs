using UnityEngine;

namespace Hsinpa.Utility.Input
{
    public interface BtnClickInterface
    {
        Vector2 fingerPosition { get; }

        bool OnClick();
        bool OnClickDown();
        bool OnClickUp();
    }
}