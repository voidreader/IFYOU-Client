using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

using TMPro;
using LitJson;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory
{
    public class GameMenu : CommonView
    {
        
        [SerializeField] RectTransform footer;
        UIContainer menuContainer;
        PopupBase gameOptionPopup;
        public RectTransform gameMenuTop;
        public GameObject replayButton;

        [Header("Skip")]
        public Image skipButtonIcon;
        public Sprite ableSkip;    // 스킵버튼 사용 가능 sprite
        public Sprite disableSkip;  // 스킵버튼 사용 불가능 sprite

        [Header("AutoPlay")]
        public Image playButton;
        public Image playToggle;
        public UIAnimator autoPlayToggleAnimator;

        public Sprite spritePlay;
        public Sprite spritePlayInactive;
        public Sprite spriteToggleOn;
        public Sprite spriteToggleOff;


        [Space(10)]
        public TextMeshProUGUI textTitle; // 타이틀 textMesh

        private void Start()
        {
            menuContainer = GetComponent<UIContainer>();
        }

        void Update() {
            if(Input.GetKeyDown(KeyCode.S)) {
                SkipScene();
            }
        }
        
        
        public override void OnStartView()
        {
            base.OnStartView();
            
            // 타이틀 처리 타입, 순번, 타이틀 조합
            SystemManager.SetText(textTitle, GameManager.main.currentEpisodeData.combinedEpisodeTitle);

            // 화면에 상단바가 표출중인 경우 아래로 80만큼 내린다
            if (ViewGame.main.commonTop.isVisible || ViewGame.main.commonTop.isShowing)
                gameMenuTop.anchoredPosition = new Vector2(0, -80f);
            else
                gameMenuTop.anchoredPosition = Vector2.zero;
        }
        
        public override void OnHideView() {
            base.OnHideView();
        }
        

        /// <summary>
        /// 스킵버튼의 icon을 변경해준다
        /// </summary>
        /// <param name="skipable">스킵이 가능한지의 여부</param>
        public void ChangeSkipIcon(bool skipable)
        {
            
            if(UserManager.main.HasProjectFreepass()) {
                skipButtonIcon.sprite = ableSkip;
            }
            else {
                if (skipable)
                    skipButtonIcon.sprite = ableSkip;
                else
                    skipButtonIcon.sprite = disableSkip;    
            }
            
            

            skipButtonIcon.SetNativeSize();
        }

        #region OnClick Event

        /// <summary>
        /// 게임 옵션 열기
        /// </summary>
        public void OnClickOpenGameOption()
        {
            gameOptionPopup = PopupManager.main.GetPopup(GameConst.POPUP_GAME_OPTION);

            if(gameOptionPopup == null)
            {
                Debug.LogError("게임 옵션 팝업 존재 안함!!!");
                return;
            }

            PopupManager.main.ShowPopup(gameOptionPopup, false);
        }


        /// <summary>
        /// 스킵 처리 
        /// </summary>
        public void SkipScene()
        {
            // 스킵이 가능하지 않으면 아무것도 실행하지 않는다.
            // 어드민 유저도 아니고 스킵도 가능하지 않으면 return. 
            
            // 2022.08.01 프리미엄 패스 유저는 자유로운 스킵이 가능.
            if(!UserManager.main.HasProjectFreepass()) {
            
                if (!GameManager.main.skipable && !UserManager.main.CheckAdminUser() ) {
                    // OnClickBlockSkip();
                    return;
                }
            }


            // 선택지 도중에서는 스킵할 수 없음.
            if (GameManager.main.isSelectionInputWait)
            {
                SystemManager.ShowMessageWithLocalize("6102");
                return;
            }

            Debug.Log("## SkipScene ##");


            // 시간 흐름중 스킵하면 시간흐름용 fadeImage를 비활성화 해버린다
            if (GameManager.main.currentRow.template.Equals(GameConst.TEMPLATE_FLOWTIME))
                ViewGame.main.fadeImage.gameObject.SetActive(false);

            if (GameManager.main.currentRow.template.Equals(GameConst.TEMPLATE_MOVEIN))
                ViewGame.main.placeTextBG.gameObject.SetActive(false);

            GameManager.main.useSkip = true;
            GameManager.main.isThreadHold = false;
            GameManager.main.isWaitingScreenTouch = false;

            ViewGame.main.isVibrate = false;

            menuContainer.Hide();
        }
        

        /// <summary>
        /// 로그 오픈 
        /// </summary>
        public void OpenLog()
        {
            menuContainer.Hide();
            ViewGame.main.ShowLog();
        }

        /// <summary>
        /// 인게임 메뉴 나가기 클릭 
        /// </summary>
        public void ExitGameByMenu()
        {
            // QuitGame은 강제종료고 EndGame은 정상종료다.
            SystemManager.ShowSystemPopupLocalize("6037", GameManager.main.QuitGame, null, false);
        }

        public void OnClickAutoPlay()
        {
            if(GameManager.main.isAutoPlay)
                StopAutoPlay();
            else
            {
                GameManager.main.isAutoPlay = true;
                playButton.sprite = spritePlay;
                playToggle.sprite = spriteToggleOn;
                autoPlayToggleAnimator.Play();
            }
        }


        /// <summary>
        /// 처음부터 버튼 클릭 
        /// </summary>
        public void OnClickReplay()
        {
            SystemManager.ShowSystemPopupLocalize("6039", ProceedStartOver, null);
        }
        
        
        /// <summary>
        /// 처음으로 돌아가기 할때 서버통신으로 통해 현재 회차의 진행도 제거. 
        /// </summary>
        void ProceedStartOver() {
            JsonData sendingData = new JsonData();
            sendingData["func"] = "resetPlayingEpisode";
            sendingData["project_id"] = StoryManager.main.CurrentProjectID;
            sendingData["episode_id"] = StoryManager.main.CurrentEpisodeID;
            
            NetworkLoader.main.SendPost(UserManager.main.CallbackStartOverEpisode, sendingData, true);
        }
        

        public void StopAutoPlay()
        {
            GameManager.main.isAutoPlay = false;
            GameManager.main.flowTime = 0f;
            playButton.sprite = spritePlayInactive;
            playToggle.sprite = spriteToggleOff;
            autoPlayToggleAnimator.Play(true);
        }

        #endregion

        public void OnInputEscape(InputAction.CallbackContext context)
        {
            if (gameOptionPopup != null && gameOptionPopup.isShow)
                return;

            if (menuContainer.isHidden || menuContainer.isHiding)
                return;

            menuContainer.Hide();
        }
        
        
        /// <summary>
        /// 개발전용 점프 팝업 
        /// </summary>
        public void OpenDevRowJump() {
            
            if(!SystemManager.main.isTestServerAccess) {
                return;
            }
            
            PopupBase p = PopupManager.main.GetPopup(CommonConst.POPUP_DEV_JUMP);
            PopupManager.main.ShowPopup(p, true);
        }
    }
}
