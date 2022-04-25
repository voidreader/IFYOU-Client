using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using Doozy.Runtime.UIManager.Input;

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
        
        
        [Space(20)][Header("Prefab")]
        [SerializeField] GameObject popupAccount;
        [SerializeField] GameObject popupAchivement;
        [SerializeField] GameObject popupAdvertisementShow;
        public GameObject popupAttendance;

        [SerializeField] GameObject popupConfirm;
        public GameObject popupConnectingShop;
        public GameObject popupCoinStarHistory;
        [SerializeField] GameObject popupCoupon;

        public GameObject popupDataManager;
        public GameObject popupEndingHint;
        [SerializeField] GameObject popupEpisodeClearReward;
        [SerializeField] GameObject popupExpireToken; // 로그인 토큰 만료 팝업 
        [SerializeField] GameObject popupFlowReset; // 리셋.        
        
        [SerializeField] GameObject popupGameAbility; // 게임 능력치 증감 안내 메세지 
        public GameObject popupGameMessage;
        public GameObject popupGameOption;
        public GameObject popupGradeBenefitInfo;
        [SerializeField] GameObject popupGradeExp;
        [SerializeField] GameObject popupGradeUp;
        [SerializeField] GameObject popupHowToPlay; // How to play 팝업

        [SerializeField] GameObject popupMail;
        [SerializeField] GameObject popupMessageAlert;
        [SerializeField] GameObject popupNotice;
        [SerializeField] GameObject popupNickname;       
        
        [SerializeField] GameObject popupPackDetail;
        [SerializeField] GameObject popupPremiumPass;
        [SerializeField] GameObject popupResource; // 리소스 표현  팝업 
        [SerializeField] GameObject popupSideAlert;
        [SerializeField] GameObject popupSimpleAlert;
        [SerializeField] GameObject popupSpecialEpisodeBuy; // 스페셜 에피소드 구매 
        [SerializeField] GameObject popupStoryReset; // 스토리 전체 리셋 1화로.. 
        
        [SerializeField] GameObject popupSpecialHint;

        // 튜토리얼 팝업 모음
        public GameObject popupTutorial1;
        public GameObject popupTutorial2;
        public GameObject popupTutorial3;
        
        
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
        
        
        void Update() {
            
            // 팝업에서 안드로이드 Back 버튼 동작관련 처리 추가 
            
            if(Input.GetKeyDown(KeyCode.Escape))  {
                if(GetFrontActivePopup() == null) {
                    Debug.Log("((( PopupManager : No active popup");
                    return;
                }
                else {
                    
                    Debug.Log("((( PopupManager : Hide current popup");
                    
                    if(GetFrontActivePopup().isBlockBackButton) {
                        // 닫히지 않음.
                    }
                    else {
                        GetFrontActivePopup().Hide(); // Hide    
                    }
                }    
            }
        }
        

        
        /// <summary>
        /// 팝업 매니저 초기화
        /// 각 씬의 담당 매니저에서 따로 호출해줘야한다. (LobbyManager, GameManager)
        /// </summary>
        public void InitPopupManager() {
            
            ClearShowingPopup();
            
            // 팝업 큐 루틴 시작 
            StartCoroutine(PopupQueueRoutine());
        }
        
        public void ClearShowingPopup() {
            ListShowingPopup.Clear();
        }
        
        
        IEnumerator PopupQueueRoutine() {
            
            
            
            BackButton.blockBackInput = false; 
            
            PopupQueue.Clear();
            CurrentQueuePopup = null;
            
            Debug.Log("<color=yellow> PopupQueueRoutine START </color>");
            
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
            popup.popupName = popupName;
           
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
            if (popup == null) {
                Debug.Log("<color=yellow>### No Popup. </color>");
                return;
            }
            
            if(addToPopupQueue) {
                Debug.Log("<color=yellow>### Added Popup Queue. </color>" + popup.name);
                PopupQueue.Enqueue(popup);  // 큐를 통해 실행.
            }
            else {
                Debug.Log("<color=yellow>### new Popup. </color>" + popup.name);
                popup.Show(); // 독립적인 실행 
            }
        }
        
        /// <summary>
        /// 독립팝업 보여주기
        /// 다른 팝업이 하나라도 떠있으면 실행되지 않음. 
        /// </summary>
        /// <param name="popup"></param>
        public void ShowIndependentPopup(PopupBase popup) {
            if(popup == null)
                return;
                
            if(ListShowingPopup.Count > 0)
                return;
                
            popup.Show();
        }
        
        
        /// <summary>
        /// 가장 전면에 활성화된 팝업창 가져오기 
        /// </summary>
        /// <returns></returns>
        public PopupBase GetFrontActivePopup() {

            // null 에서 정리되지 않을때가 있어서 정리하고 입력 처리
            CheckShowingPopupListValidation();
            
            if(ListShowingPopup.Count == 0)
                return null;
               
                
            return ListShowingPopup[ListShowingPopup.Count-1]; 
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
            
            // null 에서 정리되지 않을때가 있어서 정리하고 입력 처리
            CheckShowingPopupListValidation();
            
            if(!ListShowingPopup.Contains(__p))
                ListShowingPopup.Add(__p);
            
            BackButton.blockBackInput = true;
            
        }
        
        public void RemoveActivePopup(PopupBase __p) {
            ListShowingPopup.Remove(__p);
        
            CheckShowingPopupListValidation();            
            
            // 종료 시점에 살아있는 팝업이 있으면 backbutton true 처리 
            if(ListShowingPopup.Count > 0)
                BackButton.blockBackInput = true;
            else 
                BackButton.blockBackInput = false;
            
        }

        /// <summary>
        /// 
        /// </summary>        
        void CheckShowingPopupListValidation() {
            for(int i=ListShowingPopup.Count-1; i>=0; i--) {
                if(ListShowingPopup[i] == null) {
                    ListShowingPopup.RemoveAt(i);
                }
            }            
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
                
                yield return new WaitForSeconds(Random.Range(0.2f,0.5f));
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

            if (DictPopup.ContainsKey(LobbyConst.POPUP_ATTENDANCE))
                DictPopup[LobbyConst.POPUP_ATTENDANCE] = popupAttendance;
            else
                DictPopup.Add(LobbyConst.POPUP_ATTENDANCE, popupAttendance);
                
            if(DictPopup.ContainsKey(LobbyConst.POPUP_COUPON)) 
                DictPopup[LobbyConst.POPUP_COUPON] = popupCoupon;
            else 
                DictPopup.Add(LobbyConst.POPUP_COUPON, popupCoupon);

            if (DictPopup.ContainsKey(CommonConst.POPUP_CONNECTING_SHOP))
                DictPopup[CommonConst.POPUP_CONNECTING_SHOP] = popupConnectingShop;
            else
                DictPopup.Add(CommonConst.POPUP_CONNECTING_SHOP, popupConnectingShop);

            if (DictPopup.ContainsKey(LobbyConst.POPUP_COIN_STAR_HISTORY))
                DictPopup[LobbyConst.POPUP_COIN_STAR_HISTORY] = popupCoinStarHistory;
            else
                DictPopup.Add(LobbyConst.POPUP_COIN_STAR_HISTORY, popupCoinStarHistory);


            if (DictPopup.ContainsKey(CommonConst.POPUP_CONFIRM))
                DictPopup[CommonConst.POPUP_CONFIRM] = popupConfirm;
            else
                DictPopup.Add(CommonConst.POPUP_CONFIRM, popupConfirm);


            if (DictPopup.ContainsKey(LobbyConst.POPUP_DATA_MANAGER))
                DictPopup[LobbyConst.POPUP_DATA_MANAGER] = popupDataManager;
            else
                DictPopup.Add(LobbyConst.POPUP_DATA_MANAGER, popupDataManager);


            if (DictPopup.ContainsKey(LobbyConst.POPUP_ENDING_HINT))
                DictPopup[LobbyConst.POPUP_ENDING_HINT] = popupEndingHint;
            else
                DictPopup.Add(LobbyConst.POPUP_ENDING_HINT, popupEndingHint);
                
            if (DictPopup.ContainsKey(LobbyConst.POPUP_SPECIAL_HINT))
                DictPopup[LobbyConst.POPUP_SPECIAL_HINT] = popupSpecialHint;
            else
                DictPopup.Add(LobbyConst.POPUP_SPECIAL_HINT, popupSpecialHint);

            if (DictPopup.ContainsKey(GameConst.POPUP_EPISODE_FIRST_REWARD)) 
                DictPopup[GameConst.POPUP_EPISODE_FIRST_REWARD] = popupEpisodeClearReward;
            else 
                DictPopup.Add(GameConst.POPUP_EPISODE_FIRST_REWARD, popupEpisodeClearReward);

            if (DictPopup.ContainsKey(LobbyConst.POPUP_GRADE_BENEFIT_INFO))
                DictPopup[LobbyConst.POPUP_GRADE_BENEFIT_INFO] = popupGradeBenefitInfo;
            else
                DictPopup.Add(LobbyConst.POPUP_GRADE_BENEFIT_INFO, popupGradeBenefitInfo);

            if(DictPopup.ContainsKey(LobbyConst.POPUP_GRADE_EXP)) 
                DictPopup[LobbyConst.POPUP_GRADE_EXP] = popupGradeExp;
            else 
                DictPopup.Add(LobbyConst.POPUP_GRADE_EXP, popupGradeExp);

            if (DictPopup.ContainsKey(LobbyConst.POPUP_GRADE_UP))
                DictPopup[LobbyConst.POPUP_GRADE_UP] = popupGradeUp;
            else
                DictPopup.Add(LobbyConst.POPUP_GRADE_UP, popupGradeUp);


            if (DictPopup.ContainsKey(GameConst.TEMPLATE_GAME_MESSAGE))
                DictPopup[GameConst.TEMPLATE_GAME_MESSAGE] = popupGameMessage;
            else
                DictPopup.Add(GameConst.TEMPLATE_GAME_MESSAGE, popupGameMessage);
                
            if(DictPopup.ContainsKey(CommonConst.POPUP_MESSAGE_ALERT)) 
                DictPopup[CommonConst.POPUP_MESSAGE_ALERT] = popupMessageAlert;
            else 
                DictPopup.Add(CommonConst.POPUP_MESSAGE_ALERT, popupMessageAlert);
                
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

            // Tutorial
            if (DictPopup.ContainsKey(CommonConst.POPUP_TUTORIAL_MISSION_1))
                DictPopup[CommonConst.POPUP_TUTORIAL_MISSION_1] = popupTutorial1;
            else
                DictPopup.Add(CommonConst.POPUP_TUTORIAL_MISSION_1, popupTutorial1);

            if (DictPopup.ContainsKey(CommonConst.POPUP_TUTORIAL_MISSION_2))
                DictPopup[CommonConst.POPUP_TUTORIAL_MISSION_2] = popupTutorial2;
            else
                DictPopup.Add(CommonConst.POPUP_TUTORIAL_MISSION_2, popupTutorial2);

            if (DictPopup.ContainsKey(CommonConst.POPUP_TUTORIAL_MISSION_3))
                DictPopup[CommonConst.POPUP_TUTORIAL_MISSION_3] = popupTutorial3;
            else
                DictPopup.Add(CommonConst.POPUP_TUTORIAL_MISSION_3, popupTutorial3);


            if (DictPopup.ContainsKey("Nickname")) 
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

            if (DictPopup.ContainsKey(GameConst.POPUP_GAME_OPTION))
                DictPopup[GameConst.POPUP_GAME_OPTION] = popupGameOption;
            else
                DictPopup.Add(GameConst.POPUP_GAME_OPTION, popupGameOption);
                
            if(DictPopup.ContainsKey(CommonConst.POPUP_SPECIAL_EPISODE_BUY)) 
                DictPopup[CommonConst.POPUP_SPECIAL_EPISODE_BUY] = popupSpecialEpisodeBuy;
            else 
                DictPopup.Add(CommonConst.POPUP_SPECIAL_EPISODE_BUY, popupSpecialEpisodeBuy);
                
            if(DictPopup.ContainsKey(CommonConst.POPUP_EXPIRE_TOKEN)) 
                DictPopup[CommonConst.POPUP_EXPIRE_TOKEN] = popupExpireToken;
            else 
                DictPopup.Add(CommonConst.POPUP_EXPIRE_TOKEN, popupExpireToken);
        }
        
        #endregion
        
    }
}