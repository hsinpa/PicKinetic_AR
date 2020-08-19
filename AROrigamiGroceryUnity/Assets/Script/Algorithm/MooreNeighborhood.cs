using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private int _neighbourCount;


    public int[] Execute(int[] grayImages, int width, int height, float threshold = 0.1f)
    {
        contourImage = ResetOutputImage(width * height);
        _images = grayImages;
        _width = width;
        _height = height;
        _threshold = threshold;
        _eightNeighbors = SetEightNeighbor();
        _neighbourCount = _eightNeighbors.Count;

        Point startPoint = SearchForFirstContact(0, 0);

        p = startPoint;

        c = GetMooreNeighborhood(p, p.backTracePoint);
        //While c not equal to s do
        while (!IsTerminatePoint(c, startPoint)) {
            //If c is black
            if (c.value <= _threshold)
            {
                //insert c in B
                contourImage[c.GetIndex(_width)] = 0;

                //set p=c
                p = c;

                //backtrack (move the current pixel c to the pixel from which p was entered)
                c = GetMooreNeighborhood(p, p.backTracePoint);

            }
            //advance the current pixel c to the next clockwise pixel in M(p)
            else
            {
                c = GetMooreNeighborhood(p, c.position);
            }

            //Something went wrong
            if (c.neighborIndex == -1)
                break;
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

    private Point GetMooreNeighborhood(Point boundaryPixel, Vector2 backtracePoint, int neighbourIndex = -1) {
        Vector2 direction = backtracePoint - boundaryPixel.position;

        int currentNeighborIndex = neighbourIndex;
        if (currentNeighborIndex < 0)
            currentNeighborIndex = _eightNeighbors.FindIndex(x => x == direction);

        boundaryPixel.neighborIndex = currentNeighborIndex;

        if (currentNeighborIndex >= 0)
        {
            int nextNIndex = (currentNeighborIndex + 1) % _neighbourCount;

            boundaryPixel.backTracePoint = boundaryPixel.position + _eightNeighbors[currentNeighborIndex];

            boundaryPixel.position = boundaryPixel.position + _eightNeighbors[nextNIndex];

            int mapIndex = (int)((boundaryPixel.position.y * _width) + boundaryPixel.position.x);

            boundaryPixel.value = _images[mapIndex];

            boundaryPixel.neighborIndex = nextNIndex;
        }


        return boundaryPixel;
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
            //Set As white
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

        public int neighborIndex;

        public int GetIndex(int width) {
            return (int) ((position.y * width) + position.x);
        }

    }



}
