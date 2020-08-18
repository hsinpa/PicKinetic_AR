using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MooreNeighborhood
{

    private int[] contourImage;

    private Point p; //current boundary pixel
    private Point c; //current pixel under consideration i.e.c is in M(p).
    private Point b; //backtrack of c(i.e.neighbor pixel of p that was previously tested)

    private int[] _images;
    private int _width;
    private int _height;
    private float _threshold;

    private List<Vector2> _eightNeighbors;


    public int[] Execute(int[] grayImages, int width, int height, float threshold = 0.1f)
    {
        contourImage = ResetOutputImage(width * height);
        _images = grayImages;
        _width = width;
        _height = height;
        _threshold = threshold;
        _eightNeighbors = SetEightNeighbor();

        Point startPoint = SearchForFirstContact(0, 0);

        p = startPoint;


        while (!IsTerminatePoint(c, startPoint)) {

            //advance the current pixel c to the next clockwise pixel in M(p)
        }

        return contourImage;
    }

    private Point SearchForFirstContact(int startX, int startY) {

        Point currentPoint = new Point();

        currentPoint.position = new Vector2(0, 0);
        currentPoint.backTracePoint = new Vector2(0, 0);

        for (int x = startX; x < _width; x++) {
            for (int y = startY; y < _height; y++) {
                int index = x + (y * _width);

                currentPoint.position.Set(x, y);

                //Is Wall detected
                if (_images[index] <= _threshold) {
                    currentPoint.value = 0;
                    return currentPoint;
                }

                currentPoint.backTracePoint.Set(x, y);
            }
        }

        return currentPoint;
    }

    private Point GetMooreNeighborhood(Point targetPoint, Vector2 backtracePoint) {
        Vector2 direction = backtracePoint - targetPoint.position;
        int neighborCount = _eightNeighbors.Count;

        int currentNeighborIndex = _eightNeighbors.FindIndex(x => x == direction);

        if (currentNeighborIndex >= 0)
        {
            int nextNIndex = (currentNeighborIndex + 1) % neighborCount;

            targetPoint.backTracePoint = targetPoint.position + _eightNeighbors[currentNeighborIndex];

            targetPoint.position = targetPoint.position + _eightNeighbors[nextNIndex];
        }

        return targetPoint;

    }

    private List<Vector2> SetEightNeighbor() {
        return new List<Vector2> {
            new Vector2(-1, 1), new Vector2(0, 1),new Vector2(1, 1),
            new Vector2(1, 0), new Vector2(1, -1),
            new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0)
        };
    }


    private int[] ResetOutputImage(int length) {
        int[] output = new int[length];
        for (int i =0; i < length; i++)
        {
            output[i] = 1;
        }
        return output;
    }

    private bool IsTerminatePoint(Point point, Point startPoint)
    {
        return (point.position == startPoint.position && point.backTracePoint == startPoint.backTracePoint);
    }

    private struct Point {
        public Vector2 position;
        public int value;
        public Vector2 backTracePoint;
    }



}
