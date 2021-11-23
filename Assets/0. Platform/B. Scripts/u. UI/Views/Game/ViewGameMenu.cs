using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class ViewGameMenu : CommonView
    {
        JsonData episodeJSON = null; // 현재 실행중인 에피소드 JSON 

        [Header("Skip")]
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
        string episodeTitle = string.Empty;
        string episodeType = string.Empty;

        public override void OnView()
        {
            base.OnView();

            episodeJSON = GameManager.main.currentEpisodeJson;

            episodeType = SystemManager.GetJsonNodeString(episodeJSON, CommonConst.COL_EPISODE_TYPE);

            // 엔딩과 일반 에피소드 다르게 타이틀 처리 
            try
            {
                switch (episodeType)
                {
                    case CommonConst.COL_CHAPTER:
                        episodeTitle = string.Format("Episode {0}. {1}", SystemManager.GetJsonNodeString(episodeJSON, CommonConst.COL_EPISODE_NO), SystemManager.GetJsonNodeString(episodeJSON, CommonConst.COL_TITLE));
                        break;

                    default:
                        episodeTitle = string.Format("{0}", SystemManager.GetJsonNodeString(episodeJSON, CommonConst.COL_TITLE));
                        break;
                }

                textTitle.text = episodeTitle;
            }
            catch (UnityException e)
            {
                textTitle.text = SystemManager.GetLocalizedText("80080");
            }

            SetAutoPlayButtonSprite();
        }

        public override void OnStartView()
        {
            base.OnStartView();
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
                skipButtonIcon.sprite = ableSkip;
                skipButtonIcon.SetNativeSize();
            }
            else
            {
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

            SystemManager.ShowConfirmPopUp(SystemManager.GetLocalizedText("6037"), GameManager.main.EndGame, null);
        }

        void StopAutoPlay()
        {
            GameManager.main.isAutoPlay = false;
            StopCoroutine(GameManager.main.routineAutoPlay);
            SetAutoPlayButtonSprite();
        }

        public void AutoPlay()
        {
            GameManager.main.isAutoPlay = true;
            StartCoroutine(GameManager.main.routineAutoPlay);
            SetAutoPlayButtonSprite();
        }

        public void OnClickReplay()
        {
            StopAutoPlay();

            JsonData purchaseData = null;
            UserManager.main.CheckPurchaseEpisode(SystemManager.GetJsonNodeString(episodeJSON, CommonConst.COL_EPISODE_ID), ref purchaseData);

            if (SystemManager.GetJsonNodeString(purchaseData, CommonConst.COL_PURCHASE_TYPE).Equals("OneTime") && !UserManager.main.CheckAdminUser())
            {
                SystemManager.ShowAlertWithLocalize("6038");
                return;
            }

            SystemManager.ShowConfirmPopUp(SystemManager.GetLocalizedText("6039"), GameManager.main.RetryPlay, null);
        }

        #endregion
    }
}
