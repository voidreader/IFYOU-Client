using System.Collections;
using UnityEngine;

using TMPro;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory
{
    public class PopupTutorialMission_3 : PopupBase
    {
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


        [Space(15)][Header("프리패스 구매 유저 Container")]
        public GameObject hasFreepass;

        public GameObject text2;
        public GameObject passRewardInfo;
        public TextMeshProUGUI rewardText3;

        public ImageRequireDownload passBadge;

        public GameObject text3;

        public override void Show()
        {
            base.Show();

            noneFreepass.SetActive(!UserManager.main.HasProjectFreepass());
            hasFreepass.SetActive(UserManager.main.HasProjectFreepass());
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
            yield return new WaitForSeconds(0.2f);
            mainContainer.Hide();
            reduceWaitingTimeContainer.Show();
            EpisodeEndControls.OnEpisodePlay?.Invoke();
        }

        public void OnClickReduceWaitingTime()
        {
            // 누르면 코인이 사용되는 연출이 일어나고, 실제 기다무를 풀어준다
            // 여기서는 튜토리얼 스텝 갱신을 해주고 통신이 완료되면 튜토리얼 미션 팝업을 종료 시켜주자
        }

        void CallbackTutorialUpdate()
        {
            Hide();
        }


        #endregion


        #region 프리패스 구매 유저 제어 Function

        /// <summary>
        /// 프리패스 가진 사람용 open버튼 클릭 액션
        /// </summary>
        public void OnClickPassVersionOpen()
        {
            text2.SetActive(false);
            rewardInfo.SetActive(false);
            text3.SetActive(false);

            passBadge.OnDownloadImage = null;
            passBadge.SetDownloadURL(StoryManager.main.freepassBadgeURL, StoryManager.main.freepassBadgeKey, true);
        }





        #endregion

    }
}