
using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewAddMenu : CommonView
    {
        public override void OnView()
        {
            base.OnView();
        }

        public override void OnStartView()
        {
            Signal.Send(LobbyConst.STREAM_IFYOU, "activateAddMenu", string.Empty);
        }
    }
}
