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
        
        private void Awake() {
            if(main != null) {
                Destroy(this.gameObject);
                return;
            }
            
            main = this;
            
            HideConfetti();
            
            DontDestroyOnLoad(this);
                
        }
        
        /// <summary>
        /// 팝업 매니저 초기화
        /// 각 씬의 담당 매니저에서 따로 호출해줘야한다. (LobbyManager, GameManager)
        /// </summary>
        public void InitPopupManager() {
            
            
            // 팝업 큐 루틴 시작 
            StartCoroutine(PopupQueueRoutine());
        }
        
        
        IEnumerator PopupQueueRoutine() {
            
            PopupQueue.Clear();
            CurrentQueuePopup = null;
            
            while(true) {
                
                yield return null;
                
                // * 현재 보여지고 있는 큐 팝업이 있으면, 대기 
                while(CurrentQueuePopup) {
                    yield return null;
                }
                
                // * 팝업 큐에 팝업이 없으면 대기
                while(PopupQueue.Count < 1) {
                    yield return null;
                }
                
                // 큐에서 하나 가져온다. 
                CurrentQueuePopup = PopupQueue.Dequeue();
                yield return null;
                
                CurrentQueuePopup.Show(); // 보여주기 
            }
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
            
            if(addToPopupQueue)
                PopupQueue.Enqueue(popup);  // 큐를 통해 실행.
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
        
    }
}