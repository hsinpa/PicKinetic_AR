using System.Collections;
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
        MeshCalResult meshCalResult;
        private Mesh2DTo3D mesh2DTo3D;

        EdgeImageDetector edgeImage;
        MarchingCubeBorder marchingCubeBorder;
        public System.Action<Texture2D> OnEdgeTexUpdate;
        public System.Action<MeshCalResult> OnMeshCalculationDone;

        int resize = 64;

        //ComputeShader Properties
        private Color[] process_tex_colors;
        private Color[] process_tex_colors_cpu;

        private ComputeBuffer _colorBuffer;
        private int floatSize = sizeof(float);
        int GetColorKernelHandle;
        bool IsComputeShaderFree = true;

        Camera _camera;
        Vector2 _meshPosition = new Vector2();
        #endregion


        public void Start()
        {
            meshCalResult = new MeshCalResult();
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
        public async void CaptureEdgeBorderMesh(int skinSize, MeshObject meshObject, TextureUtility.TextureStructure textureStructure)
        {

            if (OnEdgeTexUpdate != null)
                OnEdgeTexUpdate(edgeOutputTex);

            var maskColors = await imageMaskGeneator.AsyncCreateBorder(process_tex_colors_cpu);

            if (!CheckIfValid(maskColors)) return;

            var marchCubeResult = await ProcessMarchCube(maskColors.img, resize, resize);
            var meshData = mesh2DTo3D.CreateMeshData(marchCubeResult.vertices, marchCubeResult.triangles, marchCubeResult.uv, null);

            meshObject.SetMesh(mesh2DTo3D.CreateMesh(meshObject.mesh, meshData), highlightRenderer, skinSize);

            if (OnMeshCalculationDone != null && meshObject != null)
                OnMeshCalculationDone(GetMeshCalResult(maskColors, meshObject, textureStructure));             
        }

        public async void CaptureContourMesh(RenderTexture skinTexture, MeshObject meshObject, TextureUtility.TextureStructure textureStructure)
        {
            var maskColors = await imageMaskGeneator.AsyncCreateMask(process_tex_colors_cpu);

            if (!CheckIfValid(maskColors)) return;

            var marchCubeResult = await ProcessMarchCube(maskColors.img, resize, resize);

            var mesh = await MeshTo3D(marchCubeResult, meshObject.mesh);

            if (mesh.Item1 != null) {
                meshObject.SetMesh(mesh.Item1, skinTexture, skinTexture.width);
                meshObject.SetControlPoint(mesh.Item2.topVertice, mesh.Item2.bottomVertice);
                meshObject.GenerateControlPoints();
            }

            if (OnMeshCalculationDone != null && meshObject != null)
                OnMeshCalculationDone(GetMeshCalResult(maskColors, meshObject, textureStructure));
        }
        #endregion

        private async UniTask<MarchingCube.MarchingCubeResult> ProcessMarchCube(Color[] maskImage, int textureWidth, int textureHeight)
        {
            return await UniTask.Run(() =>
            {
                meshGenerator.GenerateMesh(maskImage, textureWidth, textureHeight, 1);
                return marchingCube.Calculate(meshGenerator.squareGrid);
            });
        }

        private async UniTask<(Mesh, Mesh2DTo3D.MeshData)> MeshTo3D(MarchingCube.MarchingCubeResult meshResult, Mesh mesh) {
            Vector3[] borderVertices = await marchingCubeBorder.AsynSort(meshResult.borderVertices);
            //TestBorderArray = borderVertices;
            Mesh2DTo3D.MeshData meshData = await mesh2DTo3D.Convert(meshResult, borderVertices);

            return (mesh2DTo3D.CreateMesh(mesh, meshData), meshData);
        }

        private MeshCalResult GetMeshCalResult(MooreNeighborhood.MooreNeighborInfo meshInfo, MeshObject meshObject, TextureUtility.TextureStructure textureStructure) {

            meshCalResult.meshObject = meshObject;
            float _x = ((meshInfo.centerPoint.x / resize) * textureStructure.xRatio) + (textureStructure.xResidualRatio * 0.5f);
            //float y = (meshInfo.centerPoint.y * 4) + startPixelY;
            float _y = ((meshInfo.centerPoint.y / resize) * textureStructure.yRatio) + (textureStructure.yResidualRatio * 0.5f);

            float x = (meshInfo.centerPoint.x / resize);
            float y = (meshInfo.centerPoint.y / resize);

            //Debug.Log(string.Format("Original X {0}, Y {1}", x, y));

            _meshPosition.Set(x, y);

            meshCalResult.screenPoint = _meshPosition;
            meshCalResult.meshObject = meshObject;

            return meshCalResult;
            //point.z = 0;

            //if (meshObject != null)
            //    meshObject.transform.position = point;
        }

        private bool CheckIfValid(MooreNeighborhood.MooreNeighborInfo meshInfo) {
            return (meshInfo.area > 40);
        }

        public struct MeshCalResult {
            public MeshObject meshObject;
            public Vector2 screenPoint;
        }

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