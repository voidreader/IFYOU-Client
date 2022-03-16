
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class PopupConnectingShop : PopupBase
    {
        public override void Show()
        {
            base.Show();

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