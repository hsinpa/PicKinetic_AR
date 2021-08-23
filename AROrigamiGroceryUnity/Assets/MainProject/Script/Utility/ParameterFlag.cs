using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PicKinetic
{
    public class ParameterFlag
    {
        public class General
        {
            public static int ScreenWidth;
            public static int ScreenHeight;

            public static Vector3 VectorZero = new Vector3(0, 0, 0);
            public static Vector2Int Vector2Zero = new Vector2Int(0, 0);

            public const float ColorIdentityRatio = 0.0039215f;

            public static Color PositiveColor = new Color(84 * ColorIdentityRatio, 109 * ColorIdentityRatio, 229 * ColorIdentityRatio);
            public static Color NegativeColor = new Color(196 * ColorIdentityRatio, 69 * ColorIdentityRatio, 105 * ColorIdentityRatio);
        }

        public class ShaderProperty {
            public const string MainTex = "_MainTex";
            public const string MainColor = "_MainColor";
            public const string ShowSideTex = "_ShowSideTex";
            public const string ControlPoints = "_ControlPoints";
            public const string RenderTransition = "_RenderTransition";
        }

        public class ColliderLayer {
            public const int FloorLayer = 1 << 8;
        }

        public class SaveSystem {
            public const string DiskFolder = "MeshTextures";
            public const string TempFolder = "Temps";
        }
    }
}