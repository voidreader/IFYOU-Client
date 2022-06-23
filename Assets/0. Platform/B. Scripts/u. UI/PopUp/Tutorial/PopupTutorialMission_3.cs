using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory
{
    public class PopupTutorialMission_3 : PopupBase
    {
        [Space(15)]
        public TextMeshProUGUI tutorialMissionText;

        [Header("프리패스 미구매 유저 Container")]
        public GameObject noneFreepass;

        public UIContainer mainContainer;
        public GameObject text1;
        public GameObject rewardInfo;
        public TextMeshProUGUI rewardText1;
        
        public CanvasGroup rewardCoin;

        [Space]
        public UIContainer reduceWaitingTimeContainer;
        public TextMeshProUGUI rewardText2;
        public TextMeshProUGUI needCoiinAmount;
        public Button coinOpenButton;

        [Space(15)][Header("프리패스 구매 유저 Container")]
        public GameObject hasFreepass;

        public GameObject text2;
        public GameObject passRewardInfo;
        public TextMeshProUGUI rewardText3;

        public ImageRequireDownload passBadge;

        public GameObject text3;
        public Button openButton;

        [Space(15)]
        public ParticleSystem coinFirecracker;

        public override void Show()
        {
            base.Show();

            SystemManager.SetText(tutorialMissionText, string.Format(SystemManager.GetLocalizedText("5167"), 3));
            SystemManager.SetText(rewardText1, string.Format(SystemManager.GetLocalizedText("5168"), Data.contentValue));

            needCoiinAmount.text = string.Format("{0}", Data.contentValue);
            
            SystemManager.SetText(rewardText2, string.Format(SystemManager.GetLocalizedText("5168"), 100));
            SystemManager.SetText(rewardText3, string.Format(SystemManager.GetLocalizedText("5168"), 100));

            noneFreepass.SetActive(!UserManager.main.HasProjectFreepass());
            hasFreepass.SetActive(UserManager.main.HasProjectFreepass());

            if(UserManager.main.HasProjectFreepass())
                passBadge.SetDownloadURL(StoryManager.main.freepassBadgeURL, StoryManager.main.freepassBadgeKey, true);
        }


        #region 프리패스 미구매 유저 제어 Function

        public void OnClickOpenButton()
        {
            // 상단의 CoinIndicator를 가져와서 현재 코인에서 기다무에 필요한 코인만큼을 얻는 연출(0.2초임)을 해주고
            rewardCoin.DOFade(1f, 0.1f);
            // contentValue에서 오픈하는데 필요한 코인수를 받아와야 한다
            // 이 팝업은 EpisodeControls. 즉 에피소드 종료 페이지에서 실행 될 것이다
            ViewCommonTop.OnForShowCoin?.Invoke(UserManager.main.coin + Data.contentValue);
            // reduceWaitingTimeContainer를 Show해준다
            StartCoroutine(WaitCoinShow());

        }

        IEnumerator WaitCoinShow()
        {
            yield return new WaitForSeconds(1.2f);
            mainContainer.Hide();
            reduceWaitingTimeContainer.Show();
            EpisodeEndControls.OnEpisodePlay?.Invoke();
        }

        public void OnClickReduceWaitingTime()
        {
            // 누르면 코인이 사용되는 연출이 일어나고, 실제 기다무를 풀어준다
            // 여기서는 튜토리얼 스텝 갱신을 해주고 통신이 완료되면 튜토리얼 미션 팝업을 종료 시켜주자
            JsonData j = new JsonData();

            j[CommonConst.COL_PROJECT_ID] = StoryManager.main.CurrentProjectID;
            j["price"] = 0;
            j[CommonConst.FUNC] = "requestWaitingEpisodeWithCoin";

            NetworkLoader.main.SendPost(CallbackTutroialRewardFreeReduce, j, true);
            coinOpenButton.interactable = false;
        }

        /// <summary>
        /// 튜토리얼로 기다무 해금
        /// </summary>
        void CallbackTutroialRewardFreeReduce(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackTutroialRewardFreeReduce");
                StoryLobbyMain.CallbackReduceWaitingTimeFail?.Invoke();
                coinOpenButton.interactable = true;
                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);
            
            SystemManager.ShowMessageWithLocalize("6220");
            UserManager.main.SetNodeUserProjectCurrent(result["projectCurrent"]);
            UserManager.main.SetBankInfo(result);   

            //에피소드 구매 기록 
            if (result.ContainsKey("episodePurchase"))
                UserManager.main.SetNodeEpisodePurchaseHistory(result[CommonConst.JSON_EPISODE_PURCHASE_HISTORY]);

            EpisodeEndControls.CallbackReduceWaitingTimeSuccess?.Invoke();

            ViewCommonTop.OnForShowCoin?.Invoke(UserManager.main.coin);
            UserManager.main.UpdateTutorialStep(3, 1, CallbackTutorialUpdate);
        }

        /// <summary>
        /// 기다무 해금 후 튜토리얼 업데이트 콜백
        /// </summary>
        void CallbackTutorialUpdate(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackTutorialUpdate, Tutorial Mission3");
                openButton.interactable = true;
                coinOpenButton.interactable = true;
                return;
            }

            coinFirecracker.Play(true);

            // 프리패스가 있는 경우
            if(UserManager.main.HasProjectFreepass())
            {
                text2.SetActive(false);
                rewardInfo.SetActive(false);
                text3.SetActive(false);

                passBadge.gameObject.SetActive(true);

                Sequence endTween = DOTween.Sequence();
                const float animTime = 1.5f;
                endTween.Append(passBadge.GetComponent<Image>().DOFade(0f, animTime).SetDelay(0.8f));

                RectTransform passRect = passBadge.GetComponent<RectTransform>();

                endTween.Join(passRect.DOAnchorPos(new Vector2(141, 281), animTime));
                endTween.Join(passRect.DOSizeDelta(new Vector2(passRect.sizeDelta.x * 0.3f, passRect.sizeDelta.y * 0.3f), animTime));
                endTween.onComplete = Hide;
            }
            else
            {
                StartCoroutine(WaitParticleStop());
            }
        }


        #endregion


        /// <summary>
        /// 프리패스 가진 사람용 open버튼 클릭 액션
        /// </summary>
        public void OnClickPassVersionOpen()
        {
            openButton.interactable = false;
            UserManager.main.UpdateTutorialStep(3, 1, CallbackTutorialUpdate);
        }

        IEnumerator WaitParticleStop()
        {
            yield return new WaitUntil(() => coinFirecracker.isStopped);
            Hide();
        }
    }
}