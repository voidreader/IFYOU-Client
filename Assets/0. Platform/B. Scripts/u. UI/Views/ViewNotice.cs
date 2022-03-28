﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.Signals;
using LitJson;

namespace PIERStory
{
    public class ViewNotice : CommonView
    {
        public NoticeElement[] noticeElements;
        
        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SAVE_STATE, string.Empty);
            StartCoroutine(DelaySendingSignal());

            foreach (NoticeElement ne in noticeElements)
                ne.gameObject.SetActive(false);

            JsonData noticeList = SystemManager.main.noticeData;

            for (int i = 0; i < noticeList.Count; i++)
                noticeElements[i].InitNoticeBanner(noticeList[i], true);
        }

        public override void OnView()
        {
            base.OnView();




        }

        public override void OnHideView()
        {
            base.OnHideView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_RECOVER, string.Empty);
        }
        
        IEnumerator DelaySendingSignal() {
            yield return null;
            yield return null;
            yield return null;
            
            
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5001"), string.Empty);
            
            
        }        
    }
}
