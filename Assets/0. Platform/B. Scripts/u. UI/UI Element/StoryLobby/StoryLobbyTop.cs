using System;
using UnityEngine;


namespace PIERStory {
    public class StoryLobbyTop : MonoBehaviour
    {
        public static Action OnInitializeStoryLobbyTop = null;
        
        // public PassButton passButton; // 프리미엄 패스 구매 버튼
        // public ImageRequireDownload passBadge; // 프리미엄 패스 뱃지 
        
        public AllPassTimer allpassTimer; // 올패스 타이머 
        public PremiumPassButton premiumPassButton; // 프리미엄 패스 
        public OnedayPassButton onedayPassButton; // 원데이 패스 
        
        
        // Start is called before the first frame update
        void Start()
        {
            OnInitializeStoryLobbyTop = InitStoryLobbyTop;
        }


        /// <summary>
        /// 초기화
        /// </summary>
        void InitStoryLobbyTop()
        {
            
            Debug.Log("#### InitStoryLobbyTop ####");
            
            // 패스 버튼을 처리한다.
            premiumPassButton.gameObject.SetActive(false);
            onedayPassButton.gameObject.SetActive(false);
            
            premiumPassButton.SetPass(StoryManager.main.CurrentProject);
            onedayPassButton.SetPass(StoryManager.main.CurrentProject);
            
            // 프리미엄 패스를 구매한 경우 원데이 패스 버튼을 보여줄 필요가 없다. 
            if( !StoryManager.main.CurrentProject.IsValidOnedayPass() && StoryManager.main.CurrentProject.hasPremiumPass )
                onedayPassButton.gameObject.SetActive(false);            

        }
    }
}