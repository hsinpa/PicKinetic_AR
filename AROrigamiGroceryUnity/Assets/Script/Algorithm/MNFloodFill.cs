using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MNFloodFill
{

    int _width, _height;
    List<Vector2Int> _NeighborList = MaskUtility.FourNeighborList;

    public Color[] DrawMask(Color[] contourImage, int width, int height) {
        _width = width;
        _height = height;

        contourImage = FloodFill(contourImage, width, height);

        return contourImage;
    }


    private Color[] FloodFill(Color[] images, int width, int height) {
        Color[] outputImage = GetNewImage(width * height);
        Color white = Color.white;

        List<Vector2Int> opened = new List<Vector2Int>();
        List<Vector2Int> closed = new List<Vector2Int>();
        Vector2Int point = new Vector2Int(0, 0);

        //Four Corner
        //Top Left
        opened.Add(new Vector2Int(0, 0));

        //Bottom Right
        opened.Add(new Vector2Int(width - 1, height - 1));

        //Top Right
        opened.Add(new Vector2Int(width - 1, 0));

        //Bottom Left
        opened.Add(new Vector2Int(0, height - 1));

        while (opened.Count > 0) {
            var nextPoint = opened[0];
            opened.RemoveAt(0);

            int imageIndex = nextPoint.x + (nextPoint.y * width);
            Color contourValue = images[imageIndex];

            //Is Not Wall
            if (contourValue.r >= 0.9f) {
                outputImage[imageIndex] = white;
            } else
            {
                continue;
            }

            var possiblePoint = FilterOpenPoint(nextPoint, closed);
            opened.AddRange(possiblePoint);
            closed.AddRange(possiblePoint);
        }

        return outputImage;
    }

    private List<Vector2Int> FilterOpenPoint(Vector2Int point, List<Vector2Int> closedList) {
        int possibleDirCount = _NeighborList.Count;
        List<Vector2Int> possiblePoints = new List<Vector2Int>();

        for (int i = 0; i < possibleDirCount; i++) {
            var newPoint = _NeighborList[i] + point;

            if (newPoint.x >= 0 && newPoint.x < _width && newPoint.y >= 0 && newPoint.y < _height && !closedList.Contains(newPoint)) {
                possiblePoints.Add(newPoint);
            }
        }

        return possiblePoints;
    }

    private Color[] GetNewImage(int length) {
        Color c = new Color(0,0,0, 1);
        Color[] newImage = new Color[length];
        for (int i = 0; i < length; i++)
            newImage[i] = c;

        return newImage;
    }
}
