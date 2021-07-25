using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;

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

        public IEnumerator ExecEdgeProcessing(RenderTexture processTex)
        {
            if (IsComputeShaderFree)
            {
                IsComputeShaderFree = false;
                var edgeTex = edgeDetector.GetEdgeTex(processTex);
                yield return new WaitForEndOfFrame();

#if !UNITY_EDITOR && UNITY_ANDROID

                RenderTexture.active = edgeTex;
                _edgeLineTex.ReadPixels(androidOnlyEdgeRect, 0, 0);
                _edgeLineTex.Apply();

                yield return new WaitForEndOfFrame();
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
        public async UniTask<MeshLocData> CaptureEdgeBorderMesh(int skinSize, MeshObject meshObject, TextureUtility.TextureStructure textureStructure)
        {
            var maskColors = await imageMaskGeneator.AsyncCreateBorder(process_tex_colors_cpu);

            if (!CheckIfValid(maskColors)) return default(MeshLocData);

            var marchCubeResult = await ProcessMarchCube(maskColors.img, resize, resize);
            var meshData = mesh2DTo3D.CreateMeshData(marchCubeResult.vertices, marchCubeResult.triangles, marchCubeResult.uv, null);

            meshObject.SetMesh(mesh2DTo3D.CreateMesh(meshObject.mesh, meshData), highlightRenderer, skinSize);

            return GetMeshLocData(maskColors, meshObject, textureStructure);             
        }

        public async UniTask<MeshLocData> CaptureContourMesh(RenderTexture skinTexture, MeshObject meshObject, TextureUtility.TextureStructure textureStructure)
        {
            var maskColors = await imageMaskGeneator.AsyncCreateMask(process_tex_colors_cpu);

            if (!CheckIfValid(maskColors)) return default(MeshLocData);

            var marchCubeResult = await ProcessMarchCube(maskColors.img, resize, resize);

            var mesh = await MeshTo3D(marchCubeResult, meshObject.mesh);

            if (mesh.Item1 != null) {
                meshObject.SetMesh(mesh.Item1, skinTexture, skinTexture.width);
                meshObject.SetControlPoint(mesh.Item2.topVertice, mesh.Item2.bottomVertice);
                meshObject.GenerateControlPoints();
            }

            return GetMeshLocData(maskColors, meshObject, textureStructure);
        }
#endregion

        private async UniTask<MarchingCube.MarchingCubeResult> ProcessMarchCube(Color[] maskImage, int textureWidth, int textureHeight)
        {
            return await UniTask.Run(() =>
            {
                meshBuilder.GenerateMesh(maskImage, textureWidth, textureHeight, 1);
                return marchingCube.Calculate(meshBuilder.squareGrid);
            });
        }

        private async UniTask<(Mesh, Mesh2DTo3D.MeshData)> MeshTo3D(MarchingCube.MarchingCubeResult meshResult, Mesh mesh) {
            Vector3[] borderVertices = await marchingCubeBorder.AsynSort(meshResult.borderVertices);
            //TestBorderArray = borderVertices;
            Mesh2DTo3D.MeshData meshData = await mesh2DTo3D.Convert(meshResult, borderVertices);

            return (mesh2DTo3D.CreateMesh(mesh, meshData), meshData);
        }

        private MeshLocData GetMeshLocData(MooreNeighborhood.MooreNeighborInfo meshInfo, MeshObject meshObject, TextureUtility.TextureStructure textureStructure) {
            //float _x = ((meshInfo.centerPoint.x / resize) * textureStructure.xRatio) + (textureStructure.xResidualRatio * 0.5f);
            //float _y = ((meshInfo.centerPoint.y / resize) * textureStructure.yRatio) + (textureStructure.yResidualRatio * 0.5f);

            float x = (meshInfo.centerPoint.x / resize);
            float y = (meshInfo.centerPoint.y / resize);

            //Debug.Log(string.Format("Original X {0}, Y {1}", x, y));

            _meshPosition.Set(x, y);

            meshCalResult.screenPoint = _meshPosition;
            meshCalResult.meshObject = meshObject;

            return meshCalResult;
        }

        //Prevent small area image count as valid
        private bool CheckIfValid(MooreNeighborhood.MooreNeighborInfo meshInfo) {
            return (meshInfo.area > 40);
        }

        public struct MeshLocData {
            public MeshObject meshObject;
            public Vector2 screenPoint;

            public bool isValid => this.meshObject != null;
        }

        private void ResetData()
        {
            _colorBuffer.Dispose();
        }

    }
}