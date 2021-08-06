using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.UIStyle
{
    [CreateAssetMenu(fileName = "UIStyleText", menuName = "UIStyle/UIStyleText", order = 2)]
    public class UIStyleText : UIStylePredefine
    {
        public Font Font;
        public int FontSize;
    }
}