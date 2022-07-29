using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Febucci.UI;
using DG.Tweening;


namespace PIERStory {
    public class PopupIntro : PopupBase
    {
        public List<IntroMessage> listIntroMessage; // 4개 인트로 문자메세지 
        public int currentIntroPhase = 0;
        
        public GameObject finger; // 손가락 오브젝트
        public float fingerTimer = 0;
        
        public StoryData selectedStory;

        public GameObject skipButton;
        
        [Space]
        [Header("Phase 1")]
        public CanvasGroup imageBubble; // 말풍선 
        public TextMeshProUGUI textBubble; // 말풍선 텍스트
        public TextMeshProUGUI textArabicBubble; // 아랍 말풍선 텍스트
        public TextAnimator textAnimator;
        public TextAnimatorPlayer textAnimatorPlayer;  // 말풍선 텍스트 플레이어
        
        [Space]
        [Header("Phase 2")]
        public Transform phone1;
        public Image phone1MessageEffect;
        public TextMeshProUGUI textPhone1BottomMessage;
        
        [Space]
        [Header("Phase 3")]
        public Transform phone2;
        public TextMeshProUGUI textPhone2BottomMessage;
        
        
        [Space]
        [Header("Phase 4")]
        [SerializeField] GameObject introduceStory;
        public ViewIntroduce introduceBox;
        public GameObject homeButton;

        
        public override void Show()
        {
            if(isShow)
                return;
            
            base.Show();
            InitControls();
        }
        
        void Update() {
            
            // 손가락 보이고 있을때는 동작하지 않음 
            if(finger.activeSelf)
                return;
            
            if(currentIntroPhase == 1) {
                fingerTimer += Time.deltaTime;
                
                if(fingerTimer > 2) {
                    Debug.Log("Finger Show");
                    SetFinger(new Vector2(120, -150));
                }
            } 
            else if (currentIntroPhase == 3) {
                fingerTimer += Time.deltaTime;
                
                if(fingerTimer > 2) {
                    Debug.Log("Finger Show #2");
                    SetFinger(new Vector2(60, -100));
                }
            }
            else if (currentIntroPhase == 5) {
                fingerTimer += Time.deltaTime;
                
                if(fingerTimer > 2) {
                    Debug.Log("Finger Show #3");
                    SetFinger(new Vector2(230, -320));
                }
            }
        }
        
        void SetFinger(Vector2 __pos) {
            finger.transform.DOKill();
            finger.transform.localPosition = __pos;
            finger.transform.localScale = Vector3.one;
            finger.SetActive(true);
            finger.transform.DOScale(1.1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        
        
        /// <summary>
        ///  초기화
        /// </summary>
        void InitControls() {
            
            currentIntroPhase = -1;
            
            finger.SetActive(false);
            fingerTimer = 0;
            
            // Phase 1
            textBubble.gameObject.SetActive(false);
            textBubble.text = string.Empty;
            textArabicBubble.gameObject.SetActive(false);
            textArabicBubble.text = string.Empty;
            imageBubble.alpha = 0;
            
            // Phase 2
            phone1.gameObject.SetActive(false);
            phone1MessageEffect.color = new Color(1,1,1,0);
            textPhone1BottomMessage.transform.localScale = Vector3.one;
            textPhone1BottomMessage.text = string.Empty;
            
            
            // Phase 3
            phone2.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            phone2.gameObject.SetActive(true);
            phone2.localPosition = new Vector2(0, -1400);
            
            
            // 인트로 메세지 초기화 
            for(int i=0; i<SystemManager.main.introData.Count;i++) {
                if(i >= listIntroMessage.Count)
                    break;
                    
                    
                listIntroMessage[i].Init(SystemManager.main.introData[i], this);
                
            }
            
            
            // Phase 4
            introduceStory.GetComponent<CanvasGroup>().alpha = 0;
            introduceStory.SetActive(false); 
            
            
            // 아랍어에서 추가처리.
            if(SystemManager.main.currentAppLanguageCode == CommonConst.COL_AR) {
                // Destroy(textAnimator);
                // Destroy(textAnimatorPlayer);
                
                textAnimator.enabled = false;
                textAnimatorPlayer.enabled = false;
            }
            
        }
        
        /// <summary>
        /// 다음 페이즈로 넘어가기. 
        /// </summary>
        public void OnClickMoveNextPhase() {
            
            if(currentIntroPhase == 0) {
                
                if(SystemManager.main.currentAppLanguageCode != CommonConst.COL_AR)
                    textAnimatorPlayer.SkipTypewriter();
            }
            else if(currentIntroPhase == 1) {
                Debug.Log(" >> currentIntroPhase 1");
                StartPhase2(); // 페이즈 2 시작 
                
            }
            else if (currentIntroPhase == 3) {
                Debug.Log(" >> currentIntroPhase 3");
                // 3에서 터치하면 페이즈 3으로 변경 
                StartPhase3();
            }
        }
        
        
        
        
        /// <summary>
        /// Container 이벤트가 끝나고 시작되는 페이즈1
        /// </summary>
        public void StartPhase1() {
            Debug.Log("Intro StartPhase1");
            skipButton.SetActive(true);

            imageBubble.DOFade(1,1f).OnComplete(()=> {
                
               currentIntroPhase = 0;
               Debug.Log("StartPhase1 " + SystemManager.main.currentAppLanguageCode);
               
               // 아랍어 구분...
               if(SystemManager.main.currentAppLanguageCode == CommonConst.COL_AR) {
                   Debug.Log("Arabic Intro");
                   
                    // textArabicBubble.text = SystemManager.GetLocalizedText("6326");
                    SystemManager.SetLocalizedText(textArabicBubble, "6326");
                    textArabicBubble.color = new Color(textBubble.color.r, textBubble.color.g, textBubble.color.b, 0);
                    textArabicBubble.gameObject.SetActive(true);
                    textArabicBubble.DOFade(1, 1).OnComplete(OnCompleteTypeWrite);
               }
               else {
                    textBubble.gameObject.SetActive (true);
                    textAnimatorPlayer.ShowText(SystemManager.GetLocalizedText("6326"));
                    textAnimatorPlayer.StartShowingText();
               }
            });
        }
        
        
        public void OnCompleteTypeWrite() {
            currentIntroPhase = 1; // 타이핑 완료되면 1로 변경한다. 
        }
        
        /// <summary>
        /// 페이즈1 감추기 
        /// </summary>
        void HideFinger() {
            finger.SetActive(false);
            fingerTimer = 0;
            
            
        }
        
        
        
        /// <summary>
        /// 페이즈2
        /// </summary>
        public void StartPhase2() {
            
            currentIntroPhase = 2; // 2로 증가시킨다. 
            
            HideFinger();
            
            // 말풍선 사라지게 하고 폰1 등장 
            imageBubble.DOFade(0, 0.5f).OnComplete(()=> {
                phone1.localPosition = new Vector2(0, -1300);
                phone1.gameObject.SetActive(true);
                
                textPhone1BottomMessage.gameObject.SetActive(false);
                
                // 폰 아래에서 위로 등장하고, 추가 처리 
                phone1.DOLocalMoveY(40, 1f).SetDelay(0.5f).SetEase(Ease.OutBack).OnComplete(()=> {
                    phone1MessageEffect.DOFade(1, 1f).SetLoops(-1, LoopType.Yoyo);
                    
                    Handheld.Vibrate(); // 진동 
                    
                    textPhone1BottomMessage.gameObject.SetActive(true);
                    SystemManager.SetLocalizedText(textPhone1BottomMessage, "6292");
                    textPhone1BottomMessage.transform.DOScale(1.1f, 1).SetLoops(-1, LoopType.Yoyo);
                    
                    // 효과 끝나면 3으로 증가 
                    Invoke("OnCompletePhase2", 1);
                });
            });
        }
        void OnCompletePhase2() {
            currentIntroPhase = 3;
            fingerTimer = 0;
        }
        
        
        
        
        
        /// <summary>
        ///  페이즈 3 시작 
        /// </summary>
        public void StartPhase3() {
            currentIntroPhase = 4; // 3페이지 시작되면 4로 변경
            
            HideFinger();
            
            phone1.gameObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(()=> {
                phone2.localPosition = new Vector2(0, -1300);
                phone2.GetComponent<CanvasGroup>().alpha = 1;
                
                // 폰 아래에서 위로 등장하고, 추가 처리 
                phone2.DOLocalMoveY(40, 1f).SetDelay(0.5f).SetEase(Ease.OutBack).OnComplete(()=> {
                    SystemManager.SetLocalizedText(textPhone2BottomMessage, "6292");
                    textPhone2BottomMessage.transform.DOScale(1.1f, 1).SetLoops(-1, LoopType.Yoyo);
                    
                    // 시간차 등장 처리 
                    for(int i=0; i<listIntroMessage.Count;i++) {
                        listIntroMessage[i].gameObject.GetComponent<CanvasGroup>().DOFade(1, 0.4f).SetDelay(i*0.4f);
                        Invoke("OnCompletePhase3", 2f);
                    }
                    
                    
                });                
            });
        }
        
        void OnCompletePhase3() {
            currentIntroPhase = 5;
            fingerTimer = 0;
        }
        
        /// <summary>
        /// 페이즈3 감추기 
        /// </summary>
        void HidePhase3() {
            finger.SetActive(false);
            fingerTimer = 0;
            
            phone2.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }
        
        public void SelectIntroMessage(StoryData __story) {
            
            HidePhase3();
            currentIntroPhase = 6;
            
            introduceStory.SetActive(true);
            introduceStory.GetComponent<CanvasGroup>().DOFade(1, 1f);
            
            selectedStory = __story;
            SystemListener.main.introduceStory = __story;
            introduceBox.SetInfo(__story);

            skipButton.SetActive(false);
            homeButton.SetActive(true);

            // 인트로 완료
            UserManager.main.UpdateIntroComplete();
        }
        
        
        /// <summary>
        /// Start 버튼 클릭
        /// </summary>
        public void StartStory() {
            StoryManager.main.RequestStoryInfo(selectedStory);
            Hide();
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        public void OnClickSkip() {
            
            // 메인화면으로 돌아갈래? 
            SystemManager.ShowSystemPopupLocalize("6328", SkipIntro, null);
        }
        
        void SkipIntro() {
            // 인트로 완료
            UserManager.main.UpdateIntroComplete();
            Hide();
        }
        
    }
}