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
}
