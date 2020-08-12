using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{

    public SquareGrid squareGrid;

    public void GenerateMesh(Color[] map, int width, int height, float squareSize) {
        squareGrid = new SquareGrid(map, width, height, squareSize);
    }

    public class SquareGrid {
        public Square[,] sqaures;

        public SquareGrid(Color[] map, int width, int height,  float squareSize) {
            int nodeCountX = width;
            int nodeCountY = height;
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            int totalLen = map.Length;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];


            for (int x = 0; x < nodeCountX; x++) {
                for (int y = 0; y < nodeCountY; y++)
                {

                    int mapIndex = (nodeCountY * y) + x;


                    Vector3 pos = new Vector3(-(mapWidth / 2f) + x * squareSize + (squareSize / 2f), 0, -(mapHeight / 2f) + y * squareSize + (squareSize / 2));
                    controlNodes[x, y] = new ControlNode(pos, map[mapIndex].r <= 0.1f, squareSize);
                }
            }

            sqaures = new Square[nodeCountX -1, nodeCountY -1];
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
