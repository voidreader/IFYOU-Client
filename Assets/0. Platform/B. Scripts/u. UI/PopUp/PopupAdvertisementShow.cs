using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using BestHTTP;
using LitJson;


namespace PIERStory {
    public class PopupAdvertisementShow : PopupBase
    {
        
        [SerializeField] GameObject groupTimer;
        [SerializeField] TextMeshProUGUI textTimer; // 타이머 
        [SerializeField] Image progressBar; // 하단 게이지 
        
        [SerializeField] GameObject btnClose; // 클로즈 버튼 
        
        
        
        [SerializeField] float timer = 5;
        float maxTimer = 0;
        
        bool isInvoked = false;
        bool isPurchasePressed = false;
        
        public override void Show() {
            if(isShow)
                return;
            
            base.Show();
            
            StartCoroutine(RoutineTimer());
        }
        
        IEnumerator RoutineTimer() {
            
            yield return null;
            
            UserManager.main.RefreshIndicators();
            
            timer = 5.5f ;
            maxTimer = timer;
            
            while(timer > 0) {
                timer -= Time.deltaTime;
                progressBar.fillAmount = timer / maxTimer;
                
                textTimer.text = Mathf.RoundToInt(timer).ToString();
                
                yield return null;
            }
            
            yield return null;
            
            // 구매 버튼 눌렀으면 광고 보여주면 안된다.
            if(isPurchasePressed)
                yield break;
            
           
           // 종료 후 광고 노출. 
           this.Hide();
            
        }
        
        public override void Hide() {
            base.Hide();
            
            if(isInvoked)
                return;
            
            AdManager.OnShowAdvertisement?.Invoke();
            isInvoked = true;
        }
        
        public void OnClickRemoveAD() {
            
            if(isPurchasePressed)
                return;
            
            
            // 코인 부족
            if(!UserManager.main.CheckCoinProperty(20)) {
                
                // SystemManager.ShowLobbySubmitPopup(SystemManager.GetLocalizedText("80013"));
                SystemManager.ShowSimpleAlertLocalize("80013");
                isPurchasePressed = false;
                return;
            }
            
            isPurchasePressed = true; // 버튼 눌렀음
            
            // 차감 통신 시작 
            // 코인 차감이 되면, 서버에서 차감과 동시에 구매 상태를 AD => Permanent로 변경시킨다. 
            JsonData sendingData = new JsonData();
            sendingData["func"] = "requestRemoveCurrentAD";
            sendingData["price"] = 20;
            sendingData["episode_id"] = StoryManager.main.CurrentEpisodeID;
            sendingData["project_id"] = StoryManager.main.CurrentProjectID;
            
            NetworkLoader.main.SendPost(OnComplete, sendingData, true);
            
        }
        
        void OnComplete(HTTPRequest request, HTTPResponse response) {
            if(!NetworkLoader.CheckResponseValidation(request, response))
                return;
                
            Debug.Log("### Remove current ad");
            
            JsonData result = JsonMapper.ToObject(response.DataAsText);
            
            
            // 성공했으면 bank 업데이트 
            UserManager.main.SetBankInfo(result);
            
            // 구매기록 업데이트 
            if (result.ContainsKey(UserManager.NODE_PURCHASE_HIST)) {
                UserManager.main.SetNodeEpisodePurchaseHistory(result[UserManager.NODE_PURCHASE_HIST]);
                StoryManager.main.RefreshRegularEpisodesPurchaseState();
               
               Debug.Log("GameManager.main.currentEpisodeData :: " + GameManager.main.currentEpisodeData.purchaseState.ToString()) ;
            }
            
            // 이 팝업을 하이드
            base.Hide();
        }
        
    }
}
