using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MarchingCubeBorder
{

    private List<Vector3> Sort(HashSet<Vector3> hashVertices) {
        List<Vector3> sorted = new List<Vector3>();
        List<Vector3> opended = hashVertices.ToList();

        int sortedCount = 0;
        int opendCount = opended.Count;
        int step = 0;
        int maxStep = 10000;
        float limitDist = 1.1f;

        if (opendCount <= 0)
            return sorted;


        sorted.Add(opended[0]);
        opended.RemoveAt(0);
        sortedCount = 1;
        opendCount -= 1;

        bool isDone = false;
        while (!isDone)
        {
            step++;

            for (int i = 0; i < opendCount; i++)
            {
                float topDist = Vector3.Distance(opended[i], sorted[sortedCount - 1]);
                float botDist = Vector3.Distance(opended[i], sorted[0]);

                if (step > 9900) {
                    Debug.Log("topDist " + topDist + ", botDist " + botDist);
                }

                if (topDist <= limitDist)
                {
                    sorted.Add(opended[i]);
                    opended.RemoveAt(i);
                    sortedCount++;
                    opendCount--;
                    break;
                }

                if (botDist <= limitDist)
                {
                    sorted.Insert(0, opended[i]);
                    opended.RemoveAt(i);
                    sortedCount++;
                    opendCount--;
                    break;
                }
            }

            isDone = step >= maxStep || opendCount <= 0;
        }

        Debug.Log("sortedCount " + sortedCount + ", opendCount "+ opendCount + " step " + step);

        return sorted;
    }

    public async Task<Vector3[]> AsynSort(HashSet<Vector3> borderVertices) {
        return await Task.Run(() =>
        {
            return Sort(borderVertices).ToArray();
        });
    }

    public bool CheckIsBorder(MeshGenerator.Square[,] sqaures, int height, int width, int index_x, int index_y) {
        bool isBorder = false;
            
        //if (!isBorder && index_x < width)


        return isBorder;
    }

    public void Reset() {
    }

}
