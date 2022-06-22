using System.Globalization;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;
using LitJson;

namespace PIERStory
{
    public class PopupNotice : PopupBase
    {
        public static Action<JsonData, string> ShowNoticeDetail = null;

        [Space(15)]
        public NoticeElement[] noticeElements;
        public GameObject noticeList;
        public GameObject noticeDetail;

        [Space][Header("공지사항 상세")]
        public TextMeshProUGUI noticeDetailTitle;
        public TextMeshProUGUI noticeDate;

        public UnityEngine.UI.ScrollRect noticeDetailScroll;

        public RectTransform textContent;
        public TextMeshProUGUI noticeDetailText;

        public RectTransform imageContent;
        public ImageRequireDownload noticeDetailImage;

        const string DETAIL_BANNER_URL = "detail_banner_url";
        const string DETAIL_BANNER_KEY = "detail_banner_key";

        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            
            ShowNoticeDetail = SetNoticeDetail;

            foreach (NoticeElement ne in noticeElements)
                ne.gameObject.SetActive(false);

            JsonData noticeList = SystemManager.main.noticeData;

            for (int i = 0; i < noticeList.Count; i++)
                noticeElements[i].InitNoticeBanner(noticeList[i]);
                
            SystemManager.noticePopupExcuted = true;
        }


        public void OnClilckClose()
        {
            Hide();
        }

        public void OnClickBackButton()
        {
            if (!noticeDetail.activeSelf)
                return;

            noticeDetail.SetActive(false);
            StartCoroutine(WaitSomeFrame());
        }

        IEnumerator WaitSomeFrame()
        {
            yield return null;
            isBlockBackButton = false;
        }


        public void OnInputEscape(InputAction.CallbackContext context)
        {
            OnClickBackButton();
        }


        /// <summary>
        /// 오늘 더이상 열지 않기
        /// </summary>
        public void OnClickDonotOpen()
        {
            PlayerPrefs.SetString("noticeOneday", DateTime.Today.ToString());
            Hide();
        }

        /// <summary>
        /// 공지사항 상세 설정
        /// </summary>
        void SetNoticeDetail(JsonData detailData, string startDate)
        {
            noticeDetailTitle.text = SystemManager.GetJsonNodeString(detailData, LobbyConst.STORY_TITLE);
            noticeDate.text = !string.IsNullOrEmpty(startDate) ? string.Format("{0} {1}", DateTime.Parse(startDate, null, DateTimeStyles.RoundtripKind).ToString("f", new CultureInfo("en-US")), DateTime.Parse(startDate, null, DateTimeStyles.RoundtripKind).Kind) : string.Empty;
            SystemManager.SetArabicTextUI(noticeDate);

            string textContents = SystemManager.GetJsonNodeString(detailData, "contents");

            textContent.gameObject.SetActive(!string.IsNullOrEmpty(textContents));
            imageContent.gameObject.SetActive(string.IsNullOrEmpty(textContents));

            if (string.IsNullOrEmpty(textContents))
            {
                noticeDetailImage.OnDownloadImage = EnableNoticeDetail;
                noticeDetailImage.SetDownloadURL(SystemManager.GetJsonNodeString(detailData, DETAIL_BANNER_URL), SystemManager.GetJsonNodeString(detailData, DETAIL_BANNER_KEY), true);
                noticeDetailScroll.content = imageContent;
            }
            else
            {
                noticeDetailText.text = textContents;
                noticeDetailScroll.content = textContent;
                EnableNoticeDetail();
            }
        }

        void EnableNoticeDetail()
        {
            isBlockBackButton = true;
            noticeDetail.SetActive(true);
        }
    }
}