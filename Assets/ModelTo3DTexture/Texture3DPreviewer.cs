using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public sealed class Texture3DPreviewer : MonoBehaviour
{
    public float Height = 1.0f;
    public int LayerCount = 128;
    public float Scale = 1.0f;
    public Material RenderMaterial;
    public Mesh RenderMesh;

    private Matrix4x4[] RenderMatrix;

    private void Update()
    {
        if(RenderMaterial && RenderMesh)
        {
            if(null == RenderMatrix || RenderMatrix.Length != LayerCount)
            {
                RenderMatrix = new Matrix4x4[LayerCount];

                for (int i = 0; i < LayerCount; i++)
                {
                    float curHeight = (Height / LayerCount) * i;
                    Matrix4x4 trs = Matrix4x4.TRS(this.transform.position + new Vector3(0, curHeight, 0), Quaternion.Euler(90, 0, 0), Vector3.one * Scale);
                    RenderMatrix[i] = trs;
                }
            }

            Graphics.DrawMeshInstanced(RenderMesh, 0, RenderMaterial, RenderMatrix);
        }
    }
}