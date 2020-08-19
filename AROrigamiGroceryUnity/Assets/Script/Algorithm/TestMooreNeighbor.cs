using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMooreNeighbor : MonoBehaviour
{
    private MooreNeighborhood MooreNeighborhood;

    // Start is called before the first frame update
    void Start()
    {
        MooreNeighborhood = new MooreNeighborhood();


    }


    private int[] PrepareFakeContour() {
        return new int[] {
            1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1,
            1, 1, 1, 0, 0, 1, 1, 1 ,1 ,1,
            1, 1, 1, 0, 0, 0, 0, 1 ,1 ,1,
            1, 1, 1, 1, 0, 0, 0, 1 ,1 ,1,
            1, 1, 1, 1, 0, 0, 1, 1 ,1 ,1,
            1, 1, 1, 1, 1, 0, 0, 0 ,1 ,1,
            1, 1, 1, 1, 1, 0, 0, 1 ,1 ,1,
            1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1,
            1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1,
            1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1
        };
    }
}
