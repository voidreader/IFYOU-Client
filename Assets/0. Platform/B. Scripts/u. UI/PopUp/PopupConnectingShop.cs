
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class PopupConnectingShop : PopupBase
    {
        public override void Show()
        {
            base.Show();

            if (Data.Images[0].rectTransform.sizeDelta.x < 100f || Data.Images[0].rectTransform.sizeDelta.y < 100f)
                Data.Images[0].rectTransform.sizeDelta = Data.Images[0].rectTransform.sizeDelta * 2f;
        }

        public void OnClickGoShopButton()
        {
            // 상점 열어주고
            Signal.Send(LobbyConst.STREAM_COMMON, "Shop", string.Empty);

            // 닫기~
            Hide();
        }
    }
}