﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder
{
    public SquareGrid squareGrid;

    public MeshBuilder() {
        squareGrid = new SquareGrid();
    }

    public void GenerateMesh(Color[] map, int width, int height, float squareSize) {
        squareGrid.Calculate(map, width, height, squareSize);
    }

    public class SquareGrid {
        public Square[,] sqaures;
        private ControlNode[,] controlNodes;

        private float _mapWidth;
        private float _mapHeight;

        public float mapWidth => _mapWidth;
        public float mapHeight => _mapHeight;


        public void Calculate(Color[] map, int width, int height, float squareSize) {
            int nodeCountX = width;
            int nodeCountY = height;
            _mapWidth = nodeCountX * squareSize;
            _mapHeight = nodeCountY * squareSize;

            int totalLen = map.Length;

            if (controlNodes == null)
                controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    int mapIndex = (nodeCountY * y) + x;

                    Vector3 pos = new Vector3(-(mapWidth / 2f) + x * squareSize + (squareSize / 2f), 0, -(mapHeight / 2f) + y * squareSize + (squareSize / 2));

                    controlNodes[x, y] = new ControlNode(pos, map[mapIndex].r <= 0.1f, squareSize);
                }
            }

            if (sqaures == null)
                sqaures = new Square[nodeCountX - 1, nodeCountY - 1];

            for (int x = 0; x < nodeCountX - 1; x++)
            {
                for (int y = 0; y < nodeCountY - 1; y++)
                {
                    //TopLeft, TopRight, BottomRight, BottomLeft
                    var TopLeft = controlNodes[x, y + 1];
                    var TopRight = controlNodes[x + 1, y + 1];
                    var BottomRight = controlNodes[x + 1, y];
                    var BottomLeft = controlNodes[x, y];

                    sqaures[x, y] = new Square(TopLeft, TopRight, BottomRight, BottomLeft);
                }
            }
        }
    }

    public struct Square {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerLeft, centerBottom;
        public int configuration => GetConfigurationCode(topLeft.activeInt, topRight.activeInt, bottomRight.activeInt, bottomLeft.activeInt);

        public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomRight, ControlNode bottomLeft)
        {
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
            this.bottomLeft = bottomLeft;

            this.centerTop = topLeft.right;
            this.centerRight = bottomRight.above;
            this.centerBottom = bottomLeft.right;
            this.centerLeft = bottomLeft.above;
        }

        private int GetConfigurationCode(int topLeft, int topRight, int bottomRight, int bottomLeft) {
            return (topLeft * 8) + (topRight * 4) + (bottomRight * 2) + (bottomLeft * 1);
        }
    }

    public class Node {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 _pos) {
            position = _pos;
        }
    }

    public class ControlNode : Node {
        public bool active;
        public int activeInt => (active) ? 1 : 0;
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
            active = _active;
            above = new Node(position + Vector3.forward * (squareSize / 2f));
            right = new Node(position + Vector3.right * (squareSize / 2f));
        }
    }
}
