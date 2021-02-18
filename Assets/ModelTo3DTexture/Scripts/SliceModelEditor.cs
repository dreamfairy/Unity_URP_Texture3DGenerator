using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public sealed class SpliceModelEditor : MonoBehaviour
{
    public bool StartSlice = false;
    public int SliceLayerCount = 0;
    private Camera m_sliceCamera;
    private Material m_sliceMaterial;
    private float m_modelHeight = 1;

    public void Awake()
    {
        Renderer renderer = this.gameObject.GetComponentInChildren<Renderer>();
        if (renderer)
        {
            m_sliceMaterial = renderer.sharedMaterial;
            Bounds bounds = renderer.bounds;
            m_modelHeight = bounds.max.y - bounds.min.y;
        }

        CreateCamera();
    }

    Camera CreateCamera()
    {
        Transform cameraTrans = this.transform.Find("SliceCamera");
        if (cameraTrans)
        {
            return m_sliceCamera;
        }

        GameObject cameraGo = new GameObject("SliceCamera", typeof(Camera));
        cameraGo.transform.parent = this.transform;
        cameraGo.transform.localPosition = new Vector3(0, m_modelHeight, 0);
        cameraGo.transform.rotation = Quaternion.Euler(90, 0, 0);

        m_sliceCamera = cameraGo.GetComponent<Camera>();
        m_sliceCamera.clearFlags = CameraClearFlags.SolidColor;
        m_sliceCamera.backgroundColor = Color.black;
        m_sliceCamera.orthographic = true;
        m_sliceCamera.orthographicSize = m_modelHeight * 0.5f;
        m_sliceCamera.nearClipPlane = 0;
        m_sliceCamera.farClipPlane = m_modelHeight;
        m_sliceCamera.enabled = false;

        return m_sliceCamera;
    }

    float Remap(float original, float oldMinVal, float oldMaxVal, float newMinVal, float newMaxVal)
    {
        float ret = newMinVal + (newMaxVal - newMinVal) / (oldMaxVal - oldMinVal) * (original - oldMinVal);
        return ret;
    }

    void CreateTextureArray()
    {
        Texture2D[] slice3DArray = new Texture2D[SliceLayerCount];

        for(int i = 0; i < SliceLayerCount; i++)
        {
            RenderTexture rt = new RenderTexture(SliceLayerCount, SliceLayerCount, 24, RenderTextureFormat.R8);
            float setLayer = Remap(i, 0, SliceLayerCount, 0, 2);

            m_sliceMaterial.SetFloat("_height", setLayer);
            m_sliceCamera.nearClipPlane = setLayer;
            m_sliceCamera.targetTexture = rt;
            m_sliceCamera.Render();

            Texture2D tex2D = new Texture2D(SliceLayerCount, SliceLayerCount, TextureFormat.R8, false);
            RenderTexture.active = rt;
            tex2D.ReadPixels(new Rect(0, 0, SliceLayerCount, SliceLayerCount), 0, 0);
            tex2D.Apply();

            slice3DArray[i] = tex2D;
        }

        Create3DTexture(slice3DArray);
    }

    void Create3DTexture(Texture2D[] slice3DArray)
    {
        int width = slice3DArray[0].width;
        int height = slice3DArray[0].height;
        int depth = slice3DArray.Length;

        Texture3D tex3D = new Texture3D(width, height, depth, TextureFormat.R8, true);
        Color[] colArr = new Color[width * height * depth];

        int idx = 0;
        for(int z = depth - 1; z >=0; z--)
        {
            Texture2D currentTex = slice3DArray[z];
            for(int i = 0; i < width; i++)
            {
                for(int j = 0; j < height; j++)
                {
                    colArr[idx] = currentTex.GetPixel(i, j);
                    idx++;
                }
            }
        }
        tex3D.SetPixels(colArr);
        tex3D.Apply();

        UnityEditor.AssetDatabase.CreateAsset(tex3D, "Assets/TestTex3D.asset");
        slice3DArray = null;
        tex3D = null;
    }

    private void Update()
    {
        if (StartSlice)
        {
            StartSlice = false;
            CreateTextureArray();
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}