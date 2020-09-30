using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopUtility
{
    private delegate (int, int) LoopDelegate(int x, int y);
    private int _startX, _startY, _x, _y, _width, _height;
    public int _index => (_x + (_y * _width));

    public enum LoopDirection
    {
        Top, Down, Left, Right
    }
    private LoopDirection _loopDirection;
    private LoopInfo _loopInfo;

    private Dictionary<LoopDirection, IEnumerable<LoopInfo>> LoopDirectionTable = new Dictionary<LoopDirection, IEnumerable<LoopInfo>>();

    public LoopUtility() {
        LoopDirectionTable.Add(LoopDirection.Left, LoopLeft());
        LoopDirectionTable.Add(LoopDirection.Right, LoopRight());
        LoopDirectionTable.Add(LoopDirection.Top, LoopTop());
        LoopDirectionTable.Add(LoopDirection.Down, LoopBottom());
    }

    public void SetUp(int startX, int startY, int width, int height)
    {
        _loopInfo = new LoopInfo(width);
        _x = startX;
        _y = startY;
        _startX = startX;
        _startY = startY;
        _width = width;
        _height = height;
    }

    public IEnumerable<LoopInfo> GetGeneticLooper(LoopDirection direction) {
        _loopDirection = direction;

        if (LoopDirectionTable.TryGetValue(_loopDirection, out IEnumerable<LoopInfo> enumerable)) {
            return enumerable;
        }

        return LoopDirectionTable[LoopDirection.Left];
    }


    //Left, Down Approach
    IEnumerable<LoopInfo> LoopLeft()
    {
        for (int y = _startY - 1; y >= 0; y--)
        {
            for (int x = _startX; x < _width; x++)
            {
                _loopInfo.Set(x, y);
                yield return _loopInfo;
            }
        }
    }

    //Right, Up Approach
    IEnumerable<LoopInfo> LoopRight()
    {
        for (int y = _startY; y < _height; y++)
        {
            for (int x = _startX; x >= 0; x--)
            {
                _loopInfo.Set(x, y);
                yield return _loopInfo;
            }
        }
    }

    //Simple Topdown Approach
    IEnumerable<LoopInfo> LoopTop()
    {
        for (int y = _startY - 1; y >= 0; y--)
        {
            _loopInfo.Set(_startX, y);
            yield return _loopInfo;
        }
    }

    //Simple BottomUp Approach
    IEnumerable<LoopInfo> LoopBottom()
    {
        for (int y = 0; y < _height; y++)
        {
            _loopInfo.Set(_startX, y);
            yield return _loopInfo;
        }
    }

    public struct LoopInfo {
        public int x, y; // Dynamic Change
        private int width; // Set Once

        public int index => (x + (y * width));

        public LoopInfo(int p_width) {
            x = 0;
            y = 0;
            width = p_width;
        }

        public void Set(int p_x, int p_y) {
            x = p_x;
            y = p_y;
        }
    }

}
