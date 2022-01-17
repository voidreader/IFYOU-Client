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
        [SerializeField] GameObject popupCoupon;
        [SerializeField] GameObject popupEndingAlert;
        [SerializeField] GameObject popupEpisodeClearReward;
        [SerializeField] GameObject popupExp;
        [SerializeField] GameObject popupLevelUp;
        [SerializeField] GameObject popupMessageAlert;
        [SerializeField] GameObject popupType1;
        [SerializeField] GameObject popupType2;
        [SerializeField] GameObject popupPremiumPass;
        [SerializeField] GameObject popupSideAlert;
        [SerializeField] GameObject popupSimpleAlert;
        
        [SerializeField] GameObject popupTutorialComplete;
        [SerializeField] GameObject popupTutorialEpisodeStart;
        [SerializeField] GameObject popupTutorialMain;
        [SerializeField] GameObject popupTutorialStoryDetail;
        
        [SerializeField] GameObject popupNickname;       
        
        [SerializeField] GameObject popupPackDetail;
        [SerializeField] GameObject popupNotice;
        
         
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
            StopCoroutine(PopupQueueRoutine());
            
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
                while(CurrentQueuePopup) {
                    yield return new WaitForSeconds(0.1f);
                }
                
                // * 팝업 큐에 팝업이 없으면 대기
                while(PopupQueue.Count < 1) {
                    yield return null;
                }
                
                // 큐에서 하나 가져온다. 
                CurrentQueuePopup = PopupQueue.Dequeue();
                yield return null;
                
                if(CurrentQueuePopup != null) {
                    Debug.Log(">> PopupQueueRoutine New popup show!");
                    
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
                Debug.Log("### Added Popup Queue. " + popup.name);
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
                
            if(DictPopup.ContainsKey("AchivementIllust")) 
                DictPopup["AchivementIllust"] = popupAchivement;
            else 
                DictPopup.Add("AchivementIllust", popupAchivement);
                
            if(DictPopup.ContainsKey("AdvertisementShow")) 
                DictPopup["AdvertisementShow"] = popupAdvertisementShow;
            else 
                DictPopup.Add("AdvertisementShow", popupAdvertisementShow);
                
            if(DictPopup.ContainsKey("Coupon")) 
                DictPopup["Coupon"] = popupCoupon;
            else 
                DictPopup.Add("Coupon", popupCoupon);
                
            if(DictPopup.ContainsKey("EndingAlert")) 
                DictPopup["EndingAlert"] = popupEndingAlert;
            else 
                DictPopup.Add("EndingAlert", popupEndingAlert);
                
            if(DictPopup.ContainsKey("EpisodeFirstReward")) 
                DictPopup["EpisodeFirstReward"] = popupEpisodeClearReward;
            else 
                DictPopup.Add("EpisodeFirstReward", popupEpisodeClearReward);
                
            if(DictPopup.ContainsKey("EXP")) 
                DictPopup["EXP"] = popupExp;
            else 
                DictPopup.Add("EXP", popupExp);
                
            if(DictPopup.ContainsKey("LevelUp")) 
                DictPopup["LevelUp"] = popupLevelUp;
            else 
                DictPopup.Add("LevelUp", popupLevelUp);
                
            if(DictPopup.ContainsKey("MessageAlert")) 
                DictPopup["MessageAlert"] = popupMessageAlert;
            else 
                DictPopup.Add("MessageAlert", popupMessageAlert);
                
            if(DictPopup.ContainsKey("Popup1")) 
                DictPopup["Popup1"] = popupType1;
            else 
                DictPopup.Add("Popup1", popupType1);
                
            if(DictPopup.ContainsKey("Popup2")) 
                DictPopup["Popup2"] = popupType2;
            else 
                DictPopup.Add("Popup2", popupType2);
                
            if(DictPopup.ContainsKey("PremiumPass")) 
                DictPopup["PremiumPass"] = popupPremiumPass;
            else 
                DictPopup.Add("PremiumPass", popupPremiumPass);
                
            if(DictPopup.ContainsKey("SideAlert")) 
                DictPopup["SideAlert"] = popupSideAlert;
            else 
                DictPopup.Add("SideAlert", popupSideAlert);
                
            if(DictPopup.ContainsKey("SimpleAlert")) 
                DictPopup["SimpleAlert"] = popupSimpleAlert;
            else 
                DictPopup.Add("SimpleAlert", popupSimpleAlert);
                
            if(DictPopup.ContainsKey("TutorialComplete")) 
                DictPopup["TutorialComplete"] = popupTutorialComplete;
            else 
                DictPopup.Add("TutorialComplete", popupTutorialComplete);
                
            if(DictPopup.ContainsKey("TutorialEpisodeStart")) 
                DictPopup["TutorialEpisodeStart"] = popupTutorialEpisodeStart;
            else 
                DictPopup.Add("TutorialEpisodeStart", popupTutorialEpisodeStart);
                
            if(DictPopup.ContainsKey("TutorialMain")) 
                DictPopup["TutorialMain"] = popupTutorialMain;
            else 
                DictPopup.Add("TutorialMain", popupTutorialMain);
                
            if(DictPopup.ContainsKey("TutorialStoryDetail")) 
                DictPopup["TutorialStoryDetail"] = popupTutorialStoryDetail;
            else 
                DictPopup.Add("TutorialStoryDetail", popupTutorialStoryDetail);
                
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
        }
        
        #endregion
        
    }
}