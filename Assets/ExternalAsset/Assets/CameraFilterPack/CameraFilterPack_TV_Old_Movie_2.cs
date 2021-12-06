///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2020 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Old Film/Old_Movie_2")]
public class CameraFilterPack_TV_Old_Movie_2 : MonoBehaviour
{
    #region Variables
    public Shader SCShader;
    private float TimeX = 1.0f;

    private Material SCMaterial;
    [Range(1f, 60f)]
    public float FramePerSecond = 15f;
    [Range(0f, 5f)]
    public float Contrast = 1f;
    [Range(0f, 4f)]
    public float Burn = 0f;
    [Range(0f, 16f)]
    public float SceneCut = 1f;
    [Range(0f, 1f)]
    public float Fade = 1f;

    #endregion
    #region Properties
    Material material {
        get {
            if (SCMaterial == null)
            {
                SCMaterial = new Material(SCShader);
                SCMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return SCMaterial;
        }
    }
    #endregion
    void Start()
    {

        SCShader = Shader.Find("CameraFilterPack/TV_Old_Movie_2");

    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (SCShader != null)
        {
            TimeX += Time.deltaTime;
            if (TimeX > 100) TimeX = 0;
            material.SetFloat("_TimeX", TimeX);
            material.SetFloat("_Value", FramePerSecond);
            material.SetFloat("_Value2", Contrast);
            material.SetFloat("_Value3", Burn);
            material.SetFloat("_Value4", SceneCut);
            material.SetFloat("_Fade", Fade);
            material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0.0f, 0.0f));
            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }

    void Update()
    {

#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            SCShader = Shader.Find("CameraFilterPack/TV_Old_Movie_2");
        }
#endif
    }
    void OnDisable()
    {
        if (SCMaterial)
        {
            DestroyImmediate(SCMaterial);
        }
    }
}
