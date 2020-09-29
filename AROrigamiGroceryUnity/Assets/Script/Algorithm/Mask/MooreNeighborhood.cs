using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AROrigami
{
    public class MooreNeighborhood
    {
        private Color[] contourImage = new Color[0];
        private int currentLength;

        private Point p = new Point(); //current boundary pixel
        private Point c = new Point(); //current pixel under consideration i.e.c is in M(p).
        private Point b = new Point(); //backtrack of c(i.e.neighbor pixel of p that was previously tested)

        private Color[] _images;
        private Color emptyColor = Color.white;
        private Color blockColor = Color.black;

        private int _width;
        private int _height;
        private float _threshold;

        private List<Vector2Int> _eightNeighbors;
        private int _neighbourCount;
        private MooreNeighborInfo infoContainer = new MooreNeighborInfo();

        public MooreNeighborInfo Execute(Color[] grayImages, int width, int height, Vector2Int startPoint, float threshold = 0.2f)
        {
            int maxStep = 5000;
            int step = 0;

            int area = 1;
            Vector2 centerPoint;
            contourImage = ResetOutputImage(width * height);
            _images = grayImages;
            _width = width;
            _height = height;
            _threshold = threshold;
            _eightNeighbors = MaskUtility.EightNeighborList;
            _neighbourCount = _eightNeighbors.Count;

            //0, height / 2
            Point firstContact = SearchForFirstContact(p, startPoint.x, startPoint.y );
            centerPoint = firstContact.position;

            p = firstContact;
            DrawDotOnContour(p.GetIndex(_width));

            c = GetMooreNeighborhood(p, p.backTracePoint);

            //While c not equal to s do
            while (!IsTerminatePoint(c, firstContact) && step <= maxStep)
            {
                //If c is wall
                if (c.value == 0)
                {
                    //insert c in B
                    DrawDotOnContour(c.GetIndex(_width));

                    //set p=c
                    p = c;

                    //backtrack (move the current pixel c to the pixel from which p was entered)
                    c = GetMooreNeighborhood(p, p.backTracePoint);

                    area++;
                    centerPoint += p.position;
                }
                //advance the current pixel c to the next clockwise pixel in M(p)
                else
                {
                    c = GetMooreNeighborhood(p, c.position, c.neighborIndex);
                }

                //Something went wrong
                if (c.neighborIndex == -1)
                    break;

                step++;
            }

            float divident = (1f / area);

            infoContainer.area = area;
            infoContainer.centerPoint.Set(centerPoint.x * divident, centerPoint.y * divident);
            infoContainer.img = contourImage;

            return infoContainer;
        }

        private Point SearchForFirstContact(Point point, int startX, int startY)
        {
            point.position = ParameterFlag.General.Vector2Zero;
            point.backTracePoint = ParameterFlag.General.Vector2Zero;

            for (int y = startY - 1; y >= 0; y--)
            {
                for (int x = startX; x < _width; x++)
                {
                    int index = x + (y * _width);

                    point.position.Set(x, y);

                    //Is Wall detected
                    if (_images[index].r >= _threshold)
                    {
                        point.value = 0;
                        return point;
                    }

                    point.backTracePoint.Set(x, y);
                }
            }

            return point;
        }

        private Point GetMooreNeighborhood(Point boundaryPixel, Vector2 backtracePoint, int neighbourIndex = -1)
        {
            Vector2 direction = backtracePoint - boundaryPixel.position;

            int currentNeighborIndex = neighbourIndex;
            if (currentNeighborIndex < 0)
                currentNeighborIndex = _eightNeighbors.FindIndex(x => x == direction);

            boundaryPixel.neighborIndex = currentNeighborIndex;

            if (currentNeighborIndex >= 0)
            {
                int nextNIndex = currentNeighborIndex;

                boundaryPixel.backTracePoint = boundaryPixel.position + _eightNeighbors[currentNeighborIndex];

                bool isValidPosition = false;
                while (!isValidPosition)
                {
                    nextNIndex = (nextNIndex + 1) % _neighbourCount;

                    boundaryPixel.position = boundaryPixel.position + _eightNeighbors[nextNIndex];

                    isValidPosition = boundaryPixel.position.x >= 0 && boundaryPixel.position.y >= 0 &&
                                        boundaryPixel.position.x < _width && boundaryPixel.position.y < _height;
                }

                int mapIndex = (int)((boundaryPixel.position.y * _width) + boundaryPixel.position.x); // 1

                boundaryPixel.value = _images[mapIndex].r >= _threshold ? 0 : 1;

                boundaryPixel.neighborIndex = nextNIndex;
            }

            return boundaryPixel;
        }

        private void DrawDotOnContour(int index)
        {
            contourImage[index] = blockColor;
        }

        private Color[] ResetOutputImage(int length)
        {
            if (currentLength != length)
                contourImage = new Color[length];

            for (int i = 0; i < length; i++)
            {
                //Set As white
                contourImage[i] = emptyColor;
            }
            return contourImage;
        }

        private bool IsTerminatePoint(Point point, Point startPoint)
        {
            return (point.position == startPoint.position && point.backTracePoint == startPoint.backTracePoint);
        }

        private struct Point
        {
            public Vector2Int position;
            public int value;
            public Vector2Int backTracePoint;

            public int neighborIndex;

            public int GetIndex(int width)
            {
                return Mathf.RoundToInt((position.y * width) + position.x);
            }
        }

        public struct MooreNeighborInfo
        {
            public Color[] img;
            public Vector2 centerPoint;
            public float area;
        }
    }
}