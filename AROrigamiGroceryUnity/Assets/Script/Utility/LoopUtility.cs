using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopUtility
{
    private delegate (int, int) LoopDelegate(int x, int y);
    private int _x, _y, _width, _height;
    public int _index => (_x + (_y * _width));

    public enum LoopDirection
    {
        Top, Down, Left, Right
    }
    private LoopDirection _loopDirection;
    private LoopInfo _loopInfo;

    private Dictionary<LoopDirection, LoopDelegate> LoopDirectionTable = new Dictionary<LoopDirection, LoopDelegate>();


    public void SetUp(LoopDirection direction, int startX, int startY, int width, int height)
    {
        _loopInfo = new LoopInfo();
        _loopDirection = direction;
        _x = startX;
        _y = startY;
        _width = width;
        _height = height;

        LoopDirectionTable.Add(LoopDirection.Left, LoopLeft);
    }

    private LoopInfo Loop() {
        
        return _loopInfo;
    }

    private (int, int) LoopLeft(int startX, int startY)
    {
        return (0,0);
    }

    private (int, int) LoopRight(int startX, int startY)
    {
        return (0, 0);
    }

    private (int, int) LoopTop(int startX, int startY)
    {
        return (0, 0);
    }

    private (int, int) LoopBottom(int startX, int startY)
    {
        return (0, 0);
    }

    public struct LoopInfo {
        public int x, y, index;
    }

}
