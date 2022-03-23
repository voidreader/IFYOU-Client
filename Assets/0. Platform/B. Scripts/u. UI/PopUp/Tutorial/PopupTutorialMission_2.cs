using UnityEngine;

using TMPro;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory
{
    public class PopupTutorialMission_2 : PopupBase
    {
        [Space(15)][Header("이 팝업에서 사용되는 Components")]
        public UIContainer mainContainer;           // 기본
        public UIContainer premiumContainer;        // 프리미엄 패스 눌렀을 때 container
        public UIContainer helpContainer;           // 프리미엄 패스 세일 도움말 container
        public UIContainer freeContainer;           // freeplay 눌렀을 때 container

        [Header("프리미엄 패스 구매 Container에서 사용됨")]
        public ImageRequireDownload projectPassImage;

        public TextMeshProUGUI originalPriceText;
        public TextMeshProUGUI salePriceText;

        int originalPrice = 0;
        int salePrice = 0;

        [Header("FreePlay Container에서 사용됨")]
        public CanvasGroup noAdsGroup;


        public override void Show()
        {
            base.Show();
            originalPrice = StoryManager.main.GetProjectPremiumPassOriginPrice();
            salePrice = (int)(originalPrice * 0.25f);

            // 프리미엄 패스 이미지 세팅
            StoryData storyData = StoryManager.main.CurrentProject;
            projectPassImage.SetDownloadURL(storyData.premiumPassURL, storyData.premiumPassKey);

            // 가격 세팅
            originalPriceText.text = originalPrice.ToString();
            salePriceText.text = salePrice.ToString();

            premiumContainer.InstantHide();
            helpContainer.InstantHide();
            freeContainer.InstantHide();
            mainContainer.Show();
        }

        #region OnClick Event

        /// <summary>
        /// Special sale 버튼 클릭
        /// </summary>
        public void OnClickSpecialSale()
        {
            mainContainer.Hide();
            premiumContainer.Show();
        }

        /// <summary>
        /// Free play 버튼 클릭
        /// </summary>
        public void OnClickFreePlay()
        {
            // 여기서는 튜토리얼 미션 단계를 올려주고 보상 화면으로 넘어가야될 것 같다...


            mainContainer.Hide();
            freeContainer.Show();
        }

        /// <summary>
        /// 구매 버튼 클릭
        /// </summary>
        public void OnClickPassPurchase()
        {
            // 만약 할인 가격보다 스타가 없다면, 상점 팝업 띄워주기
            if (!UserManager.main.CheckGemProperty(salePrice))
            {
                SystemManager.ShowConnectingShopPopup(SystemManager.main.spriteStar, salePrice - UserManager.main.gem);
                return;
            }

            // 구매 후 처리 추가
            UserManager.OnFreepassPurchase = UpdateTutorialStep;
            NetworkLoader.main.PurchaseProjectFreepass(string.Empty, originalPrice, salePrice);
        }


        /// <summary>
        /// 튜토리얼 미션 완료 처리
        /// </summary>
        void UpdateTutorialStep()
        {
            Hide();
        }

        /// <summary>
        /// 할인 팝업 닫기버튼 클릭
        /// </summary>
        public void OnClickCloseSale()
        {
            premiumContainer.Hide();
            mainContainer.Show();
        }

        /// <summary>
        /// 도움말 버튼 클릭
        /// </summary>
        public void OnClickPassInformation()
        {
            helpContainer.Show();
        }


        #endregion

        /// <summary>
        /// Free play 버튼을 클릭해서 나온 container show Action
        /// </summary>
        public void ShowFreePlayReward()
        {
            noAdsGroup.GetComponent<RectTransform>().DOAnchorPosY(300, 1.2f);
            noAdsGroup.DOFade(1f, 1.2f).OnComplete(() =>
            {
                UpdateTutorialStep();
            });

        }
    }
}