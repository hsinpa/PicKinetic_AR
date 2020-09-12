﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

namespace AROrigami
{

    public class TextureMeshManager : MonoBehaviour
    {
        #region Inspector 
        [SerializeField]
        private Texture2D rawColorTexture;

        [SerializeField]
        private Texture2D edgeOutputTex;

        [SerializeField]
        private Texture2D highlightTexture;
        private RenderTexture highlightRenderer;

        [SerializeField]
        private Material EdgeMaterial;

        [SerializeField]
        ComputeShader textureComputeShader;
        #endregion

        #region Private Parameters
        ImageMaskGeneator imageMaskGeneator;
        MeshGenerator meshGenerator;
        MarchingCube marchingCube;
        private Mesh2DTo3D mesh2DTo3D;

        EdgeImageDetector edgeImage;
        MarchingCubeBorder marchingCubeBorder;
        public System.Action<Texture2D> OnEdgeTexUpdate;

        private Vector3[] TestBorderArray;

        int resize = 64;
        int startPixelX;
        int startPixelY;

        //ComputeShader Properties
        private Color[] process_tex_colors;
        private Color[] process_tex_colors_cpu;

        private ComputeBuffer _colorBuffer;
        private int floatSize = sizeof(float);
        int GetColorKernelHandle;
        bool IsComputeShaderFree = true;

        Camera _camera;
        Vector3 _meshPosition = new Vector2();
        #endregion


        public void Start()
        {
            highlightRenderer = TextureUtility.GetRenderTexture(resize);
            edgeOutputTex = new Texture2D(resize, resize, TextureFormat.ARGB32, false);
            imageMaskGeneator = new ImageMaskGeneator(resize);
            marchingCube = new MarchingCube();
            meshGenerator = new MeshGenerator();
            marchingCubeBorder = new MarchingCubeBorder();
            mesh2DTo3D = new Mesh2DTo3D();
            edgeImage = new EdgeImageDetector(EdgeMaterial);
            _camera = Camera.main;

            process_tex_colors = new Color[resize * resize];
            process_tex_colors_cpu = new Color[resize * resize];
            PrepareComputeShader();

            Graphics.Blit(highlightTexture, highlightRenderer);
        }

        public void UpdateScreenInfo(int startPixelX, int startPixelY) {
            this.startPixelX = startPixelX;
            this.startPixelY = startPixelY;
        }

        public IEnumerator ExecEdgeProcessing(RenderTexture processTex)
        {
            if (IsComputeShaderFree)
            {
                IsComputeShaderFree = false;

                var edgeTex = edgeImage.GetEdgeTex(processTex);

                yield return new WaitForEndOfFrame();

                AsyncGPUReadback.Request(edgeTex, 0, TextureFormat.ARGB32, OnTexCompleteReadback);
            }
        }

        #region ComputeShader API
        public void ProcessCSTextureColor()
        {
            textureComputeShader.Dispatch(GetColorKernelHandle, resize / 16, resize / 16, 1);

            _colorBuffer.GetData(process_tex_colors_cpu);
        }

        private void PrepareComputeShader() {
            _colorBuffer = ComputeShaderUtility.SetComputeBuffer(process_tex_colors, resize * resize, floatSize * 4, ComputeBufferType.Default);

            GetColorKernelHandle = textureComputeShader.FindKernel("GetColors");

            textureComputeShader.SetInt("TexWidth", resize);

            textureComputeShader.SetTexture(GetColorKernelHandle, "MainTex", edgeOutputTex);

            textureComputeShader.SetBuffer(GetColorKernelHandle, "ColorBuffer", _colorBuffer);
        }

        private void OnTexCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                Debug.Log("GPU readback error detected.");
                return;
            }

            if (edgeOutputTex == null) return;

            edgeOutputTex.LoadRawTextureData(request.GetData<uint>());
            edgeOutputTex.Apply();

            IsComputeShaderFree = true;
        }
        #endregion

        #region Mask API 
        public async void CaptureEdgeBorderMesh(int skinSize, MeshObject meshObject)
        {

            if (OnEdgeTexUpdate != null)
                OnEdgeTexUpdate(edgeOutputTex);

            var maskColors = await imageMaskGeneator.AsyncCreateBorder(process_tex_colors_cpu, resize, resize);

            if (!CheckIfValid(maskColors)) return;

            var meshResult = await AssignMesh(maskColors.img, resize, resize);
            var meshData = mesh2DTo3D.CreateMeshData(meshResult.vertices, meshResult.triangles, meshResult.uv, null);

            meshObject.SetMesh(mesh2DTo3D.CreateMesh(meshObject.mesh, meshData), highlightRenderer, skinSize);

            AssignPosition(maskColors, meshObject);
        }

        public async void CaptureContourMesh(RenderTexture processTexture, RenderTexture skinTexture, MeshObject meshObject)
        {
            var maskColors = await imageMaskGeneator.AsyncCreateMask(process_tex_colors_cpu, resize, resize);

            if (!CheckIfValid(maskColors)) return;

            var meshResult = await AssignMesh(maskColors.img, resize, resize);

            var mesh = await MeshTo3D(meshResult, meshObject.mesh);

            if (mesh != null)
                meshObject.SetMesh(mesh, skinTexture, skinTexture.width);

            AssignPosition(maskColors, meshObject);
        }
        #endregion

        private async UniTask<MarchingCube.MarchingCubeResult> AssignMesh(Color[] maskImage, int textureWidth, int textureHeight)
        {
            return await UniTask.Run(() =>
            {
                meshGenerator.GenerateMesh(maskImage, textureWidth, textureHeight, 1);
                return marchingCube.Calculate(meshGenerator.squareGrid);
            });
        }

        private async UniTask<Mesh> MeshTo3D(MarchingCube.MarchingCubeResult meshResult, Mesh mesh) {
            Vector3[] borderVertices = await marchingCubeBorder.AsynSort(meshResult.borderVertices);
            //TestBorderArray = borderVertices;
            Mesh2DTo3D.MeshData meshData = await mesh2DTo3D.Convert(meshResult, borderVertices);

            return mesh2DTo3D.CreateMesh(mesh, meshData);
        }

        private void AssignPosition(MooreNeighborhood.MooreNeighborInfo meshInfo, MeshObject meshObject) {

            float x = (meshInfo.centerPoint.x * 4) + startPixelX;
            float y = (meshInfo.centerPoint.y * 4) + startPixelY;

            _meshPosition.Set(x, y, _camera.nearClipPlane);
            var point = _camera.ScreenToWorldPoint(_meshPosition);
            point.z = 0;

            if (meshObject != null)
                meshObject.transform.position = point;
        }

        private bool CheckIfValid(MooreNeighborhood.MooreNeighborInfo meshInfo) {
            return (meshInfo.area > 40);
        }

        //private void OnDrawGizmosSelected()
        //{
        //    if (TestBorderArray != null) {
        //        int count = TestBorderArray.Length;
        //        for (int i = 0; i < count; i++) {
        //            float pert = (float)i / count;
        //            Gizmos.color = new Color(pert, pert, pert);
        //            Gizmos.DrawSphere(TestBorderArray[i] * 0.12f, 0.04f);
        //        }
        //    }
        //}

        private void OnApplicationQuit()
        {
            ResetData();
        }

        private void ResetData()
        {
            _colorBuffer.Dispose();
        }

    }
}