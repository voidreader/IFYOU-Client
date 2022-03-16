using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace PIERStory {
    public class PopupManager : SerializedMonoBehaviour
    {
        public static PopupManager main = null;
        
        public List<PopupBase> ListShowingPopup = new List<PopupBase>();
        
        public Canvas popupCanvas = null;
        
        public Dictionary<string, GameObject> DictPopup;
        public Queue<PopupBase> PopupQueue = new Queue<PopupBase>(); 
        public PopupBase CurrentQueuePopup = null; // 큐 팝업. 
        
        [SerializeField] List<ParticleSystem> confittiParticles;
        
        
        [Space]
        [Space]
        [Header("Prefab")]
        [SerializeField] GameObject popupAccount;
        [SerializeField] GameObject popupAchivement;
        [SerializeField] GameObject popupAdvertisementShow;
        public GameObject popupAttendance;
        public GameObject popupConnectingShop;
        [SerializeField] GameObject popupCoupon;
        [SerializeField] GameObject popupEndingAlert;
        [SerializeField] GameObject popupEpisodeClearReward;
        [SerializeField] GameObject popupExp;
        public GameObject popupGameMessage;
        [SerializeField] GameObject popupLevelUp;
        [SerializeField] GameObject popupMessageAlert;
        [SerializeField] GameObject popupType1;
        [SerializeField] GameObject popupType2;
        [SerializeField] GameObject popupPremiumPass;
        [SerializeField] GameObject popupSideAlert;
        [SerializeField] GameObject popupSimpleAlert;
        
        [SerializeField] GameObject popupTutorialComplete;
        public GameObject popupTutorialSlide;
        
        [SerializeField] GameObject popupNickname;       
        
        [SerializeField] GameObject popupPackDetail;
        [SerializeField] GameObject popupNotice;
        [SerializeField] GameObject popupMail;
        [SerializeField] GameObject popupResource; // 리소스 표현  팝업 
        [SerializeField] GameObject popupHowToPlay; // How to play 팝업
        
        [SerializeField] GameObject popupFlowReset; // 리셋.        
        [SerializeField] GameObject popupStoryReset; // 스토리 전체 리셋 1화로.. 
        [SerializeField] GameObject popupGameAbility; // 게임 능력치 증감 안내 메세지 
         
        private void Awake() {
            if(main != null) {
                Destroy(this.gameObject);
                return;
            }
            
            main = this;
            
            
            HideConfetti();
            
            DontDestroyOnLoad(this);
                
        }
        
        void Start() {

            CreatePopupDict();
        }
        

        
        /// <summary>
        /// 팝업 매니저 초기화
        /// 각 씬의 담당 매니저에서 따로 호출해줘야한다. (LobbyManager, GameManager)
        /// </summary>
        public void InitPopupManager() {
            // StopCoroutine(PopupQueueRoutine());
            
            // 팝업 큐 루틴 시작 
            StartCoroutine(PopupQueueRoutine());
        }
        
        
        IEnumerator PopupQueueRoutine() {
            
            PopupQueue.Clear();
            CurrentQueuePopup = null;
            
            Debug.Log(">> PopupQueueRoutine START");
            
            while(true) {
                
                yield return null;
                
                // * 현재 보여지고 있는 큐 팝업이 있으면, 대기 
                while(CurrentQueuePopup != null && CurrentQueuePopup.gameObject.activeSelf) {
                    yield return null;
                }
                
                // * 팝업 큐에 팝업이 없으면 대기
                while(PopupQueue.Count < 1) {
                    yield return null;
                }
                
                // 큐에서 하나 가져온다. 
                CurrentQueuePopup = PopupQueue.Dequeue();
                
                if(CurrentQueuePopup != null) {
                    Debug.Log("<color=yellow>### PopupQueueRoutine New popup show!</color> : " + CurrentQueuePopup.name);
                    
                    CurrentQueuePopup.Show(); // 보여주기 
                }
            }
            
            // Debug.Log(">> PopupQueueRoutine END");
        }
        
        
        /// <summary>
        /// 팝업 생성 및 받아오기 
        /// </summary>
        /// <param name="popupName"></param>
        /// <returns></returns>
        public PopupBase GetPopup(string popupName) {
            if(!DictPopup.ContainsKey(popupName))
                return null;
                
            // 생성될 캔버스 처리 
            if(!popupCanvas) {
                popupCanvas = GameObject.FindGameObjectWithTag("PopupCanvas").GetComponent<Canvas>();
            }
            
            GameObject clone = Instantiate(DictPopup[popupName], popupCanvas.transform);
            PopupBase popup = clone.GetComponent<PopupBase>();
            popup.InitPopup(); // 알파값을 0으로 만들기. 
           
            return popup;
        }
        
        /// <summary>
        /// 팝업 보여주기
        /// </summary>
        /// <param name="popupName"></param>
        /// <param name="addToPopupQueue"></param>
        /// <param name="instantAction"></param>
        public void ShowPopup(string popupName, bool addToPopupQueue, bool instantAction = false) {
            PopupBase p = GetPopup(popupName);
            
            if(p == null) {
                Debug.LogError(">> No Popup... " + popupName);
                return; 
            }
            
            ShowPopup(p, addToPopupQueue, instantAction);
        }
        
        /// <summary>
        /// 팝업 보여주기 
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="addToPopupQueue"></param>
        /// <param name="instantAction"></param>
        public void ShowPopup(PopupBase popup, bool addToPopupQueue, bool instantAction = false) {
            if (popup == null) return;
            
            if(addToPopupQueue) {
                Debug.Log("<color=yellow>### Added Popup Queue. </color>" + popup.name);
                PopupQueue.Enqueue(popup);  // 큐를 통해 실행.
            }
            else {
                popup.Show(); // 독립적인 실행 
            }
        }
        
        
        /// <summary>
        /// 모든 활성화 팝업 감추기
        /// </summary>
        public void HideActivePopup() {
            for(int i=0; i<ListShowingPopup.Count;i++) {
                
                if(ListShowingPopup[i] != null)
                    ListShowingPopup[i].Hide();
            }
        }
        
        /// <summary>
        /// 팝업이 활성화될때 호출 
        /// </summary>
        /// <param name="__p"></param>
        public void AddActivePopup(PopupBase __p) {
            ListShowingPopup.Add(__p);
        }
        
        public void RemoveActivePopup(PopupBase __p) {
            ListShowingPopup.Remove(__p);
        }
        
        
        /// <summary>
        /// 폭죽놀이다!
        /// </summary>
        public void PlayConfetti() {
            StartCoroutine(RoutineConfetti());
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void HideConfetti() {
            for(int i=0; i<confittiParticles.Count;i++) {
                confittiParticles[i].gameObject.SetActive(false);
            }
        }
        
        IEnumerator RoutineConfetti() {
            for(int i=0; i<confittiParticles.Count;i++) {
                confittiParticles[i].gameObject.SetActive(true);
                confittiParticles[i].Play();
                
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f,0.5f));
            }
        }
        
        
        
        #region CreatePopupDict
        
        void CreatePopupDict() {
            if(DictPopup.ContainsKey("Account")) 
                DictPopup["Account"] = popupAccount;
            else 
                DictPopup.Add("Account", popupAccount);
                
            if(DictPopup.ContainsKey(GameConst.POPUP_ACHIEVEMENT_ILLUST)) 
                DictPopup[GameConst.POPUP_ACHIEVEMENT_ILLUST] = popupAchivement;
            else 
                DictPopup.Add(GameConst.POPUP_ACHIEVEMENT_ILLUST, popupAchivement);
                
            if(DictPopup.ContainsKey("AdvertisementShow")) 
                DictPopup["AdvertisementShow"] = popupAdvertisementShow;
            else 
                DictPopup.Add("AdvertisementShow", popupAdvertisementShow);

            if (DictPopup.ContainsKey("Attendance"))
                DictPopup["Attendance"] = popupAttendance;
            else
                DictPopup.Add("Attendance", popupAttendance);
                
            if(DictPopup.ContainsKey("Coupon")) 
                DictPopup["Coupon"] = popupCoupon;
            else 
                DictPopup.Add("Coupon", popupCoupon);

            if (DictPopup.ContainsKey(CommonConst.POPUP_CONNECTING_SHOP))
                DictPopup[CommonConst.POPUP_CONNECTING_SHOP] = popupConnectingShop;
            else
                DictPopup.Add(CommonConst.POPUP_CONNECTING_SHOP, popupConnectingShop);

            if(DictPopup.ContainsKey(GameConst.POPUP_ENDING_ALERT)) 
                DictPopup[GameConst.POPUP_ENDING_ALERT] = popupEndingAlert;
            else 
                DictPopup.Add(GameConst.POPUP_ENDING_ALERT, popupEndingAlert);
                
            if(DictPopup.ContainsKey(GameConst.POPUP_EPISODE_FIRST_REWARD)) 
                DictPopup[GameConst.POPUP_EPISODE_FIRST_REWARD] = popupEpisodeClearReward;
            else 
                DictPopup.Add(GameConst.POPUP_EPISODE_FIRST_REWARD, popupEpisodeClearReward);
                
            if(DictPopup.ContainsKey("EXP")) 
                DictPopup["EXP"] = popupExp;
            else 
                DictPopup.Add("EXP", popupExp);


            if (DictPopup.ContainsKey(GameConst.TEMPLATE_GAME_MESSAGE))
                DictPopup[GameConst.TEMPLATE_GAME_MESSAGE] = popupGameMessage;
            else
                DictPopup.Add(GameConst.TEMPLATE_GAME_MESSAGE, popupGameMessage);
                
            if(DictPopup.ContainsKey("LevelUp")) 
                DictPopup["LevelUp"] = popupLevelUp;
            else 
                DictPopup.Add("LevelUp", popupLevelUp);
                
            if(DictPopup.ContainsKey(CommonConst.POPUP_MESSAGE_ALERT)) 
                DictPopup[CommonConst.POPUP_MESSAGE_ALERT] = popupMessageAlert;
            else 
                DictPopup.Add(CommonConst.POPUP_MESSAGE_ALERT, popupMessageAlert);
                
            if(DictPopup.ContainsKey(CommonConst.POPUP_TYPE_1)) 
                DictPopup[CommonConst.POPUP_TYPE_1] = popupType1;
            else 
                DictPopup.Add(CommonConst.POPUP_TYPE_1, popupType1);
                
            if(DictPopup.ContainsKey(CommonConst.POPUP_TYPE_2)) 
                DictPopup[CommonConst.POPUP_TYPE_2] = popupType2;
            else 
                DictPopup.Add(CommonConst.POPUP_TYPE_2, popupType2);
                
            if(DictPopup.ContainsKey("PremiumPass")) 
                DictPopup["PremiumPass"] = popupPremiumPass;
            else 
                DictPopup.Add("PremiumPass", popupPremiumPass);
                
            if(DictPopup.ContainsKey(GameConst.POPUP_SIDE_ALERT)) 
                DictPopup[GameConst.POPUP_SIDE_ALERT] = popupSideAlert;
            else 
                DictPopup.Add(GameConst.POPUP_SIDE_ALERT, popupSideAlert);
                
            if(DictPopup.ContainsKey(CommonConst.POPUP_SIMPLE_ALERT)) 
                DictPopup[CommonConst.POPUP_SIMPLE_ALERT] = popupSimpleAlert;
            else 
                DictPopup.Add(CommonConst.POPUP_SIMPLE_ALERT, popupSimpleAlert);
            
            if(DictPopup.ContainsKey(CommonConst.POPUP_TUTORIAL_TUTORIAL_COMPLETE)) 
                DictPopup[CommonConst.POPUP_TUTORIAL_TUTORIAL_COMPLETE] = popupTutorialComplete;
            else 
                DictPopup.Add(CommonConst.POPUP_TUTORIAL_TUTORIAL_COMPLETE, popupTutorialComplete);
                
            if (DictPopup.ContainsKey(CommonConst.POPUP_TUTORIAL_SLIDE))
                DictPopup[CommonConst.POPUP_TUTORIAL_SLIDE] = popupTutorialSlide;
            else
                DictPopup.Add(CommonConst.POPUP_TUTORIAL_SLIDE, popupTutorialSlide);

            if(DictPopup.ContainsKey("Nickname")) 
                DictPopup["Nickname"] = popupNickname;
            else 
                DictPopup.Add("Nickname", popupNickname);
                
            if(DictPopup.ContainsKey("PackDetail")) 
                DictPopup["PackDetail"] = popupPackDetail;
            else 
                DictPopup.Add("PackDetail", popupPackDetail);
                
            if(DictPopup.ContainsKey("Notice")) 
                DictPopup["Notice"] = popupNotice;
            else 
                DictPopup.Add("Notice", popupNotice);
                
            if(DictPopup.ContainsKey("Mail")) 
                DictPopup["Mail"] = popupMail;
            else 
                DictPopup.Add("Mail", popupMail);
                
            if(DictPopup.ContainsKey("Resource")) 
                DictPopup["Resource"] = popupResource;
            else 
                DictPopup.Add("Resource", popupResource);
                
                
            if(DictPopup.ContainsKey("HowToPlay")) 
                DictPopup["HowToPlay"] = popupHowToPlay;
            else 
                DictPopup.Add("HowToPlay", popupHowToPlay);
                
            if(DictPopup.ContainsKey("Reset")) 
                DictPopup["Reset"] = popupFlowReset;
            else 
                DictPopup.Add("Reset", popupFlowReset);
                
            if(DictPopup.ContainsKey("StoryReset")) 
                DictPopup["StoryReset"] = popupStoryReset;
            else 
                DictPopup.Add("StoryReset", popupStoryReset);
                
            if(DictPopup.ContainsKey(GameConst.POPUP_GAME_ABILITY)) 
                DictPopup[GameConst.POPUP_GAME_ABILITY] = popupGameAbility;
            else 
                DictPopup.Add(GameConst.POPUP_GAME_ABILITY, popupGameAbility);
        }
        
        #endregion
        
    }
}