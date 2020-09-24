using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

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

    public async UniTask<MooreNeighborhood.MooreNeighborInfo> AsyncCreateMask(Color[] scaledImage, int width, int height)
    {
        return await Task.Run(() =>
        {
            var TestContour = MooreNeighborhood.Execute(scaledImage, width, height);
            TestContour.img =  MNFloodFill.Execute(TestContour.img, width, height);
            return TestContour;
        });
    }

    public async UniTask<MooreNeighborhood.MooreNeighborInfo> AsyncCreateBorder(Color[] scaledImage, int width, int height)
    {
        return await Task.Run(() =>
        {
            return MooreNeighborhood.Execute(scaledImage, width, height);
        });
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
