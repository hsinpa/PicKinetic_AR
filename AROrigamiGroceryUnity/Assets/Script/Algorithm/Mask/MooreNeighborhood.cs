using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MooreNeighborhood
{

    private Color[] contourImage;

    private Point p; //current boundary pixel
    private Point c; //current pixel under consideration i.e.c is in M(p).
    private Point b; //backtrack of c(i.e.neighbor pixel of p that was previously tested)

    private Color[] _images;
    private Color emptyColor = Color.white;
    private Color blockColor = Color.black;

    private int _width;
    private int _height;
    private float _threshold;

    private List<Vector2Int> _eightNeighbors;
    private int _neighbourCount;
    private MooreNeighborInfo infoContainer = new MooreNeighborInfo();

    public MooreNeighborInfo Execute(Color[] grayImages, int width, int height, float threshold = 0.2f)
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

        Point startPoint = SearchForFirstContact(0, height/2);
        centerPoint = startPoint.position;

        p = startPoint;
        DrawDotOnContour(p.GetIndex(_width));

        c = GetMooreNeighborhood(p, p.backTracePoint);

        //While c not equal to s do
        while (!IsTerminatePoint(c, startPoint) && step <= maxStep) {
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

    private Point SearchForFirstContact(int startX, int startY) {

        Point currentPoint = new Point();
        currentPoint.position = new Vector2Int(0, 0);
        currentPoint.backTracePoint = new Vector2Int(0, 0);

        for (int y = startY - 1; y >= 0; y--) {
            for (int x = startX; x < _width; x++) {
                int index = x + (y * _width);

                currentPoint.position.Set(x, y);

                //Is Wall detected
                if (_images[index].r >= _threshold) {
                    currentPoint.value = 0;
                    return currentPoint;
                }

                currentPoint.backTracePoint.Set(x, y);
            }
        }

        return currentPoint;
    }

    private Point GetMooreNeighborhood(Point boundaryPixel, Vector2 backtracePoint, int neighbourIndex = -1) {
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
            while (!isValidPosition) {
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

    private void DrawDotOnContour(int index) {
        contourImage[index] = blockColor;
    }

    private Color[] ResetOutputImage(int length) {
        Color[] output = new Color[length];
        for (int i =0; i < length; i++)
        {
            //Set As white
            output[i] = emptyColor;
        }
        return output;
    }

    private Color[] ToColor(int[] binaryArray)
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

    private bool IsTerminatePoint(Point point, Point startPoint)
    {
        return (point.position == startPoint.position && point.backTracePoint == startPoint.backTracePoint);
    }

    private struct Point {
        public Vector2Int position;
        public int value;
        public Vector2Int backTracePoint;

        public int neighborIndex;

        public int GetIndex(int width) {
            return Mathf.RoundToInt((position.y * width) + position.x);
        }
    }

    public struct MooreNeighborInfo {
        public Color[] img;
        public Vector2 centerPoint;
        public float area;
    }
}