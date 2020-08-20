using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMooreNeighbor : MonoBehaviour
{
    private MooreNeighborhood MooreNeighborhood;
    private MNFloodFill MNFloodFill;

    // Start is called before the first frame update
    void Start()
    {
        MooreNeighborhood = new MooreNeighborhood();
        MNFloodFill = new MNFloodFill();
        int width = 10, heigth = 10;

        int[] TestGrayImage = PrepareFakeContour();

        Color[] TestContour = MooreNeighborhood.Execute(TestGrayImage, width, heigth);
        TestContour = MNFloodFill.DrawMask(TestContour, width, heigth);

        LogContour(TestContour, width, heigth);
    }

    private void LogContour(Color[] p_contour, int width, int height) {

        string contour = "";

        for (int h = 0; h < height; h++) {
            for (int w = 0; w < width; w++) {
                int index = w + (h * width);

                contour += p_contour[index].r.ToString();

                if (w == width - 1) {
                    contour += "\n";
                }
            }
        }

        Debug.Log(contour);
    }

    private int[] PrepareFakeContour() {
        return new int[] {
            1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1,
            1, 0, 0, 0, 0, 1, 1, 1 ,1 ,1,
            1, 0, 0, 0, 0, 0, 0, 1 ,1 ,1,
            1, 0, 0, 0, 0, 0, 0, 1 ,1 ,1,
            1, 1, 0, 0, 0, 0, 0, 0 ,1 ,1,
            1, 1, 0, 1, 0, 0, 0, 0 ,1 ,1,
            1, 1, 1, 1, 0, 0, 0, 1 ,1 ,1,
            1, 1, 1, 1, 1, 0, 1, 1 ,1 ,1,
            1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1,
            1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1
        };
    }
}
