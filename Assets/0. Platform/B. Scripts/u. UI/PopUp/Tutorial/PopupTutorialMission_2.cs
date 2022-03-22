
using TMPro;
using Doozy.Runtime.UIManager.Containers;

namespace PIERStory
{
    public class PopupTutorialMission_2 : PopupBase
    {
        public UIContainer mainContainer;           // 기본
        public UIContainer premiumContainer;        // 프리미엄 패스 눌렀을 때 container
        public UIContainer helpContainer;           // 프리미엄 패스 세일 도움말 container
        public UIContainer freeContainer;           // freeplay 눌렀을 때 container

        public TextMeshProUGUI originalPriceText;
        public TextMeshProUGUI salePriceText;

        int originalPrice = 0;
        int salePrice = 0;


        public override void Show()
        {
            base.Show();
            originalPrice = StoryManager.main.GetProjectPremiumPassOriginPrice();
            salePrice = (int)(originalPrice * 0.25f);

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
            UserManager.OnFreepassPurchase = CallbackFreepassPurcahse;
            NetworkLoader.main.PurchaseProjectFreepass(string.Empty, originalPrice, salePrice);
        }

        void CallbackFreepassPurcahse()
        {
            // 구매를 했든, 프리 플레이를 했든 아무튼 튜토리얼 단계 레벨업이 필요함
            // 구매했으니 
        }


        void UpdateTutorialStep()
        {

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

        }

    }
}