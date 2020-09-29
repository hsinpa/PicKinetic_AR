using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
namespace AROrigami
{

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

        public static readonly List<Vector2> RaycastStartPos = new List<Vector2> {
            new Vector2(0, 0.5f), // Left
            new Vector2(), // Right
            new Vector2(), // Top
            new Vector2() // Bottom 
        };

        public ImageMaskGeneator(int maskSize)
        {
            MooreNeighborhood = new MooreNeighborhood();
            MNFloodFill = new MNFloodFill();
        }

        public async UniTask<MooreNeighborhood.MooreNeighborInfo> AsyncCreateMask(Color[] scaledImage, int width, int height)
        {
            return await Task.Run(() =>
            {
                MooreNeighborhood.MooreNeighborInfo TestContour = MooreNeighborhood.Execute(scaledImage, width, height, new Vector2Int(0, height / 2));
                TestContour.img = MNFloodFill.Execute(TestContour.img, width, height);
                return TestContour;
            });
        }

        public async UniTask<MooreNeighborhood.MooreNeighborInfo> AsyncCreateBorder(Color[] scaledImage, int width, int height)
        {
            return await Task.Run(() =>
            {
                return MooreNeighborhood.Execute(scaledImage, width, height, new Vector2Int(0, height / 2));
            });
        }

        private MooreNeighborhood[] PrpareMooreNeighborProcess(int p_process) {
            MooreNeighborhood[] processArray = new MooreNeighborhood[p_process];

            for (int i = 0; i < p_process; i++) {
                processArray[i] = new MooreNeighborhood();
            }

            return processArray;
        }
    }
}