﻿#if UNITY_EDITOR || UNITY_ANDROID
namespace Toast.Gamebase.Internal.Mobile.Android
{
    public class AndroidGamebaseImageNotice : NativeGamebaseImageNotice
    {
        override protected void Init()
        {
            CLASS_NAME = "com.toast.android.gamebase.plugin.GamebaseImageNoticePlugin";
            messageSender = AndroidMessageSender.Instance;

            base.Init();
        }
    }
}
#endif
