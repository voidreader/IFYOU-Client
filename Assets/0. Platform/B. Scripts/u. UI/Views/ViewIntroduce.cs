using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;

namespace PIERStory {
    public class ViewIntroduce : CommonView
    {
        public Doozy.Runtime.UIManager.Containers.UIContainer container;

        [Space(15)]
        public GameObject onedayPass;       // 원데이패스 관련 묶음 Object
        public GameObject onedayPurchaseButton;
        public GameObject onedayBadgeButton;
        public TextMeshProUGUI onedayPassTimer;

        public GameObject premiumPurchaseButton;
        public ImageRequireDownload premiumBadgeButton;

        public Button buttonAlert;          // 작품 알림 버튼
        public Sprite spriteAlertOff;       // 작품 알림 버튼 Off Sprite
        public Sprite spriteAlertOn;        // 작품 알림 버튼 On Sprite

        public Button btnLike;              // 좋아요 버튼
        public Sprite spriteLikeOff;        // 좋아요 버튼 OFF 스프라이트
        public Sprite spriteLikeOn;         // 좋아요 버튼 ON 스프라이트

        [SerializeField] TextMeshProUGUI textRecommend;     // 추천 작 안내

        [Header("소개 메인")][Space(15)]
        public ImageRequireDownload mainThumbnail;          // 메인 썸네일
        public GameObject viewCountTag;
        public TextMeshProUGUI viewCountText;
        public GameObject likeCountTag;
        public TextMeshProUGUI likeCountText;
        public GameObject newTag;

        public TextMeshProUGUI textTitle;               // 타이틀
        public TextMeshProUGUI productInfo;             // 작품 제작 정보


        [Header("작품 소개 상세")][Space(15)]
        public ImageRequireDownload introduceThumbnail;     // 서브 섬네일

        public GameObject completeStoryTag;
        public GameObject updateStateGroup;
        public GameObject updateDate_1;
        public TextMeshProUGUI updateDateText_1;
        public GameObject updateDate_2;
        public TextMeshProUGUI updateDateText_2;
        public TextMeshProUGUI totalChapterCountText;


        public TextMeshProUGUI introduceStoryTitleText;
        public TextMeshProUGUI storySummaryText;
        public Transform hashTagsParent;
        public GameObject hashTagPrefab;
        List<GameObject> createdHashtag = new List<GameObject>();

        public GameObject serialGroup; // 연재 관련 오브젝트 
        public TextMeshProUGUI textSerialDay; // 연재 정보 
        
        public StoryData introduceStory;
        
        public override void OnView() {
            base.OnView();
        }
        
        public override void OnStartView() {
            base.OnStartView();
        
            textRecommend.gameObject.SetActive(false);    
            SetInfo();
            InitOnedayPass();
        }

        public void SetInfo(StoryData introduceStoryData = null) {
            
            introduceStory = introduceStoryData == null ? SystemListener.main.introduceStory : introduceStoryData;
            
            if(string.IsNullOrEmpty(introduceStory.projectID))
                return;

            SetLikeButtonState();
            SetAlertButtonState();

            // 이미지를 프리미엄 패스 이미지와 동일한 이미지를 사용한다.
            mainThumbnail.SetDownloadURL(introduceStory.premiumPassURL, introduceStory.premiumPassKey);

            int viewCount = introduceStory.hitCount * 10, likeCount = introduceStory.likeCount * 10;

            viewCountTag.SetActive(viewCount >= 100);
            likeCountTag.SetActive(likeCount >= 100);
            newTag.SetActive(!viewCountTag.activeSelf && !likeCountTag.activeSelf);

            if (viewCountTag.activeSelf)
                viewCountText.text = AbbrevationUtility.FormatNumberFirstDecimalPlace(viewCount);

            if(likeCountTag.activeSelf)
                likeCountText.text = AbbrevationUtility.FormatNumberFirstDecimalPlace(likeCount);

            SystemManager.SetText(textTitle, introduceStory.title);

            string originText = SystemManager.GetLocalizedText("6179") + " / " + introduceStory.original;
            string productText = SystemManager.GetLocalizedText("6180") + " / " + introduceStory.writer;
            string translateText = string.Empty;
            string productInfoText = string.IsNullOrEmpty(translateText) ? string.Format("{0}\n{1}", originText, productText) : string.Format("{0}\n{1}\n{2}", originText, productText, translateText);

            productInfo.rectTransform.sizeDelta = string.IsNullOrEmpty(translateText) ? new Vector2(productInfo.rectTransform.sizeDelta.x, 60) : new Vector2(productInfo.rectTransform.sizeDelta.x, 90);
            SystemManager.SetText(productInfo, productInfoText);

            //SystemManager.SetText(textSummary, introduceStory.summary); // 요약

            //SystemManager.SetText(textGenre, SystemManager.GetLocalizedText("6181") + " / " + introduceStory.genre); // 장르


            // serialGroup.SetActive(introduceStory.isSerial);
            /*
            serialGroup.SetActive(true);
            
            if(introduceStory.isSerial)
                SystemManager.SetText(textSerialDay, string.Format(SystemManager.GetLocalizedText("5184"), introduceStory.GetSeiralDay())); // 연재일 설정..
            else 
                SystemManager.SetLocalizedText(textSerialDay, "5186");// 완결 
            */

            completeStoryTag.SetActive(!introduceStory.isSerial);
            updateStateGroup.SetActive(introduceStory.isSerial);

            if (introduceStory.isSerial)
                SystemManager.SetText(updateDateText_1, introduceStory.GetSeiralDay());


            // 작품 세부 정보
            introduceThumbnail.SetDownloadURL(introduceStory.premiumPassURL, introduceStory.premiumPassKey);
            SystemManager.SetText(introduceStoryTitleText, introduceStory.title);
            SystemManager.SetText(storySummaryText, introduceStory.summary);

            // list 초기화
            foreach (GameObject g in createdHashtag)
                Destroy(g);

            createdHashtag.Clear();

            // 작품 해시 태그
            for (int i = 0; i < introduceStory.arrHashtag.Length; i++)
            {
                StoryHashtag hashtag = Instantiate(hashTagPrefab, hashTagsParent).GetComponent<StoryHashtag>();
                hashtag.Init(introduceStory.arrHashtag[i]);
                createdHashtag.Add(hashtag.gameObject);
            }

            // 인트로에서 넘어온 경우에 대한 처리 추가 
            if (SystemListener.main.isIntroduceRecommended) {
                
                SystemManager.SetLocalizedText(textRecommend, "6289");
                textRecommend.gameObject.SetActive(true);
                SystemListener.main.isIntroduceRecommended = false;
            }

            StartCoroutine(LayoutRebuild());
        }

        IEnumerator LayoutRebuild()
        {
            yield return null;
            viewCountTag.SetActive(false);
            likeCountTag.SetActive(false);
            hashTagsParent.gameObject.SetActive(false);
            yield return null;
            viewCountTag.SetActive(true);
            likeCountTag.SetActive(true);
            hashTagsParent.gameObject.SetActive(true);

        }
        
        
        public void OnClickStart() {
            if(introduceStory.isLock) {
                SystemManager.ShowMessageWithLocalize("6061");
                return;
            }
            
            // 트윈 동작중에 클릭되지 않게. 
            if(container.inTransition)
                return;
            
            // 스토리매니저에게 작품 상세정보 요청 
            StoryManager.main.RequestStoryInfo(introduceStory);
            
        }

        #region 원데이 패스 관련

        void InitOnedayPass()
        {
            onedayPurchaseButton.SetActive(string.IsNullOrEmpty(introduceStory.onedayExpireDate));
            onedayBadgeButton.SetActive(!string.IsNullOrEmpty(introduceStory.onedayExpireDate) && introduceStory.IsValidOnedayPass());
            onedayPass.SetActive(onedayPurchaseButton.activeSelf || onedayBadgeButton.activeSelf);
        }


        public void OpenOnedayPassPopup()
        {

        }


        #endregion

        #region 프리미엄 패스 관련

        public void OpenPremiumPassPopup()
        {

        }


        #endregion


        #region 좋아요 버튼 관련 메소드

        /// <summary>
        /// 좋아요 버튼 세팅
        /// </summary>
        void SetLikeButtonState()
        {
            if (StoryManager.main.CheckProjectLike(introduceStory.projectID))
                btnLike.image.sprite = spriteLikeOn;
            else
                btnLike.image.sprite = spriteLikeOff;
        }
        
        /// <summary>
        ///  좋아요 버튼 클릭
        /// </summary>
        public void OnClickLikeButton()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "updateProjectLike";
            sending["project_id"] = introduceStory.projectID;

            NetworkLoader.main.SendPost(OnProjectLike, sending, true);
        }

        void OnProjectLike(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkLoader.CheckResponseValidation(request, response))
                return;

            Debug.Log("OnProjectLike : " + response.DataAsText);

            // 서버에서 likeID 통으로 응답받는다.
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            StoryManager.main.SetLikeStoryData(result["like"]); // 리스트 갱신. 

            // 갱신된 정보를 버튼에 반영 
            SetLikeButtonState();

            // 눌렀을 때만 Alert popup이 뜨도록 수정
            if (StoryManager.main.CheckProjectLike(introduceStory.projectID))
                SystemManager.ShowMessageWithLocalize("6188");
            else
                SystemManager.ShowMessageWithLocalize("6189");
                
            
            
            introduceStory.isNotify = SystemManager.GetJsonNodeBool(result, "is_notify");
            StoryManager.main.FindProject(introduceStory.projectID).isNotify = introduceStory.isNotify;
            
            Debug.Log("Notification Reset : " + introduceStory.isNotify);
            
            // 갱신된 알림 버튼 세팅 
            SetAlertButtonState();
        }

        #endregion


        #region 작품 알림 버튼 관련 메소드

        /// <summary>
        /// 작품 알림 이미지 세팅
        /// </summary>
        void SetAlertButtonState()
        {
            // 프로젝트 알림설정 기능 사용여부 추가
            if(!SystemManager.main.useProjectNotify)
            {
                buttonAlert.gameObject.SetActive(false);
                return;
            }
            

            buttonAlert.gameObject.SetActive(true);

            buttonAlert.image.sprite = introduceStory.isNotify? spriteAlertOn : spriteAlertOff;
        }


        /// <summary>
        /// 작품 알림 버튼 클릭
        /// </summary>
        public void OnClickAlertButton()
        {
            /*
            if(Application.isEditor) {
                Debug.LogError("It's not working in editor");
                return;
            }
            */
            
            // 푸쉬 알림에 대해 허용해두지 않았다면 팝업 띄워주고 그만둠
            /*
            if(!SystemManager.main.pushTokenInfo.agreement.pushEnabled)
            {
                SystemManager.ShowMessageWithLocalize("6315");
                return;
            }
            */

            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "setUserProjectNotification";
            sending[CommonConst.COL_PROJECT_ID] = introduceStory.projectID;
            sending["is_notify"] = introduceStory.isNotify ? 0 : 1;

            NetworkLoader.main.SendPost(CallbackProjectAlert, sending, true);
        }


        void CallbackProjectAlert(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkLoader.CheckResponseValidation(request, response))
                return;

            JsonData result = JsonMapper.ToObject(response.DataAsText);
            Debug.Log("CallbackProjectAlert : " + JsonMapper.ToStringUnicode(result));

            // 작품 알람 설정값 갱신
            StoryManager.main.FindProject(introduceStory.projectID).isNotify = SystemManager.GetJsonNodeBool(result, "is_notify");
            introduceStory.isNotify = SystemManager.GetJsonNodeBool(result, "is_notify");

            SetAlertButtonState();

            // 눌렀을 때만 Alert popup이 뜨도록 수정
            if (introduceStory.isNotify)
            {
                
                SystemManager.ShowMessageWithLocalize("6311");
            }
            else
            {
                
                SystemManager.ShowMessageWithLocalize("6312");
            }
        }


        #endregion
    }
}