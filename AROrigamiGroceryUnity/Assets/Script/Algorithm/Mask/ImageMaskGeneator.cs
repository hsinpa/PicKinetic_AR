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

        private UniTask<MooreNeighborhood.MooreNeighborInfo>[] taskArray;

        public static readonly List<Vector2> RaycastStartPos = new List<Vector2> {
            new Vector2(0, 0.51f), // Left
            new Vector2(1, 0.49f), // Right
            new Vector2(0.5f, 1), // Top
            new Vector2(0.5f, 0) // Bottom 
        };

        private Vector2Int _indexVector = new Vector2Int();
        private Color[] _scaledImage;
        private int _width, _height;

        public ImageMaskGeneator()
        {
            MooreNeighborhood = new MooreNeighborhood();
            MNFloodFill = new MNFloodFill();

            taskArray = new UniTask<MooreNeighborhood.MooreNeighborInfo>[RaycastStartPos.Count];
            for (int i = 0; i < RaycastStartPos.Count; i++) {
                taskArray[i] = AsyncFindBestContour(RaycastStartPos[i]);
            }
        }

        public async UniTask<MooreNeighborhood.MooreNeighborInfo> AsyncCreateMask(Color[] scaledImage, int width, int height)
        {
            this._scaledImage = scaledImage;
            this._width = width;
            this._height = height;

            return await UniTask.Run(() =>
            {
                MooreNeighborhood.MooreNeighborInfo TestContour = MooreNeighborhood.Execute(scaledImage, width, height, new Vector2Int(0, height / 2));
                TestContour.img = MNFloodFill.Execute(TestContour.img, width, height);
                return TestContour;
            });
        }

        public async UniTask<MooreNeighborhood.MooreNeighborInfo> AsyncCreateBorder(Color[] scaledImage, int width, int height)
        {
            return await UniTask.Run(() =>
            {
                return MooreNeighborhood.Execute(scaledImage, width, height, new Vector2Int(0, height / 2));
            });
        }

        private async UniTask<MooreNeighborhood.MooreNeighborInfo> AsyncFindBestContour(Vector2 direction) {
            return await UniTask.Run(() =>
            {
                _indexVector.Set(
                    Mathf.FloorToInt(this._width * direction.x),
                    Mathf.FloorToInt(this._height * direction.y)
                ); ;

                return MooreNeighborhood.Execute(this._scaledImage, this._width, this._height, _indexVector);
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