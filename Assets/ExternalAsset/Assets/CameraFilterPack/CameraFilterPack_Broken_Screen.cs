////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2020 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Broken/Broken_Screen")]
public class CameraFilterPack_Broken_Screen : MonoBehaviour
{
    #region Variables
    public Shader SCShader;
    private float TimeX = 1.0f;
    [Range(0, 1)]
    public float Fade = 1f;
    [Range(-1, 1)]
    public float Shadow = 1f;
    private Material SCMaterial;
    public Texture2D Texture2;

    // 추가 변수 
    public bool isAnimate = false; // 애니메이트 시키는지? 
    public string command = string.Empty;
    bool isForward = false;
    

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
        SCShader = Shader.Find("CameraFilterPack/Broken_Screen");
    }

    /// <summary>
    /// 초기화 
    /// </summary>
    /// <param name="__fade"></param>
    /// <param name="__shadow"></param>
    public void Init(float __fade = 1, float __shadow = 1)
    {
        isAnimate = false;

        Fade = __fade;

        if (Fade > 1)
            Fade = 1;

        if (Fade < 0)
            Fade = 0;

        Shadow = __shadow;

        if (Shadow > 1)
            Shadow = 1;

        if (Shadow < -1)
            Shadow = -1;

        // SetAnim("브레이크");
    }

    /// <summary>
    /// 애니메이팅 처리 하기
    /// </summary>
    /// <param name="__targetFade"></param>
    /// <param name="__targetShadow"></param>
    public void SetAnim(string __command) 
    {
        command = __command; // 명령어 기반으로 돌린다. 
        isAnimate = true;

        // 브레이크 & 복원 
        if (command != "복원")
        {
            Fade = 0;
            isForward = true;
        }
        else
        {
            Fade = 1;
            isForward = false;
        }
    }

    



    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (SCShader != null)
        {
            TimeX += Time.deltaTime;
            if (TimeX > 100) 
                TimeX = 0;


            material.SetFloat("_TimeX", TimeX);
            material.SetFloat("_Fade", Fade);
            material.SetFloat("_Shadow", Shadow);
            material.SetTexture("_MainTex2", Texture2);

            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }
    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (Application.isPlaying != true)
            SCShader = Shader.Find("CameraFilterPack/Broken_Screen");
#endif

        if (!isAnimate || SCShader == null)
            return;

        if(isForward)
        {
            Fade += 0.01f;
            if (Fade > 1)
            {
                Fade = 1;
                isAnimate = false;
            }
        }
        else
        {
            Fade -= 0.01f;
            if(Fade < 0)
            {
                Fade = 0;
                isAnimate = false;
                // 자동 비활성화 처리 
                SetInactive();
            }
        }
    }

    /// <summary>
    /// 비활성화
    /// </summary>
    void SetInactive()
    {
        this.enabled = false;
    }


    void OnDisable()
    {
        if (SCMaterial)
        {
            DestroyImmediate(SCMaterial);
        }

    }


}