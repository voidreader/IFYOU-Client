using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;


namespace PIERStory {
    public class ViewIntroduce : CommonView
    {
        public Doozy.Runtime.UIManager.Containers.UIContainer container;
        
        [SerializeField] ImageRequireDownload mainThumbnail;    // 썸네일 
        [SerializeField] TextMeshProUGUI textRecommend; // 추천 작 안내 
        
        [SerializeField] TextMeshProUGUI textTitle;             // 타이틀
        [SerializeField] TextMeshProUGUI textAuthor;            // 원작자
        [SerializeField] TextMeshProUGUI textProducer;          // 제작사
        [SerializeField] TextMeshProUGUI textGenre;             // 장르 
        [SerializeField] TextMeshProUGUI textSummary;           // 요약

        public Button buttonAlert;          // 작품 알림 버튼
        public Sprite spriteAlertOff;       // 작품 알림 버튼 Off Sprite
        public Sprite spriteAlertOn;        // 작품 알림 버튼 On Sprite

        [SerializeField] Button btnLike; // 좋아요 버튼
        [SerializeField] Sprite spriteLikeOff; // 좋아요 버튼 OFF 스프라이트
        [SerializeField] Sprite spriteLikeOn; // 좋아요 버튼 ON 스프라이트        
        
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
            
        }
        
        void SetInfo() {
            
            if(string.IsNullOrEmpty(SystemListener.main.introduceStory.projectID))
                return;
            
            introduceStory = SystemListener.main.introduceStory;
             

            // 이미지를 프리미엄 패스 이미지와 동일한 이미지를 사용한다.
            mainThumbnail.SetDownloadURL(introduceStory.premiumPassURL, introduceStory.premiumPassKey);
            
            
            textTitle.text = introduceStory.title;
            textAuthor.text = SystemManager.GetLocalizedText("6179") + " / " + introduceStory.original; // 원작
            textProducer.text = SystemManager.GetLocalizedText("6180") + " / " + introduceStory.writer;
            textSummary.text = introduceStory.summary; // 요약 
            textGenre.text = SystemManager.GetLocalizedText("6181") + " / " + introduceStory.genre;
            
            // serialGroup.SetActive(introduceStory.isSerial);
            serialGroup.SetActive(true);
            
            if(introduceStory.isSerial)
                textSerialDay.text = string.Format(SystemManager.GetLocalizedText("5184"), introduceStory.GetSeiralDay()); // 연재일 설정..
            else 
                textSerialDay.text = SystemManager.GetLocalizedText("5186"); // 완결 
            
            SetLikeButtonState();
            SetAlertButtonState();

            // 인트로에서 넘어온 경우에 대한 처리 추가 
            if (SystemListener.main.isIntroduceRecommended) {
                
                textRecommend.text = SystemManager.GetLocalizedText("6289");
                textRecommend.gameObject.SetActive(true);
                SystemListener.main.isIntroduceRecommended = false;
            }
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
                SystemManager.ShowSimpleAlertLocalize("6188");
            else
                SystemManager.ShowSimpleAlertLocalize("6189");
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
            if(Application.isEditor) {
                Debug.LogError("It's not working in editor");
                return;
            }
            
            // 푸쉬 알림에 대해 허용해두지 않았다면 팝업 띄워주고 그만둠
            if(SystemManager.main.pushTokenInfo.agreement.pushEnabled)
            {
                SystemManager.ShowMessageWithLocalize("6315");
                return;
            }

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
                
                SystemManager.ShowSimpleAlertLocalize("6311");
            }
            else
            {
                
                SystemManager.ShowSimpleAlertLocalize("6312");
            }
        }


        #endregion
    }
}