using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using LitJson;

namespace PIERStory {

    public class PopupLevelUp : PopupBase
    {
        
        [SerializeField] TextMeshProUGUI textLevel;
        [SerializeField] TextMeshProUGUI textSpecialGiftName;
        [SerializeField] TextMeshProUGUI textNormalGiftQuantity;
        
        [SerializeField] TextMeshProUGUI textEventBonus; // 이벤트 보너스 
        
        [SerializeField] ImageRequireDownload specialGiftIcon;
        
        [SerializeField] GameObject normalGiftGroup; // 일반 기프트 그룹 
        
        [SerializeField] List<ResourceIconQuantity> listNormalGift;
        
        [SerializeField] Image aura; // 
        [SerializeField] bool isCompleteLoad = false;
        
        [SerializeField] string currency_type = string.Empty;
        [SerializeField] string currency_name = string.Empty;
        
        JsonData beforeData = null; // 경험치 획득 전 
        JsonData currentData = null; // 경험치 획득 후 
        JsonData rewardData = null;
        
        JsonData rewardList = null; // 재화 여러개. 
        
        [SerializeField] int beforeLevel = 0;
        [SerializeField] int currentLevel = 0;
        
        
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            
            StartCoroutine(CheckTime());
            
            isCompleteLoad = false;
            textLevel.text = string.Empty;
            textSpecialGiftName.text = string.Empty;
            textNormalGiftQuantity.text = string.Empty;
            
            // normalGiftIcon.OnDownloadImage = OnLoadNormalGift;
            specialGiftIcon.OnDownloadImage = OnLoadSpecialGift;
            
            
            aura.DOKill();
            aura.color = new Color(1,1,1,0);
            
            specialGiftIcon.gameObject.SetActive(false);
            normalGiftGroup.SetActive(false);
            for(int i=0; i<listNormalGift.Count;i++){ 
                listNormalGift[i].gameObject.SetActive(false);
            }
            
            
            beforeData = Data.contentJson["before"]; // 경험치 얻기 전 
            currentData = Data.contentJson["current"]; // 경험치 얻은 후 
            rewardData = Data.contentJson["reward"];  // 보상 데이터 
            
            rewardList = Data.contentJson["rewardList"];  // 보상 여러개인 경우의 데이터 
            
            beforeLevel = SystemManager.GetJsonNodeInt(beforeData, "level"); // 이전 레벨 
            currentLevel = SystemManager.GetJsonNodeInt(currentData, "level"); // 현재 레벨 
            
            // 보상 정보 설정             
            currency_type = rewardData["currency_type"].ToString();
            currency_name = rewardData["currency_name"].ToString();
            
            // 특별선물과 일반선물의 구분
            if(rewardList.Count > 0) {
                normalGiftGroup.SetActive(true);
                
                // 여러개라서 
                for(int i=0; i<rewardList.Count;i++) {
                    
                    if(i >= listNormalGift.Count)
                        break;
                    
                    listNormalGift[i].SetResourceInfo(SystemManager.GetJsonNodeString(rewardList[i], "icon_image_url"), SystemManager.GetJsonNodeString(rewardList[i], "icon_image_key"), SystemManager.GetJsonNodeInt(rewardList[i], "quantity"));
                }
                
                Invoke("OnLoadNormalGift", 1);
                
            }
            else if (rewardList.Count == 1 && currency_type == "consumable") { // 하나만, 소모성인경우 
                normalGiftGroup.SetActive(true);
                listNormalGift[0].SetResourceInfo(SystemManager.GetJsonNodeString(rewardData, "icon_image_url"), SystemManager.GetJsonNodeString(rewardData, "icon_image_key"), SystemManager.GetJsonNodeInt(rewardData, "quantity"));
                Invoke("OnLoadNormalGift", 1);
            }
            else { // 스페셜 
                specialGiftIcon.gameObject.SetActive(true);
                specialGiftIcon.SetDownloadURL(rewardData["icon_image_url"].ToString(), rewardData["icon_image_key"].ToString());
                textSpecialGiftName.text = rewardData["currency_name"].ToString();
            }
            
            
            
            
            // 레벨 설정
            textLevel.text = beforeData["level"].ToString(); // 일단 before 레벨로 세팅 
            
            // 이벤트 설정 처리 
            textEventBonus.gameObject.SetActive(true);
            
            if(SystemManager.GetJsonNodeBool(Data.contentJson, "event")) {
                Debug.Log("### Level up Event");
                textEventBonus.text = SystemManager.GetLocalizedText("6149");
            }
            else {
                Debug.Log("### level up Not Event");
                
                textEventBonus.text = SystemManager.GetLocalizedText("6177");
            }
            
            
        }
        
        /// <summary>
        /// 쇼 끝나고 실행된다.
        /// </summary>
        public void OnShow() {
            if(isShow)
                return;
            
            aura.DOFade(1, 1).SetLoops(-1, LoopType.Yoyo);
            PopupManager.main.PlayConfetti();
            
            textLevel.DOCounter(beforeLevel, currentLevel, 1);
            // 점프 시키고 
            textLevel.rectTransform.DOLocalJump(textLevel.rectTransform.localPosition, 10, 1, 1);
            
        }
        
        /// <summary>
        /// 컨테이너 하이드 시작할때 실행한다. 
        /// </summary>
        public void OnStartHide() {
            PopupManager.main.HideConfetti();
        }
        
        
        
        void OnLoadNormalGift() {
            isCompleteLoad = true;
        }
        
        void OnLoadSpecialGift() {
            isCompleteLoad = true;
        }
        
        IEnumerator CheckTime() {
            yield return new WaitForSeconds(3);
            
            isCompleteLoad = true;
        }
        
        public override void Hide() {
            
            // 다 끝나야 닫을 수 있게 하자.
            if(!isCompleteLoad)
                return;
            
            base.Hide();
            
            // UserManager.main.SetBankInfo();
            // UserManager.main.SetBankInfo(Data.contentJson); // 뱅크 리프레시 
            UserManager.main.SetNotificationInfo(Data.contentJson);
            
        }

    }
}