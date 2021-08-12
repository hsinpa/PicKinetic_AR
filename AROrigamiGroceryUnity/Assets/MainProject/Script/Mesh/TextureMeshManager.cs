using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace PicKinetic
{
    public class TextureMeshManager : MonoBehaviour
    {
        #region Inspector
        [Header("Editor Feature")]
        [SerializeField]
        private Texture2D rawColorTexture;

        [SerializeField]
        private Texture2D _edgeLineTex;
        public Texture2D edgeLineTex => this._edgeLineTex;

        [SerializeField]
        private Texture2D highlightTexture;
        private RenderTexture highlightRenderer;

        [Header("Reference Asset")]
        [SerializeField]
        private Material EdgeMaterial;

        [SerializeField]
        ComputeShader textureComputeShader;
        #endregion

        #region Private Parameters
        Rect androidOnlyEdgeRect;
        ImageMaskGeneator imageMaskGeneator;
        MeshBuilder meshBuilder;
        MarchingCube marchingCube;
        MeshLocData meshCalResult;

        private Mesh2DTo3D mesh2DTo3D;
        EdgeImageDetector edgeDetector;
        MarchingCubeBorder marchingCubeBorder;
        public System.Action<Texture2D> OnEdgeTexUpdate;

        int resize = 64;

        //ComputeShader Properties
        private Color[] process_tex_colors;
        private Color[] process_tex_colors_cpu;

        private ComputeBuffer _colorBuffer;
        private int floatSize = sizeof(float);
        int GetColorKernelHandle;
        bool IsComputeShaderFree = true;

        Vector2 _meshPosition = new Vector2();
        #endregion

        public void SetUp()
        {
            meshCalResult = new MeshLocData();
            highlightRenderer = TextureUtility.GetRenderTexture(resize);
            _edgeLineTex = new Texture2D(resize, resize, TextureFormat.RGB24, false);
            androidOnlyEdgeRect = new Rect(0, 0, resize, resize);
            imageMaskGeneator = new ImageMaskGeneator(resize);
            marchingCube = new MarchingCube();
            meshBuilder = new MeshBuilder();
            marchingCubeBorder = new MarchingCubeBorder();
            mesh2DTo3D = new Mesh2DTo3D();
            edgeDetector = new EdgeImageDetector(EdgeMaterial);

            process_tex_colors = new Color[resize * resize];
            process_tex_colors_cpu = new Color[resize * resize];
            PrepareComputeShader();

            Graphics.Blit(highlightTexture, highlightRenderer);
        }

        public void ExecEdgeProcessing(RenderTexture processTex)
        {
            if (IsComputeShaderFree)
            {
                IsComputeShaderFree = false;
                var edgeTex = edgeDetector.GetEdgeTex(processTex);

#if !UNITY_EDITOR && UNITY_ANDROID
                RenderTexture.active = edgeTex;
                _edgeLineTex.ReadPixels(androidOnlyEdgeRect, 0, 0);
                _edgeLineTex.Apply();

                IsComputeShaderFree = true;
#else
                AsyncGPUReadback.Request(edgeTex, 0, TextureFormat.RGB24, OnTexCompleteReadback);
#endif

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

            textureComputeShader.SetTexture(GetColorKernelHandle, "MainTex", _edgeLineTex);

            textureComputeShader.SetBuffer(GetColorKernelHandle, "ColorBuffer", _colorBuffer);
        }

        private void OnTexCompleteReadback(AsyncGPUReadbackRequest request)
        {
            if (request.hasError)
            {
                Debug.Log("GPU readback error detected.");
                return;
            }

            if (_edgeLineTex == null) return;

            _edgeLineTex.LoadRawTextureData(request.GetData<uint>());
            _edgeLineTex.Apply();

            IsComputeShaderFree = true;
        }
#endregion

#region Mask API 
        public async Task<MeshLocData> CaptureEdgeBorderMesh(int skinSize, MeshObject meshObject, TextureUtility.TextureStructure textureStructure)
        {
            ParallelMeshResult pEdgeMesh = await Task.Run(() =>
            {
                var maskColors = imageMaskGeneator.AsyncCreateBorder(process_tex_colors_cpu);

                var marchCubeResult = ProcessMarchCube(maskColors.img, resize, resize);

                return new ParallelMeshResult() {
                    maskColors = maskColors,
                    meshData = mesh2DTo3D.CreateMeshData(marchCubeResult.vertices, marchCubeResult.triangles, marchCubeResult.uv, null) 
                };
            });

            //Mesh is Unity object, so can't put inside thread
            meshObject.SetMesh(mesh2DTo3D.CreateMesh(meshObject.mesh, pEdgeMesh.meshData), highlightRenderer, skinSize);

            return GetMeshLocData(pEdgeMesh.maskColors, meshObject, textureStructure, MeshLocData.Type.Edge);
        }

        public async UniTask<MeshLocData> CaptureContourMesh(RenderTexture skinTexture, MeshObject meshObject, TextureUtility.TextureStructure textureStructure)
        {
            ParallelMeshResult pEdgeMesh = await Task.Run(() =>
            {
                var maskColors = imageMaskGeneator.AsyncCreateMask(process_tex_colors_cpu);

                var marchCubeResult = ProcessMarchCube(maskColors.img, resize, resize);

                Vector3[] borderVertices = marchingCubeBorder.Sort(marchCubeResult.borderVertices).ToArray();

                Mesh2DTo3D.MeshData meshData = mesh2DTo3D.Convert(marchCubeResult, borderVertices);
                //Mesh2DTo3D.MeshData meshData = mesh2DTo3D.CreateMeshData(marchCubeResult.vertices, marchCubeResult.triangles, marchCubeResult.uv, null);

                return new ParallelMeshResult()
                {
                    maskColors = maskColors,
                    meshData = meshData
                };
            });

            var mesh = mesh2DTo3D.CreateMesh(meshObject.mesh, pEdgeMesh.meshData);

            meshObject.SetMesh(mesh, skinTexture, skinTexture.width);
            meshObject.SetControlPoint(pEdgeMesh.meshData.topVertice, pEdgeMesh.meshData.bottomVertice);
            meshObject.GenerateControlPoints();
            
            return GetMeshLocData(pEdgeMesh.maskColors, meshObject, textureStructure, MeshLocData.Type.Contour);
        }
        #endregion

        private MarchingCube.MarchingCubeResult ProcessMarchCube(Color[] maskImage, int textureWidth, int textureHeight)
        {
            meshBuilder.GenerateMesh(maskImage, textureWidth, textureHeight, 1);
            return marchingCube.Calculate(meshBuilder.squareGrid);
        }

        private MeshLocData GetMeshLocData(MooreNeighborhood.MooreNeighborInfo meshInfo, MeshObject meshObject, TextureUtility.TextureStructure textureStructure, MeshLocData.Type type) {
            //float _x = ((meshInfo.centerPoint.x / resize) * textureStructure.xRatio) + (textureStructure.xResidualRatio * 0.5f);
            //float _y = ((meshInfo.centerPoint.y / resize) * textureStructure.yRatio) + (textureStructure.yResidualRatio * 0.5f);

            float x = (meshInfo.centerPoint.x / resize);
            float y = (meshInfo.centerPoint.y / resize);

            //Debug.Log(string.Format("Original X {0}, Y {1}", x, y));

            _meshPosition.Set(x, y);

            meshCalResult.screenPoint = _meshPosition;
            meshCalResult.meshObject = meshObject;
            meshCalResult.type = type;

            return meshCalResult;
        }

        public struct MeshLocData {

            public enum Type {Edge, Contour}
            public Type type;
            public MeshObject meshObject;
            public Vector2 screenPoint;

            public bool isValid => this.meshObject != null;
        }

        private struct ParallelMeshResult
        {
            public MooreNeighborhood.MooreNeighborInfo maskColors;
            public Mesh2DTo3D.MeshData meshData;
        }

        private void ResetData()
        {
            _colorBuffer.Dispose();
        }

    }
}