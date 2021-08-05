using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
namespace PicKinetic
{

    public class ImageMaskGeneator
    {
        private MNFloodFill MNFloodFill;

        private MooreNeighborhood.MooreNeighborInfo[] taskArray;

        /// <summary>
        /// Start from 4 direction in parallel, save execute time
        /// </summary>
        public readonly List<(Vector2, LoopUtility.LoopDirection, MooreNeighborhood)> RaycastStartPos;
        private int taskCount;

        private Vector2Int _indexVector = new Vector2Int();
        private Color[] _scaledImage;
        private int _width, _height;

        public ImageMaskGeneator(int size)
        {
            _width = size;
            _height = size;
            //MooreNeighborhood = new MooreNeighborhood();
            MNFloodFill = new MNFloodFill();

            RaycastStartPos = new List<(Vector2, LoopUtility.LoopDirection, MooreNeighborhood)> {
                (new Vector2(0, 0.5f), LoopUtility.LoopDirection.Left, new MooreNeighborhood(size, size)), // Left
                (new Vector2(1, 0.5f), LoopUtility.LoopDirection.Right, new MooreNeighborhood(size, size)), // Right
                (new Vector2(0.5f, 1), LoopUtility.LoopDirection.Top, new MooreNeighborhood(size, size)), // Top
                (new Vector2(0.5f, 0), LoopUtility.LoopDirection.Down, new MooreNeighborhood(size, size)), // Bottom , 
            };

            taskCount = RaycastStartPos.Count;

            taskArray = new MooreNeighborhood.MooreNeighborInfo[RaycastStartPos.Count];
        }

        public MooreNeighborhood.MooreNeighborInfo AsyncCreateMask(Color[] scaledImage)
        {
            this._scaledImage = scaledImage;

            var mooreNeighbor = PrpareMooreNeighborProcess();

            mooreNeighbor.img = MNFloodFill.Execute(mooreNeighbor.img, _width, _height);
            return mooreNeighbor;
        }

        public MooreNeighborhood.MooreNeighborInfo AsyncCreateBorder(Color[] scaledImage)
        {
            this._scaledImage = scaledImage;

            return PrpareMooreNeighborProcess();
        }

        private MooreNeighborhood.MooreNeighborInfo AsyncContourDirection(Vector2 startVector, LoopUtility.LoopDirection direction, MooreNeighborhood mooreNeighborhood) {
            _indexVector.Set(
                Mathf.FloorToInt(this._width * startVector.x),
                Mathf.FloorToInt(this._height * startVector.y)
            );

            return mooreNeighborhood.Execute(this._scaledImage, this._width, this._height, _indexVector, direction);
        }

        private MooreNeighborhood.MooreNeighborInfo PrpareMooreNeighborProcess() {

            for (int i = 0; i < RaycastStartPos.Count; i++)
            {
                taskArray[i] = AsyncContourDirection(RaycastStartPos[i].Item1, RaycastStartPos[i].Item2, RaycastStartPos[i].Item3);
            }

            //var taskResultArray = await UniTask.WhenAll(taskArray);
            float largestArea = 0;
            int largestIndex = 0;

            for (int i = 0; i < taskCount; i++) {

                if (taskArray[i].area > largestArea) {

                    largestArea = taskArray[i].area;
                    largestIndex = i;
                }
            }

            return taskArray[largestIndex];
        }
    }
}