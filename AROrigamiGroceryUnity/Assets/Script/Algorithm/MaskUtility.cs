using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskUtility
{
    public static readonly List<Vector2Int> EightNeighborList = new List<Vector2Int> {
            new Vector2Int(-1, 1), new Vector2Int(0, 1),new Vector2Int(1, 1),
            new Vector2Int(1, 0), new Vector2Int(1, -1),
            new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0) };

    public static readonly List<Vector2Int> FourNeighborList = new List<Vector2Int> {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0), 
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0) };

    public static Color[] ColorFromInt(int[] binaryArray)
    {
        int l = binaryArray.Length;
        Color[] colors = new Color[l];
        Color color = new Color();
        color.a = 1;

        for (int i = 0; i < l; i++)
        {
            color.r = binaryArray[i];
            color.g = binaryArray[i];
            color.b = binaryArray[i];
            colors[i] = color;
        }

        return colors;
    }
}
