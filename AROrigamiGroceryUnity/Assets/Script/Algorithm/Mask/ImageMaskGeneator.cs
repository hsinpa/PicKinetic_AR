using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ImageMaskGeneator
{
    private MooreNeighborhood MooreNeighborhood;
    private MNFloodFill MNFloodFill;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    MooreNeighborhood = new MooreNeighborhood();
    //    MNFloodFill = new MNFloodFill();

    //    DrawContour();
    //}

    public ImageMaskGeneator(int maskSize) {
        MooreNeighborhood = new MooreNeighborhood();
        MNFloodFill = new MNFloodFill();
    }

    public async Task<Color[]> AsyncCreateMask(Color[] scaledImage, int width, int height)
    {
        return await Task.Run(() =>
        {
            //return MooreNeighborhood.Execute(scaledImage, width, height);
            Color[] TestContour = MooreNeighborhood.Execute(scaledImage, width, height);
            return MNFloodFill.Execute(TestContour, width, height);
        });
    }


    //private async void DrawContour() {
    //    int width = 10, heigth = 10;

    //    int[] TestGrayImage = PrepareFakeContour();

    //    var maskImage = await AsyncCreateMask(TestGrayImage, width, heigth);

    //    LogContour(maskImage, width, heigth);
    //}

    //private async Task<Color[]> AsyncCreateMask(int[] fakeInt, int width, int height) {

    //    return await Task.Run(() =>
    //    {
    //        Color[] intToColor = MaskUtility.ColorFromInt(fakeInt);
    //        Color[] TestContour = MooreNeighborhood.Execute(intToColor, width, height);

    //        return MNFloodFill.DrawMask(TestContour,, width, height);
    //    });
    //}



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
