using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    public SquareGrid squareGrid;

    public void GenerateMesh(Color[,] map, float squareSize) {
        squareGrid = new SquareGrid(map, squareSize);
    }

    public class SquareGrid {
        public Square[,] sqaures;

        public SquareGrid(Color[,] map, float squareSize) {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++) {
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-(mapWidth / 2f) + x * squareSize + (squareSize / 2f), 0, -(mapHeight / 2f) + y * squareSize + (squareSize / 2));
                    controlNodes[x, y] = new ControlNode(pos, map[x, y].r == 1, squareSize);
                }
            }

            this.sqaures = new Square[nodeCountX -1, nodeCountY -1];
            for (int x = 0; x < nodeCountX; x++)
            {
                for (int y = 0; y < nodeCountY; y++)
                {
                    //TopLeft, TopRight, BottomRight, BottomLeft
                    this.sqaures[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
                }
            }
        }
    }

    public class Square {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerLeft, centerBottom;

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
        public Node above, right;

        public ControlNode(Vector3 _pos, bool _active, float squareSize) : base(_pos) {
            active = _active;
            above = new Node(position + Vector3.forward * (squareSize / 2f));
            right = new Node(position + Vector3.right * (squareSize / 2f));
        }

    }    
}
