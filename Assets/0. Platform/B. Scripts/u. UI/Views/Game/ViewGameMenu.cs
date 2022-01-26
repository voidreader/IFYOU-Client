using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Doozy.Runtime.UIManager.Components;
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
        public UIToggle autoPlayToggle;

        [Header("Division Free")]
        public GameObject retryButton;
        public GameObject blockRetryButton;
        public GameObject skipButton;
        public GameObject blockSkipButton;

        [Space(10)]
        public TextMeshProUGUI textTitle; // 타이틀 textMesh
        
        void Start() {
            
            
            // 배너 등장시에만 처리. 
            if(AdManager.main.isIronSourceBannerLoad) {
                // rect.
                footer.anchoredPosition = new Vector2(0, footer.anchoredPosition.y + 120);
            }
            
            
        }
        
        public override void OnStartView()
        {
            base.OnStartView();
            
            // 타이틀 처리 타입, 순번, 타이틀 조합
            textTitle.text = GameManager.main.currentEpisodeData.combinedEpisodeTitle;

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
            // * 이 조건일때, 나가면 튜토리얼이 중단된다고 경고해줘야 한다.  
            if (UserManager.main.tutorialStep == 2 && UserManager.main.tutorialFirstProjectID > 0)
            {
                // ! 여기서 나가면 강제 종료야. 튜토리얼이 끊긴다고..!
                SystemManager.ShowGamePopup(SystemManager.GetLocalizedText("80108"), GiveUpTutorial, null, true, false);
                return;
            }

            // QuitGame은 강제종료고 EndGame은 정상종료다.
            SystemManager.ShowGamePopup(SystemManager.GetLocalizedText("6037"), GameManager.main.QuitGame, null);
        }

        void GiveUpTutorial()
        {
            UserManager.main.UpdateTutorialStep(3);
            GameManager.main.QuitGame();
        }

        public void OnAutoPlay()
        {
            GameManager.main.StartAutoPlay();
            playButton.color = Color.white;
            playToggle.color = Color.white;
        }

        public void OffAutoPlay()
        {
            GameManager.main.StopAutoPlay();
            playButton.color = Color.grey;
            playToggle.color = Color.grey;
        }

        /// <summary>
        /// 처음부터 버튼 클릭 
        /// </summary>
        public void OnClickReplay()
        {
            SystemManager.ShowGamePopup(SystemManager.GetLocalizedText("6039"), GameManager.main.RetryPlay, null);
        }

        public void OnClickBlockReplay()
        {
            SystemManager.ShowSimpleAlertLocalize("6172");
        }
        
        public void OnClickBack() {
            
            viewGameMenu.Hide();
        }

        #endregion
    }
}
