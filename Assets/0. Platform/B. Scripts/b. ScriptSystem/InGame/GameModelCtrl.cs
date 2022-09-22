using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Framework.Motion;

namespace PIERStory
{
    public class GameModelCtrl : MonoBehaviour
    {
        CubismRenderController cubismRender = null;
        public string modelType = "live2d";
        public CubismModel model = null;
        
        public bool isAddressable = false; // 어드레서블로 생성 여부 
        public ScriptRow activeRow; // 활성화된 스크립트 Row 
        public CubismMotionController motionController = null;
        [HideInInspector] public Animation modelAnim;
        [HideInInspector] public Dictionary<string, AnimationClip> DictMotion;
        [HideInInspector] public RawImage currRenderTexture;        // 현재 그려지고 있는 곳

        [HideInInspector] public List<string> motionLists = new List<string>();
        [HideInInspector] public string currencyName = string.Empty;    // 로비 꾸미기에서만 사용될 값

        [SerializeField] bool isModelActivated = false; // 모델이 한번이라도 Activate 되었었지에 대한 변수
        [SerializeField] string motionName = string.Empty; // 플레이할 모션 이름 

        // About DefaultCharacter
        SpriteRenderer defaultSprite = null;
        GameObject dummyInfo;
        DefaultCharacterInfo info;
        

        [Header("ScriptModelMount Value")]
        public string direction;
        public string originModelName;
        public string speaker;

        [Header("등장, 퇴장 연출")]
        public string in_effect = string.Empty;
        public string out_effect = string.Empty;

        const float fadeTime = 0.4f;
        const float slideTime = 0.7f;

        bool autoPlay = false;                 // 자동진행 값. skip을 사용해도 true로 동일하게 간주한다
        public bool moveComplete = true;      // 등장, 퇴장 연출 완료 = true, 미완 = false
        
        public float fadeInAlpha = 1; // 페이드인때의 알파값 - 50% 페이드인에서는 0.5로 설정된다. 2022.06.10


        [Header("Character tall")]
        public int tallGrade = -1; // 0~4

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
            if (GameManager.main == null)
                return;

            height = 2 * ScreenEffectManager.main.generalCam.orthographicSize;
            width = height * ScreenEffectManager.main.generalCam.aspect;
        }

        /// <summary>
        /// 모델 세팅
        /// </summary>
        /// <param name="dir">해당 캐릭터 시선 방향</param>
        public void SetModel(CubismModel __model, string dir, bool __isAssetBundle = false)
        {
            model = __model;
            modelType = CommonConst.MODEL_TYPE_LIVE2D;
            direction = dir;
            // 처음 그려지는 곳은 무조건 중앙이다.

            if (GameManager.main == null)
                return;

            currRenderTexture = ViewGame.main.modelRenders[1];
        }
        
        /// <summary>
        /// 키 어드민에서 지정하기 
        /// </summary>
        /// <param name="__tallGrade"></param>
        public void SetTallGradeByAdmin(int __tallGrade) {
            tallGrade= __tallGrade;
        }

        /// <summary>
        /// 말하지 않는 스탠딩을 밀어주며, 스케일 값을 변경해준다
        /// </summary>
        /// <param name="targetIndex">캐릭터가 밀려나게 될 위치</param>
        public void PushListenerToSide(int targetIndex, bool __autoPlay = false)
        {
            // currentRenderTexture는 현재 C를 사용하고 있기 떄문에 Talker에 있던 것을 EnterReady로 옮겨준다
            // 밑에서 Layer를 변경해주고, SetNonTalker가 선언되면서 Object의 부모가 변경되고, currentRenderTexture도 L or R로 변경될 것임
            currRenderTexture.transform.SetParent(ViewGame.main.modelRenderParents[0]);

            switch (targetIndex)
            {
                case 0:
                    if (__autoPlay)
                        SetAutoPlayListener(-1);
                    else
                    {
                        OnMoveStart();
                        transform.DOMove(new Vector3(-fixPosX, transform.position.y, 0f), animTime).OnComplete(SetListener);
                    }

                    ChangeLayerRecursively(transform, GameConst.LAYER_MODEL_L);
                    SetCurRenderTexture(GameConst.LAYER_MODEL_L);
                    break;
                
                case 2:
                    if (__autoPlay)
                        SetAutoPlayListener(1);
                    else
                    {
                        OnMoveStart();
                        transform.DOMove(new Vector3(fixPosX, transform.position.y, 0f), animTime).OnComplete(SetListener);
                    }

                    ChangeLayerRecursively(transform, GameConst.LAYER_MODEL_R);
                    SetCurRenderTexture(GameConst.LAYER_MODEL_R);
                    break;
            }
        }

        /// <summary>
        /// 오토플레이에서 리스너 처리 (트윈없음)
        /// </summary>
        void SetAutoPlayListener(int flip)
        {
            transform.localPosition = new Vector3(fixPosX * flip, 0f, 0f);

            // 뒤집혀 있으면 있는 상태 그대로 두기
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f) * listenerScale;
            else
                transform.localScale = Vector3.one * listenerScale;
        }

        /// <summary>
        /// 캐릭터 모델 활성화 
        /// </summary>
        void ActivateModel(ScriptRow row, bool __instant = false)
        {
            if (cubismRender == null)
                cubismRender = GetComponentInChildren<CubismRenderController>();

            // 모델 정보 있을때, 없을때 처리 분기. 
            if (cubismRender != null)
                cubismRender.Opacity = 1f;
            else
            {
                // 모델 등록 안한 상태에서의 처리. 
                modelType = "DefaultCharacter";
                defaultSprite = GetComponentInChildren<SpriteRenderer>();
                defaultSprite.sprite = GameManager.main.defaultCharacterSprite;
                defaultSprite.transform.localPosition = new Vector3(0f, 10f, 0f);

                if (dummyInfo == null)
                {
                    // 더미 캐릭터 정보를 prefab으로 생성하고 Canvas가 필요하니까 viewGame 자식으로 붙여준다
                    // 생성되어 있으면 또 생성할 필요가 없고 이미 연결되어 있기 때문에 또 연결할 필요가 없다.
                    dummyInfo = Instantiate(GameManager.main.infoPrefab, ViewGame.main.transform);
                    // Prefab에 들어 있는 스크립트에 정보 전달
                    info = dummyInfo.GetComponent<DefaultCharacterInfo>();
                }

                // 따라다녀야 하니 transform 값도 전달해줘야 함
                info.SetCharacterInfo(transform, row.speaker, row.character_expression);
            }


            isModelActivated = true; // true로 처리한다. 한번만 true되면 된다.
            activeRow = row;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 캐릭터 애니메이션 재생 
        /// </summary>
        /// <param name="__row"></param>
        /// <param name="characterPos">캐릭터 위치(중앙, 좌우</param>
        public void PlayCubismAnimation(ScriptRow __row, int characterPos)
        {
            
            // 더미 캐릭터고 스탠딩 캐릭터고 여기에서 다 관리하기 떄문에 이 스크립트에서 자체적으로 SetActive true/false 해주면 된다. 이동 및 크기 변경도 마찬가지
            Debug.Log(string.Format("PlayCubismAnimation [{0}], [{1}]", __row.speaker, characterPos));
            activeRow = __row;
            
            // 스탠딩 등장시, 전화 상태 해제 
            if(ViewGame.main.userCall)
            {
                ViewGame.main.userCall = false;
                ViewGame.main.HIdePhoneImage();
            }

            #region autoPlay value
            autoPlay = __row.autoplay_row < 1 ? false : true;

            if (GameManager.main.useSkip || GameManager.main.isJustSkipStop)
                autoPlay = true;
            #endregion

            // 모델이 비활성 상태일때 활성화가 되면 페이드인 처리를 한다. 
            // * isModelActivated bool 변수를 통해서 한번이라도 활성화 되었었는지를 체크합니다. 
            if (!gameObject.activeSelf || !isModelActivated)
            {
                Debug.Log(string.Format("PlayCubismAnimation !gameObject.activeSelf [{0}], [{1}]", __row.speaker, characterPos));
                
                ActivateModel(__row);

                // 등장 연출과 퇴장 연출을 미리 받아둔다.
                in_effect = __row.in_effect;
                out_effect = __row.out_effect;
                
                SetFadeInAlpha();
                EnterTalkingCharacter(characterPos);
            }
            else // 모델이 이미 화면에 나와있는 상태
            {
                // 2인 스탠딩중이면 talker니까 z 포지션을 앞으로 옮겨준다
                if (characterPos != 1)
                {   
                    // 스킵일때와 아닐때 구분 필요. 
                    if(GameManager.main.useSkip) {
                        // 방향 유지한다. 
                        if(transform.localScale.x < 0f) 
                            transform.localScale = new Vector3(-1f, 1f, 1f) * talkerScale;
                        else 
                            transform.localScale = Vector3.one * talkerScale;
                    }
                    else {
                        if (transform.localScale.x < 0f)
                            transform.DOScale(new Vector3(-1f, 1f, 1f) * talkerScale, animTime);
                        else
                            transform.DOScale(Vector3.one * talkerScale, animTime);
                    }
                    
                    // 
                    SetTalker();
                }
                else
                {
                    // 1인 스탠딩으로 변경되었으니 원상복귀
                    // 스킵중엔 연출 무효
                    if (GameManager.main.useSkip)
                    {
                        transform.localPosition = Vector3.zero;
                        
                        if(transform.localScale.x < 0f) 
                            transform.localScale = new Vector3(-1, 1, 1);
                        else
                            transform.localScale = Vector3.one; 
                        
                    }
                    else
                    {
                        SetCurRenderTexture(GameConst.LAYER_MODEL_C);
                        ChangeLayerRecursively(transform, GameConst.LAYER_MODEL_C);

                        transform.DOMoveX(0f, animTime);

                        // 이미 뒤집혀 있으면 계속 뒤집혀 있게
                        if (transform.localScale.x < 0f)
                            transform.DOScale(new Vector3(-1f, 1f, 1f), animTime);
                        else
                            transform.DOScale(Vector3.one, animTime);
                    }
                }
            } // 화면 '등장'에 대한 처리 종료 

            // * 모션 처리 시작 
            // 모션 없으면 진행하지 않음 
            if (DictMotion != null && !DictMotion.ContainsKey(__row.character_expression))
            {
                // Debug.Log(string.Format("WARNING! : {0} doesn't exists in {1}", __row.character_expression, __row.speaker));
                SystemManager.main.ShowMissingFunction(string.Format("WARNING! : {0} doesn't exists in {1}", __row.character_expression, __row.speaker));
                return;
            }


            // 더미 데이터는 애니메이션이 없으니 그만
            if (model == null)
            {
                // character_expression이 변경되었을 수도 있으니까 변경해줘야 해
                info.SetCharacterInfo(transform, __row.speaker, __row.character_expression);
                return;
            }

            // 퇴장 연출을 hide 전에 변경 하면 변경한 것에 대해 적용한다.
            // 등장 연출은 등장시에만 발동하기 때문에 이미 스크린에 있는 경우에는 무시한다.
            if (!string.IsNullOrEmpty(__row.out_effect))
                out_effect = __row.out_effect;

            // * 실제 클립 재생
            motionName = __row.character_expression;
            
            // * 립싱크 기능 추가 2022.01.25 
            // 립싱크 모션 유무 체크 
            // 모션_M 있고, 대화형 템플릿, 립싱크 관련 제어 컬럼 값 없을때 처리 
            if(DictMotion.ContainsKey(motionName + "_M") && CheckLipSyncTemplate(__row.template, __row.script_data) && string.IsNullOrEmpty(__row.controlMouthCommand)) {
                motionName = motionName + "_M"; // 립싱크 모션으로 변경 
            }
            
            // * 에셋번들과 다운로드로 생성한 모델에서 플레이 방식이 다르다. 
            if(motionController != null) {
                motionController.PlayAnimation(DictMotion[motionName], 0, CubismMotionPriority.PriorityForce);
            }
            else {
                // 클립 재생 - 생 다운로드, 혹은 fade Motion 없음 
                modelAnim.CrossFade(motionName, 0.3f);    
            }
           
        } // End of PlayAnimation
        
        
        /// <summary>
        /// 현재 캐릭터 모델의 애니메이션 정지시키기 
        /// </summary>
        public void StopAnimation() {
            
            if(motionController != null) {
                motionController.StopAllAnimation(); // 모든 애니메이션 정지 
            }
            else {
                modelAnim.Stop();
            }
        }
        
        /// <summary>
        /// 로비에서 모션 재생하기 
        /// </summary>
        /// <param name="__clip"></param>
        public void PlayLobbyAnimation(string __motionName) {
            // * 에셋번들과 다운로드로 생성한 모델에서 플레이 방식이 다르다. 
            if(motionController != null) {
                motionController.PlayAnimation(DictMotion[__motionName], 0, CubismMotionPriority.PriorityForce);
            }
            else {
                // 클립 재생 - 생 다운로드, 혹은 fade Motion 없음 
                modelAnim.CrossFade(__motionName, 0.3f);    
                
            }
        }
        
        /// <summary>
        /// 립싱크 가능 템플릿 체크 
        /// </summary>
        /// <param name="__template"></param>
        /// <returns></returns>
        bool CheckLipSyncTemplate(string __template, string __data) {
            
            if(string.IsNullOrEmpty(__data) || string.IsNullOrEmpty(__template))
                return false;
            
            // 대화, 외침, 속삭임, 중요대사
            if(__template == GameConst.TEMPLATE_TALK ||  __template == GameConst.TEMPLATE_YELL ||  __template == GameConst.TEMPLATE_WHISPER ||  __template == GameConst.TEMPLATE_SPEECH)
                return true;
                
            return false;
        }
        
        
        /// <summary>
        /// 캐릭터가 비활성화 될때는 립싱크를 멈추게 한다. 
        /// </summary>
        void OffLipSync() {
            if(!motionName.Contains("_M")) 
                return;
                
            motionName = motionName.Replace("_M", ""); // _M 을 제거
            
            // 립싱크 아닌 모션을 재생처리 
            if(motionController != null) {
                motionController.PlayAnimation(DictMotion[motionName], 0, CubismMotionPriority.PriorityForce);
            }
            else {
                // 클립 재생 - 생 다운로드, 혹은 fade Motion 없음 
                modelAnim.CrossFade(motionName, 0.3f);    
            }            
            
        }
        

        /// <summary>
        /// 화면에 없었던 캐릭터의 진입 처리 
        /// </summary>
        /// <param name="characterPosIndex">0은 왼쪽, 1은 중앙, 2는 오른쪽</param>
        void EnterTalkingCharacter(int characterPosIndex)
        {
            // 스킵이나 자동진행이 아니면 무조건 연출을 시작한다고 생각한다
            if (!autoPlay)
                OnMoveStart();

            SetTalker();

            // 캐릭터 위치(L,C,R)에 따른 위치 및 크기 조정
            switch (characterPosIndex)
            {
                case 0: // 좌측에서 등장
                    SetCharacterTransform(GameConst.VIEWDIRECTION_LEFT, -fixPosX);
                    break;

                case 1:
                    SetCharacterTransform(null, 0f);
                    break;

                case 2: // 우측에서 등장
                    SetCharacterTransform(GameConst.VIEWDIRECTION_RIGHT, fixPosX);
                    break;
            }
        }

        /// <summary>
        /// 등장하는 캐릭터 scale,pos값 조정
        /// </summary>
        /// <param name="__dir">방향</param>
        /// <param name="posX">옮겨질 위치</param>
        void SetCharacterTransform(string __dir, float posX)
        {
            // 자동진행 or skip처리
            if (autoPlay)
            {
                if(string.IsNullOrEmpty(__dir))
                {
                    transform.localPosition = Vector3.zero;
                    transform.localScale = Vector3.one;
                }
                else
                {
                    transform.localPosition = new Vector3(posX, 0f, -1f);

                    if (direction.Equals(__dir))
                        transform.localScale = new Vector3(-1f, 1f, 1f) * talkerScale;
                    else
                        transform.localScale = Vector3.one * talkerScale;
                }
                
                return;
                
            } // 자동진행 및 스킵에 대한 처리 종료
            
            
            
            // 일반적인 처리 
            if(string.IsNullOrEmpty(__dir)) // 중앙 등장. 
            {
                /*
                if(transform.localScale.x < 0f)
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                else
                    transform.localScale = Vector3.one;
                */
                
                transform.localScale = Vector3.one; // 스케일 1로 초기화 

                // 중앙등장은 진입효과가 지정되지 않았으면 페이드인으로 처리 
                if(string.IsNullOrEmpty(activeRow.in_effect)) {
                    in_effect = GameConst.INOUT_EFFECT_FADEIN;
                    SetFadeInAlpha();
                }
                
                FadeIn(0f, GameConst.LAYER_MODEL_C);
            }
            else
            {
                // 시선 방향과 위치가 동향이면 뒤집기
                if (direction.Equals(__dir))
                    transform.localScale = new Vector3(-1f, 1f, 1f) * talkerScale;
                else
                    transform.localScale = Vector3.one * talkerScale;

                // 2인 스탠딩으로 등장 연출
                if (posX < 0f)
                    EnterCharacterDirection(-width, posX, GameConst.LAYER_MODEL_L);
                else
                    EnterCharacterDirection(width, posX, GameConst.LAYER_MODEL_R);
            }
        }

        /// <summary>
        /// 등장 연출
        /// </summary>
        /// <param name="posX">시작 위치</param>
        /// <param name="moveX">놓여질 위치</param>
        void EnterCharacterDirection(float posX, float moveX, string layerName)
        {
            switch (in_effect)
            {
                case GameConst.INOUT_EFFECT_FADEIN:
                    FadeIn(moveX, layerName);
                    break;
                default:
                    SlideIn(posX, moveX, layerName);
                    break;
            }
        }

        /// <summary>
        /// 그려질 renderTexture(RawImage) 설정
        /// </summary>
        /// <param name="layerName"></param>
        void SetCurRenderTexture(string layerName)
        {
            switch (layerName)
            {
                case GameConst.LAYER_MODEL_L:
                    currRenderTexture = ViewGame.main.modelRenders[0];
                    break;
                case GameConst.LAYER_MODEL_C:
                    currRenderTexture = ViewGame.main.modelRenders[1];
                    break;
                case GameConst.LAYER_MODEL_R:
                    currRenderTexture = ViewGame.main.modelRenders[2];
                    break;
            }
            
            if(fadeInAlpha < 1) {
                currRenderTexture.color = new Color(currRenderTexture.color.r, currRenderTexture.color.g, currRenderTexture.color.b, fadeInAlpha);
            }
            else {
                currRenderTexture.color = new Color(currRenderTexture.color.r, currRenderTexture.color.g, currRenderTexture.color.b, 1);
            }
        }

        /// <summary>
        /// 페이드인 등장 연출
        /// </summary>
        /// <param name="posX">등장 x 좌표값</param>
        /// <param name="layerName"></param>
        void FadeIn(float posX, string layerName)
        {
            // 바꾸려는 레이어와 같지 않은 경우에만 변경한다
            if (!gameObject.layer.Equals(LayerMask.NameToLayer(layerName)))
                ChangeLayerRecursively(transform, layerName);

            SetCurRenderTexture(layerName);
            transform.localPosition = new Vector3(posX, transform.localPosition.y, 0);
            currRenderTexture.color = new Color(currRenderTexture.color.r, currRenderTexture.color.g, currRenderTexture.color.b, 0);

            currRenderTexture.DOFade(fadeInAlpha, fadeTime).OnComplete(OnMoveCompleted);
        }


        /// <summary>
        /// 2인 스탠딩에서 캐릭터 퇴장 
        /// </summary>
        void FadeOut()
        {
            OnMoveStart();
            currRenderTexture.DOFade(0f, fadeTime).OnComplete(DisableModel);
        }

        void SlideIn(float posX, float moveX, string layerName)
        {
            if (!gameObject.layer.Equals(LayerMask.NameToLayer(layerName)))
                ChangeLayerRecursively(transform, layerName);

            SetCurRenderTexture(layerName);

            transform.localPosition = new Vector3(posX, transform.localPosition.y, 0);
            transform.DOMoveX(moveX, slideTime).OnComplete(OnMoveCompleted);
        }

        void SlideOut(float moveX)
        {
            OnMoveStart();
            transform.DOMoveX(moveX, slideTime).OnComplete(() =>
            {
                DisableModel();
            });
        }

        /// <summary>
        /// 캐릭터를 '말하고 있는' 상태로 설정. 
        /// </summary>
        void SetTalker()
        {
            // 더미 캐릭터인경우 상태설정 안함
            if(defaultSprite != null && defaultSprite.sprite != null)
                return;

            currRenderTexture.transform.SetParent(ViewGame.main.modelRenderParents[2]);
        }

        /// <summary>
        /// 2인 스탠딩 중, 캐릭터를 '듣는' 상태로 설정. 
        /// </summary>
        public void SetListener()
        {
            currRenderTexture.transform.SetParent(ViewGame.main.modelRenderParents[1]);

            // 리스터는 살짝 작은 스케일로 조정해준다. 
            if (transform.localScale.x < 0f)
                transform.DOScale(new Vector3(-1f, 1f, 1f) * listenerScale, animTime);
            else
                transform.DOScale(listenerScale, animTime);

            // * 22.01.21
            // 빠르게 진행하는 경우 투명도 값이 0~1 사이에 있는채로 방치되어 다시 사용할 경우를 위해 강제로 1로 만들어준다
            if(ViewGame.main.modelRenders[1].color.a < 1f)
                ViewGame.main.modelRenders[1].color = new Color(1, 1, 1, 1);

            OnMoveCompleted();
            
            OffLipSync(); // 립싱크 OFF 추가 
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
        public void HideModel(int CharacterPosIndex, bool useSkip)
        {
            // 스킵으로 진행하거나, 가운데 캐릭터는 바로 비활성화 처리 
            // 가운데 캐릭터를 Fadeout 처리하면 다른 캐릭터가 등장할때 겹쳐..!
            if (useSkip || CharacterPosIndex == 1)
            {
                DisableModel();
                return;
            }

            // 어느 위치냐에 따라서 좌우로 이동하면서 퇴장 처리 
            if (CharacterPosIndex == 0)
                ExitCharacter(-width);
            else
                ExitCharacter(width);
        }

        /// <summary>
        /// 캐릭터 퇴장시키기. (움직임 처리)
        /// </summary>
        /// <param name="moveX">퇴장시키려는 X 좌표</param>
        void ExitCharacter(float moveX)
        {
            switch (out_effect)
            {
                case GameConst.INOUT_EFFECT_FADEOUT:
                    FadeOut();
                    break;
                default:
                    SlideOut(moveX);
                    break;
            }
        }

        /// <summary>
        /// 캐릭터 모델이 이동 시작하기 '전' 호출
        /// </summary>
        void OnMoveStart()
        {
            // 캐릭터 모델 연출이 시작되기 전에 누적이 되어 있는 dotween이 있을 수 있으니 kill해준다
            transform.DOKill();
            currRenderTexture.DOKill();

            moveComplete = false;
        }

        void OnMoveCompleted()
        {
            moveComplete = true;
        }

        /// <summary>
        /// 캐릭터 비활성화 (최종)
        /// </summary>
        void DisableModel()
        {
            gameObject.SetActive(false);

            if (currRenderTexture.color.a < 1f)
                currRenderTexture.color = new Color(1, 1, 1, 1);

            // layer가 Center가 아니면 center로 변경
            if (!gameObject.layer.Equals(LayerMask.NameToLayer(GameConst.LAYER_MODEL_C)))
                ChangeLayerRecursively(gameObject.transform, GameConst.LAYER_MODEL_C);

            // rawImage가 center세팅되어 있는 곳으로 안되있으면 변경
            if (!currRenderTexture.Equals(ViewGame.main.modelRenders[1]))
                SetCurRenderTexture(GameConst.LAYER_MODEL_C);

            // 퇴장했으니 다시 부모를 변경해준다
            currRenderTexture.transform.SetParent(ViewGame.main.modelRenderParents[0]);

            // scale 값이 1이 아니면 2인 스탠딩이었기 때문에 원상복귀
            if (!transform.localScale.y.Equals(1f))
            {
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
            }

            // 더미 캐릭터라면 dummyInfo 파괴해주자
            if (dummyInfo != null)
                Destroy(dummyInfo);

            OnMoveCompleted();
        }

        /// <summary>
        /// 캐릭터 키(tall) 세팅
        /// </summary>
        public void SetTall(int __grade)
        {
            // 제일 높은 키를 체크한다. 
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
            
            if(tallGrade < 0)
                tallGrade = 3;
        }
        
        
        /// <summary>
        /// 충돌체 설정하기 
        /// </summary>
        public void SetBoxColliders()
        {
            if (model == null) {
                Debug.LogError(this.originModelName + " is not created");
                return;
            }

            // 모든 drawables에 boxCollider 추가 
            for (int i = 0; i < model.Drawables.Length; i++)
                model.Drawables[i].gameObject.AddComponent<BoxCollider>();
        }
        
        
        
        /// <summary>
        /// 등장 효과에 따른 알파값 조정하기 
        /// </summary>
        void SetFadeInAlpha() {
            if(string.IsNullOrEmpty(in_effect))
                return;
                
            if(in_effect == GameConst.INOUT_EFFECT_FADEIN)
                fadeInAlpha = 1f;
            else if(in_effect == GameConst.INOUT_EFFECT_FADEIN50)
                fadeInAlpha = 0.72f;
        }
    }
}

