using UnityEngine;

namespace PIERStory
{
    public class PopupRate : PopupBase
    {
        public override void Show()
        {
            base.Show();


        }

        public void OnClickRate()
        {
            UserManager.main.UpdateRateHistory(1);

#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=pier.make.story");
#elif UNITY_IOS
            Application.OpenURL("https://apps.apple.com/us/app/id1553733978");
#endif
            Hide();
        }

        public void OnClickClose()
        {
            UserManager.main.UpdateRateHistory(0);
            Hide();
        }
    }
}