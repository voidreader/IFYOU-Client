using System;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIManager.Containers;


namespace PIERStory
{
    public class ViewGameMenu : CommonView
    {
        
        [SerializeField] RectTransform footer;
        [SerializeField] UIView viewGameMenu; // 게임메뉴. 

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

        [Header("Division Free")]
        public GameObject retryButton;
        public GameObject blockRetryButton;
        public GameObject skipButton;
        public GameObject blockSkipButton;

        [Space(10)]
        public TextMeshProUGUI textTitle; // 타이틀 textMesh
        
        void Start() {
            

        }
        
        public override void OnStartView()
        {
            base.OnStartView();
            
            // 타이틀 처리 타입, 순번, 타이틀 조합
            textTitle.text = GameManager.main.currentEpisodeData.combinedEpisodeTitle;

            
            // BM 변경으로 블락하지 않음. (2022.03.08)
            retryButton.SetActive(true);
            blockRetryButton.SetActive(false);
            skipButton.SetActive(true);
            blockSkipButton.SetActive(false);
            
            /*
            // 무료(광고) 플레이인 경우 보여주는 버튼을 아예 변경해준다
            if(GameManager.main.currentEpisodeData.purchaseState == PurchaseState.AD)
            {
                retryButton.SetActive(false);
                blockRetryButton.SetActive(true);
                skipButton.SetActive(false);
                blockSkipButton.SetActive(true);
            }
            else
            {
                retryButton.SetActive(true);
                blockRetryButton.SetActive(false);
                skipButton.SetActive(true);
                blockSkipButton.SetActive(false);
            }
            */
        }

        /// <summary>
        /// 스킵버튼의 icon을 변경해준다
        /// </summary>
        /// <param name="skipable">스킵이 가능한지의 여부</param>
        public void ChangeSkipIcon(bool skipable)
        {
            if (skipable)
                skipButtonIcon.sprite = ableSkip;
            else
                skipButtonIcon.sprite = disableSkip;

            skipButtonIcon.SetNativeSize();
        }

        #region OnClick Event


        /// <summary>
        /// 스킵 처리 
        /// </summary>
        public void SkipScene()
        {
            // 스킵이 가능하지 않으면 아무것도 실행하지 않는다.
            // 어드민 유저도 아니고 스킵도 가능하지 않으면 return. 
            if (!GameManager.main.skipable && !UserManager.main.CheckAdminUser() ) {
                // OnClickBlockSkip();
                return;
            }
            
            
            // 선택지 도중에서는 스킵할 수 없음.
            if(GameManager.main.isSelectionInputWait) {
                SystemManager.ShowMessageAlert(SystemManager.GetLocalizedText("6102"), true);
                return;    
            }
            
            // Doozy.Runtime.UIManager.Input.BackButton.Fire(); // 백버튼 발동처리
            
            viewGameMenu.Hide();
            

            // 시간 흐름중 스킵하면 시간흐름용 fadeImage를 비활성화 해버린다
            if (GameManager.main.currentRow.template.Equals(GameConst.TEMPLATE_FLOWTIME))
                ViewGame.main.fadeImage.gameObject.SetActive(false);

            if (GameManager.main.currentRow.template.Equals(GameConst.TEMPLATE_MOVEIN))
                ViewGame.main.placeTextBG.gameObject.SetActive(false);

            GameManager.main.useSkip = true;
            GameManager.main.isThreadHold = false;
            GameManager.main.isWaitingScreenTouch = false;
        }
        

        public void OnClickBlockSkip()
        {
            
            if(UserManager.main.CheckAdminUser()) {
                SkipScene();
                return;
            }
            
            SystemManager.ShowSimpleAlertLocalize("6171");
        }

        /// <summary>
        /// 로그 오픈 
        /// </summary>
        public void OpenLog()
        {
            viewGameMenu.Hide();
            ViewGame.main.ShowLog();
            
            // Doozy.Runtime.UIManager.Input.BackButton.Fire();
        }

        /// <summary>
        /// 인게임 메뉴 나가기 클릭 
        /// </summary>
        public void ExitGameByMenu()
        {
            // QuitGame은 강제종료고 EndGame은 정상종료다.
            SystemManager.ShowGamePopup(SystemManager.GetLocalizedText("6037"), GameManager.main.QuitGame, null);
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
            SystemManager.ShowGamePopup(SystemManager.GetLocalizedText("6039"), ProceedStartOver, null);
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
        

        public void OnClickBlockReplay()
        {
            SystemManager.ShowSimpleAlertLocalize("6172");
        }
        
        public void OnClickBack() {
            
            if(viewGameMenu.inTransition)
                return;
            
            viewGameMenu.Hide();
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
    }
}
