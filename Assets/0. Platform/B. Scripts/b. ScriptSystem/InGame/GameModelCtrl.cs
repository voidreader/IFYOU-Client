using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;

namespace PIERStory
{
    public class GameModelCtrl : MonoBehaviour
    {
        CubismRenderController cubismRender = null;
        public string modelType = "live2d";
        public CubismModel model = null;
        [HideInInspector] public Animation modelAnim;
        [HideInInspector] public Dictionary<string, AnimationClip> DictMotion;
        [HideInInspector] public RawImage currRenderTexture;        // 현재 그려지고 있는 곳


        // About DefaultCharacter
        SpriteRenderer defaultSprite = null;
        GameObject dummyInfo;
        //DefaultCharacterInfo info;

        [Header("ScriptModelMount Value")]
        public string direction;
        public string originModelName;
        public string speaker;

        [Header("등장, 퇴장 연출")]
        public string in_effect = string.Empty;
        public string out_effect = string.Empty;

        const float fadeTime = 0.4f;
        const float slideTime = 0.7f;

        bool autoPlay = false;          // 자동진행 값. skip을 사용해도 true로 동일하게 간주한다


        [Header("Character tall")]
        public int tallGrade = 0; // 0~4

        float height, width;

        #region const float value

        const float fixPosX = 2.5f;
        const float animTime = 0.5f;
        const float listenerScale = 0.95f; // 2인 스탠딩 말하지 않는 캐릭터의 크기 
        const float talkerScale = 0.98f; // 2인 스탠딩 말하고 있는 캐릭터의 크기 

        #endregion

        
        void Start()
        {
            cubismRender = GetComponentInChildren<CubismRenderController>();

            // height는 width를 구하기 위해 필요한 변수이다
            // 실질적으로 사용되는 곳은 SlideIn/out의 연출이 일어날 때인데, 화면 바깥에서 들어오게 하기 위함임
            // generalCam은 일러스트, 라이브오브제, 모델 등 실질적 화면에 렌더해주는 Camera이며, 이 카메라를 기점으로 height, width를 구하고 있음
            height = 2 * ScreenEffectManager.main.generalCam.orthographicSize;
            width = height * ScreenEffectManager.main.generalCam.aspect;
        }

        /// <summary>
        /// 모델 세팅
        /// </summary>
        /// <param name="dir">해당 캐릭터 시선 방향</param>
        public void SetModel(CubismModel __model, string dir)
        {
            model = __model;
            modelType = GameConst.MODEL_TYPE_LIVE2D;
            direction = dir;
            // 처음 그려지는 곳은 무조건 중앙이다.
            currRenderTexture = ViewGame.main.modelRenders[1];
        }

        /// <summary>
        /// Layer 변경
        /// </summary>
        /// <param name="layerName">레이어 명칭</param>
        public void ChangeLayerRecursively(Transform trans, string layerName)
        {
            trans.gameObject.layer = LayerMask.NameToLayer(layerName);
            foreach (Transform tr in trans)
                ChangeLayerRecursively(tr, layerName);
        }


        /// <summary>
        /// 캐릭터 화면에서 감춰주세요!
        /// 필요에 따라 ExitCharacter => DisableModel(최종)
        /// </summary>
        /// <param name="CharacterPosIndex">0(왼쪽), 1(가운데), 2(오른쪽)</param>
        /// <param name="useSkip"></param>
        public void HideModel(int CharacterPosIndex, bool useSkip)
        {
            // 스킵으로 진행하거나, 가운데 캐릭터는 바로 비활성화 처리 
            // 가운데 캐릭터를 Fadeout 처리하면 다른 캐릭터가 등장할때 겹쳐..!
            if (useSkip || CharacterPosIndex == 1)
            {
                //DisableModel();
                return;
            }

            // 어느 위치냐에 따라서 좌우로 이동하면서 퇴장 처리 
            //if (CharacterPosIndex == 0)
            //    ExitCharacter(-width);
            //else
            //    ExitCharacter(width);
        }

        /// <summary>
        /// 캐릭터 키(tall) 세팅
        /// </summary>
        public void SetTall(int __grade)
        {
            if (tallGrade >= __grade)
                return;

            tallGrade = __grade;

        }

        /// <summary>
        /// 충돌체 제거하기. (최초 로딩시에만 필요하다)
        /// </summary>
        public void RemoveColliders()
        {
            if (model == null)
                return;

            BoxCollider box = null;

            for (int i = 0; i < model.Drawables.Length; i++)
            {
                box = model.Drawables[i].GetComponent<BoxCollider>();
                if (box != null)
                    box.enabled = false;
            }
        }
    }

}

