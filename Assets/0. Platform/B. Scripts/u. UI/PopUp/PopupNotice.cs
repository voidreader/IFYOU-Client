using System;
using UnityEngine;

using LitJson;

namespace PIERStory
{
    public class PopupNotice : PopupBase
    {
        public NoticeElement[] noticeElements;

        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();

            foreach (NoticeElement ne in noticeElements)
                ne.gameObject.SetActive(false);


            JsonData noticeList = SystemManager.main.noticeData;

            for (int i = 0; i < noticeList.Count; i++)
                noticeElements[i].InitNoticeBanner(noticeList[i]);
        }


        public void OnClilckClose()
        {
            SystemManager.appFirstExecute = false;
            Hide();
        }

        /// <summary>
        /// 오늘 더이상 열지 않기
        /// </summary>
        public void OnClickDonotOpen()
        {
            PlayerPrefs.SetString("noticeOneday", DateTime.Today.ToString());
            SystemManager.appFirstExecute = false;
            Hide();
        }
    }
}