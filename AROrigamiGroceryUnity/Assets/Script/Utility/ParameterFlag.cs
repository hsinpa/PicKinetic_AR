using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami
{
    public class ParameterFlag
    {
        public class General
        {
            public static int ScreenWidth;
            public static int ScreenHeight;

            public static Vector3 VectorZero = new Vector3(0, 0, 0);
            public static Vector2Int Vector2Zero = new Vector2Int(0, 0);
        }

        public class ShaderProperty {
            public const string MainTex = "_MainTex";
            public const string ShowSideTex = "_ShowSideTex";
            public const string ControlPoints = "_ControlPoints";
            public const string RenderTransition = "_RenderTransition";
        }

        public class ColliderLayer {
            public const int FloorLayer = 1 << 8;
        }
    }
}