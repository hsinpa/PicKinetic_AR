using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AROrigami
{
    public class ComputeShaderUtility
    {
        public static ComputeBuffer SetComputeBuffer<T>(T[] emptyArray, int length, int byteSize, ComputeBufferType bufferType = ComputeBufferType.Default)
        {
            ComputeBuffer computeBuffer = new ComputeBuffer(length, byteSize, bufferType);
            computeBuffer.SetData(emptyArray);
            return computeBuffer;
        }
    }
}