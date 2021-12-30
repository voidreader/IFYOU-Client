using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Doozy.Runtime.UIManager.Components;

namespace PIERStory
{
    public class ViewGameMenu : CommonView
    {

        [Header("Skip")]
        public Button skipButton;
        public Image skipButtonIcon;
        public Sprite ableSkip;    // 스킵버튼 사용 가능 sprite
        public Sprite disableSkip;  // 스킵버튼 사용 불가능 sprite

        [Header("AutoPlay")]
        public Image playButton;
        public Image playToggle;
        public Sprite playOnImage;
        public Sprite playOffImage;
        public Sprite playToggleOnImage;
        public Sprite playToggleOffImage;


        [Space(10)]
        public TextMeshProUGUI textTitle; // 타이틀 textMesh
        
        public override void OnStartView()
        {
            base.OnStartView();

            // 타이틀 처리 타입, 순번, 타이틀 조합
            textTitle.text = GameManager.main.currentEpisodeData.combinedEpisodeTitle;

            SetAutoPlayButtonSprite();
        }

        void SetAutoPlayButtonSprite()
        {
            // 자동재생 값에 대한 sprite 변화
            if (GameManager.main.isAutoPlay)
            {
                playButton.sprite = playOnImage;
                playToggle.sprite = playToggleOnImage;
            }
            else
            {
                playButton.sprite = playOffImage;
                playToggle.sprite = playToggleOffImage;
            }
        }

        /// <summary>
        /// 스킵버튼의 icon을 변경해준다
        /// </summary>
        /// <param name="skipable">스킵이 가능한지의 여부</param>
        public void ChangeSkipIcon(bool skipable)
        {
            if (skipable)
            {
                skipButton.interactable = true;
                skipButtonIcon.sprite = ableSkip;
                skipButtonIcon.SetNativeSize();
            }
            else
            {
                skipButton.interactable = false;
                skipButtonIcon.sprite = disableSkip;
                skipButtonIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 27);
            }
        }

        #region OnClick Event

        /// <summary>
        /// 스킵 처리 
        /// </summary>
        public void SkipScene()
        {
            // AD 무료플레이에서는 스킵 할 수 없도록 한다.
            if(GameManager.main.currentEpisodeData.purchaseState == PurchaseState.AD) {
                SystemManager.ShowAlert("무료 플레이에서는 스킵을 사용할 수 없습니다.");
                return;
            }
            
            
            
            // 스킵이 가능하지 않으면 아무것도 실행하지 않는다.
            if (!GameManager.main.skipable)
                return;

            // 시간 흐름중 스킵하면 시간흐름용 fadeImage를 비활성화 해버린다
            if (GameManager.main.currentRow.template.Equals(GameConst.TEMPLATE_FLOWTIME))
                ViewGame.main.fadeImage.gameObject.SetActive(false);

            GameManager.main.useSkip = true;
            GameManager.main.isThreadHold = false;
            GameManager.main.isWaitingScreenTouch = false;
        }

        /// <summary>
        /// 로그 오픈 
        /// </summary>
        public void OpenLog()
        {
            ViewGame.main.ShowLog();
            StopAutoPlay();
            
            Doozy.Runtime.UIManager.Input.BackButton.Fire();
        }

        /// <summary>
        /// 인게임 메뉴 나가기 클릭 
        /// </summary>
        public void ExitGameByMenu()
        {
            StopAutoPlay();

            // 이 창을 띄우는 시점에 저장을 해둔다. 
            // EndGame이 호출되면 Game씬에서 빠져나가기 때문에 오류 발생
            GameManager.main.SaveCurrentPlay();

            // 팝업에 대해 뭔가 정해질 때까지 묻지 않고 그냥 종료
            //SystemManager.ShowConfirmPopUp(SystemManager.GetLocalizedText("6037"), GameManager.main.EndGame, null);
            GameManager.main.EndGame();
        }

        void StopAutoPlay()
        {
            GameManager.main.isAutoPlay = false;
            GameManager.main.StopAutoPlay();
            SetAutoPlayButtonSprite();
        }


        /// <summary>
        /// 오토플레이 버튼 클릭 
        /// </summary>
        public void AutoPlay()
        {
            
            
            GameManager.main.isAutoPlay = !GameManager.main.isAutoPlay;
            
            if(GameManager.main.isAutoPlay) {
                GameManager.main.StartAutoPlay();    
                SetAutoPlayButtonSprite();
            }
            else {
                StopAutoPlay();
            }
            
            
            
        }

        /// <summary>
        /// 처음부터 버튼 클릭 
        /// </summary>
        public void OnClickReplay()
        {
            StopAutoPlay();
            GameManager.main.RetryPlay();

            // 1회권 유저는 처음부터 불가하다.             
            if (GameManager.main.currentEpisodeData.purchaseState == PurchaseState.OneTime) {
                SystemManager.ShowAlertWithLocalize("6038");
                return; 
            }

            // 팝업이 결정될 때까진 걍 재시작
            //SystemManager.ShowConfirmPopUp(SystemManager.GetLocalizedText("6039"), GameManager.main.RetryPlay, null);
        }

        #endregion
    }
}
