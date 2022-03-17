using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LitJson;
using BestHTTP;


namespace PIERStory {
    public class ViewIntroduce : CommonView
    {
        public Doozy.Runtime.UIManager.Containers.UIContainer container;
        
        [SerializeField] ImageRequireDownload mainThumbnail;    // 썸네일 
        
        [SerializeField] TextMeshProUGUI textTitle;             // 타이틀
        [SerializeField] TextMeshProUGUI textAuthor;            // 원작자
        [SerializeField] TextMeshProUGUI textProducer;          // 제작사
        [SerializeField] TextMeshProUGUI textGenre;             // 장르 
        [SerializeField] TextMeshProUGUI textSummary;           // 요약
        
        [SerializeField] Button btnLike; // 좋아요 버튼
        [SerializeField] Sprite spriteLikeOff; // 좋아요 버튼 OFF 스프라이트
        [SerializeField] Sprite spriteLikeOn; // 좋아요 버튼 ON 스프라이트        
        
        public StoryData introduceStory;
        
        public override void OnView() {
            base.OnView();
        }
        
        public override void OnStartView() {
            base.OnStartView();
            
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
        }
        
        
        public void OnClickStart() {
            if(introduceStory.isLock) {
                SystemManager.ShowMessageWithLocalize("6061", true);
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
    }
}