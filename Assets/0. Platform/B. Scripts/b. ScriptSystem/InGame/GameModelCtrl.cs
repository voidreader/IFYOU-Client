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

        bool autoPlay = false;                 // 자동진행 값. skip을 사용해도 true로 동일하게 간주한다
        public bool moveComplete = true;       // 등장, 퇴장 연출 완료 = true, 미완 = false


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
            modelType = CommonConst.MODEL_TYPE_LIVE2D;
            direction = dir;
            // 처음 그려지는 곳은 무조건 중앙이다.
            currRenderTexture = ViewGame.main.modelRenders[1];
        }

        /// <summary>
        /// 말하지 않는 스탠딩을 밀어주며, 스케일 값을 변경해준다
        /// </summary>
        /// <param name="targetIndex">캐릭터가 밀려나게 될 위치</param>
        public void PushListenerToSide(int targetIndex, bool autoPlay = false)
        {
            // currentRenderTexture는 현재 C를 사용하고 있기 떄문에 Talker에 있던 것을 EnterReady로 옮겨준다
            // 밑에서 Layer를 변경해주고, SetNonTalker가 선언되면서 Object의 부모가 변경되고, currentRenderTexture도 L or R로 변경될 것임
            currRenderTexture.transform.SetParent(ViewGame.main.modelRenderParents[0]);

            switch (targetIndex)
            {
                case 0:
                    if (autoPlay)
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
                    if (autoPlay)
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
        /// 모델 활성화 
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
                    //info = dummyInfo.GetComponent<DefaultCharacterInfo>();
                }

                // 따라다녀야 하니 transform 값도 전달해줘야 함
                //info.SetCharacterInfo(transform, row.speaker, row.character_expression);
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// 캐릭터 애니메이션 재생 
        /// </summary>
        /// <param name="__row"></param>
        /// <param name="characterPos"></param>
        public void PlayCubismAnimation(ScriptRow __row, int characterPos)
        {
            // 더미 캐릭터고 스탠딩 캐릭터고 여기에서 다 관리하기 떄문에 이 스크립트에서 자체적으로 SetActive true/false 해주면 된다. 이동 및 크기 변경도 마찬가지

            #region autoPlay value
            autoPlay = __row.autoplay_row < 1 ? false : true;

            if (GameManager.main.useSkip)
                autoPlay = true;
            #endregion

            // 모델이 비활성 상태일때 활성화가 되면 페이드인 처리를 한다. 
            if (!gameObject.activeSelf)
            {
                ActivateModel(__row);

                // 등장 연출과 퇴장 연출을 미리 받아둔다.
                in_effect = __row.in_effect;
                out_effect = __row.out_effect;

                EnterTalkingCharacter(characterPos);
            }
            else // 모델이 이미 화면에 나와있는 상태
            {
                // 2인 스탠딩중이면 talker니까 z 포지션을 앞으로 옮겨준다
                if (characterPos != 1)
                {
                    if (transform.localScale.x < 0f)
                        transform.DOScale(new Vector3(-1f, 1f, 1f) * talkerScale, animTime);
                    else
                        transform.DOScale(Vector3.one * talkerScale, animTime);
                    SetTalker();
                }
                else
                {
                    // 1인 스탠딩으로 변경되었으니 원상복귀
                    // 스킵중엔 연출 무효
                    if (GameManager.main.useSkip)
                    {
                        transform.localPosition = Vector3.zero;
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
                Debug.Log(string.Format("WARNING! : {0} doesn't exists in {1}", __row.character_expression, __row.speaker));
                return;
            }


            // 더미 데이터는 애니메이션이 없으니 그만
            if (model == null)
            {
                // character_expression이 변경되었을 수도 있으니까 변경해줘야 해
                //info.SetCharacterInfo(transform, __row.speaker, __row.character_expression);
                return;
            }

            // 퇴장 연출을 hide 전에 변경 하면 변경한 것에 대해 적용한다.
            // 등장 연출은 등장시에만 발동하기 때문에 이미 스크린에 있는 경우에는 무시한다.
            if (!string.IsNullOrEmpty(__row.out_effect))
                out_effect = __row.out_effect;


            // 클립 재생 
            modelAnim.CrossFade(__row.character_expression, 0.3f);
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

            SetTalker();
        }

        /// <summary>
        /// 등장하는 캐릭터 scale,pos값 조정
        /// </summary>
        /// <param name="__dir">방향</param>
        /// <param name="posX">옮겨질 위치</param>
        void SetCharacterTransform(string __dir, float posX)
        {
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
            }
            else
            {
                if(string.IsNullOrEmpty(__dir))
                    MoveCharacterToTargetPosition(0f, false);
                else
                {
                    // 시선 방향과 위치가 동향이면 뒤집기
                    if (direction.Equals(__dir))
                        MoveCharacterToTargetPosition(posX, true);
                    else
                        MoveCharacterToTargetPosition(posX, false);
                }
            }

            if (transform.localScale.x < 0f)
                transform.DOScale(new Vector3(-1f, 1f, 1f) * talkerScale, animTime);
            else
                transform.DOScale(Vector3.one * talkerScale, animTime);
        }

        /// <summary>
        /// 등장할 캐릭터를 목표한 위치로 이동. 
        /// </summary>
        /// <param name="moveX">이동할 위치</param>
        /// <param name="flip">좌우 반전 - 방향</param>
        void MoveCharacterToTargetPosition(float moveX, bool flip)
        {
            #region flip 처리 
            // Center는 moveX가 0f이다.
            if (moveX == 0)
            {
                if (flip)
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                else
                    transform.localScale = Vector3.one;
            }
            else
            {
                if (flip)
                    transform.localScale = new Vector3(-1f, 1f, 1f) * talkerScale;
                else
                    transform.localScale = Vector3.one * talkerScale;
            }
            #endregion

            if (moveX.Equals(0f)) // 가운데 등장 캐릭터는 무조건 페이드인이야.
            {
                in_effect = GameConst.INOUT_EFFECT_FADEIN;
                FadeIn(moveX, GameConst.LAYER_MODEL_C);
            }
            else // 좌우 캐릭터 처리 
            {
                // 왼쪽에서 등장
                if (moveX < 0f)
                    EnterCharacterDirection(-width, moveX, GameConst.LAYER_MODEL_L);
                else
                    EnterCharacterDirection(width, moveX, GameConst.LAYER_MODEL_R);

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
        }

        /// <summary>
        /// 페이드인 등장 연출
        /// </summary>
        /// <param name="posX">등장 x 좌표값</param>
        /// <param name="layerName"></param>
        void FadeIn(float posX, string layerName)
        {

            // 여기에 OnMoveStart 넣으면 오류는 해결되지만, 답답해져서.. 다시 뺌. 
            // OnMoveStart();                      

            // 바꾸려는 레이어와 같지 않은 경우에만 변경한다
            if (!gameObject.layer.Equals(LayerMask.NameToLayer(layerName)))
                ChangeLayerRecursively(transform, layerName);

            SetCurRenderTexture(layerName);
            transform.localPosition = new Vector3(posX, transform.localPosition.y, 0);
            currRenderTexture.color = new Color(1, 1, 1, 0);

            currRenderTexture.DOFade(1f, fadeTime).OnComplete(OnMoveCompleted);
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
            currRenderTexture.transform.SetParent(ViewGame.main.modelRenderParents[2]);
        }

        /// <summary>
        /// 2인 스탠딩 중, 캐릭터를 '듣는' 상태로 설정. 
        /// </summary>
        public void SetListener()
        {
            // 리스터는 살짝 작은 스케일로 조정해준다. 

            if (transform.localScale.x < 0f)
                transform.DOScale(new Vector3(-1f, 1f, 1f) * listenerScale, animTime);
            else
                transform.DOScale(listenerScale, animTime);

            currRenderTexture.transform.SetParent(ViewGame.main.modelRenderParents[1]);
            OnMoveCompleted();
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

