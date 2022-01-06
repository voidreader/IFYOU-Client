using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using Coffee.UIExtensions;


namespace PIERStory {
    
    /// <summary>
    /// 선택지 상태다!
    /// </summary>
    public enum SelectionState {
        None, // 최초 시작 상태 
        Idle, // 등장 완료 후 그냥 가만히 있는 상태 
        Appear, // 등장 상태 
        Fill, // 터치로 인해 채워지고 '있는' 상태 
        Unselect, // 터치된 선택지 이외의 선택지들. 
        Select, // 게이지가 모두 채워져서 '선택이 완료'된 상태 
        Out // 선택이 완료되고 나서 선택지들의 퇴장 상태 
    }
    
    public class IFYouGameSelectionCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        
        public static List<IFYouGameSelectionCtrl> ListStacks = new List<IFYouGameSelectionCtrl>(); // 현재 선택지에 사용되는 친구들 
        static bool isOneOfSelectionPointerDown = false; // 선택지 중 하나라도 누르고 있는 경우 true로 변환 
        
        
        #region const & readonly
        static readonly Vector2 originSizeDelta = new Vector2(600, 80); // 기본 크기 
        static readonly Vector2 focusSizeDelta = new Vector2(640, 86); // 포커스 크기 
        const int originPosY = 250; // 기본 첫번재 선택지 높이 
        const int offsetPosY = -120; // offset 
        #endregion
        
        
        ScriptRow scriptRow; // 기반 스크립트 로우 
        [SerializeField] SelectionState currentState; // 선택지 애니메이션 상태
        
        [SerializeField] int selectionIndex = 0; // 선택지 순서 
        
        
        [SerializeField] CanvasGroup canvasGroup; // 캔버스 그룹 
        [SerializeField] Image imageSelection; // 선택지 버튼 이미지 
        [SerializeField] Image imageBar; // 선택지 채워짐 바
        [SerializeField] Image imageAura; // 아우라 이미지 
        [SerializeField] TextMeshProUGUI textSelection; //  텍스트
        [SerializeField] bool isButtonSelected = false; // 버튼 선택됨!
        [SerializeField] bool isLock = false; // 잠금 여부 
        [SerializeField] bool isFilling = false; // 채워짐 
        [SerializeField] UIParticle particleSelect; // 선택지 선택시 발생 파티클 
        
        string selectionText = string.Empty; // 선택지 문구 
        [SerializeField] string requisite = string.Empty; // 조건 
        [SerializeField] string targetSceneID = string.Empty; // 이동할 사건 ID 
        [SerializeField] float fillAmount = 0; // 게이지 값
        
        int targetPosY = 0; // 최종적으로 도달할 위치 (Y)
        int appearPosY = 0; // 등장 위치 
        
        #region static methods
        
        
        
        /// <summary>
        /// 다른 선택지들의 상태를 변경하기 
        /// </summary>
        /// <param name="__selected"></param>
        /// <param name="__state"></param>
        public static void SetOtherSelectionState(IFYouGameSelectionCtrl __selected, SelectionState __state) {
            
            Debug.Log(string.Format("<color=yellow>SetOtherSelectionState [{0}]/[{1}]</color>", __selected.selectionText, __state.ToString()));
            
            for(int i=0; i<ListStacks.Count;i++) {
                
                if(ListStacks[i] == __selected)
                    continue;
                    
                ListStacks[i].SetState(__state);
            }
        }
        
        #endregion
        
        // Start is called before the first frame update
        void Start()
        {
            
        }


        
        
        /// <summary>
        /// TEST MODE
        /// </summary>
        /// <param name="__index"></param>
        public void SetSelection(int __index) {
            InitSelection();
            
            selectionIndex = __index;
            textSelection.text = "테스트 선택지 " + __index.ToString();
            
            // 위치 잡기
            InitPosition();
            
            // 상태 지정 
            SetState(SelectionState.Appear);
        }
        
        /// <summary>
        /// 인게임 선택지 세팅
        /// </summary>
        /// <param name="__row"></param>
        /// <param name="__index"></param>
        public void SetSelection(ScriptRow __row, int __index) {
            
            // 초기화 
            InitSelection();    
            
            selectionIndex = __index;
            scriptRow = __row;
            selectionText = scriptRow.script_data;
            requisite = scriptRow.requisite;
            targetSceneID = scriptRow.target_scene_id;
            
            textSelection.text = selectionText;
            
            
            // 잠금 여부 설정 
            SetLockStatus();
            
            // targetScene 설정 
            // ! 없으면 안됨. 
            if(string.IsNullOrEmpty(targetSceneID)) {
                SystemManager.ShowAlert("이동해야하는 사건ID 정보 없음");
            }
            
            // 위치 잡기
            InitPosition();

            
            // 상태 지정 
            SetState(SelectionState.Appear);

        }
        
        /// <summary>
        /// 위치잡기 
        /// </summary>
        void InitPosition() {
                        
            // 위치잡기 
            targetPosY = originPosY + (selectionIndex * offsetPosY); // 타겟 위치 
            appearPosY = targetPosY - 100; // 등장 위치 
            
            // 시작 위치 지정하고.
            this.transform.localPosition = new Vector2(0, appearPosY); 
        }
        
        
        /// <summary>
        ///  초기화 
        /// </summary>
        void InitSelection() {
            
            isOneOfSelectionPointerDown = false;
            
            this.transform.localScale = Vector3.one; // 스케일 
            
            imageSelection.sprite = GameSpriteHolder.main.spriteSelectionNormal; // 버튼 스프라이트
            
            // 크기SizeDelta 초기화 
            imageSelection.rectTransform.sizeDelta = originSizeDelta;
            imageAura.color = new Color(1,1,1,0); // 투명하게. 
            imageAura.gameObject.SetActive(true);
            
            
            // 변수들 초기화 
            isButtonSelected = false;
            isLock = false;
            isFilling = false;
            fillAmount = 0;
            requisite = string.Empty;
            
            canvasGroup.alpha = 0; // 알파값 초기화 
            imageBar.fillAmount = fillAmount;
            
            // 파티클
            particleSelect.gameObject.SetActive(false);
            
            ListStacks.Add(this);
        }
        
        #region 포인터 다운, 업 
        
        public void OnPointerDown(PointerEventData eventData) {
            if(this.currentState == SelectionState.None || this.currentState == SelectionState.Appear) 
                return;
                
            // 잠금 상태의 선택지는 선택불가
            if(isLock)
                return;
                
            // 멀티터치 방지
            if(isOneOfSelectionPointerDown)
                return;
                
            
            Debug.Log("OnPointerDown : " + this.gameObject);
            isOneOfSelectionPointerDown = true;
            
            SetState(SelectionState.Fill);
            
            // 다른 선택지들 unselect 상태로 변경하기 
            SetOtherSelectionState(this, SelectionState.Unselect);
        }
        
        public void OnPointerUp(PointerEventData eventData) {
            if(this.currentState == SelectionState.None 
                || this.currentState == SelectionState.Select
                || this.currentState == SelectionState.Appear
                || this.currentState == SelectionState.Out)  
                return;
                
                
            // 멀티 터치 방지
            if (!isOneOfSelectionPointerDown)
                return;
            
            Debug.Log("OnPointerUp : " + this.gameObject);
            
            isOneOfSelectionPointerDown = false;
            SetState(SelectionState.Idle);
            
            // 다른 선택지들 Idle 상태로 변경
            SetOtherSelectionState(this, SelectionState.Idle);
            
        }
        
        #endregion
        
        
        /// <summary>
        /// 상태 변경 
        /// </summary>
        /// <param name="__state"></param>
        public void SetState(SelectionState __state) {
            
            // unselect에서 다른 상태로 돌아가는 경우 
            /*
            if(currentState == SelectionState.Unselect && currentState != __state) {
                SetCanvasGroupAlphaTween(1);
            }
            */
            
            currentState = __state;
            
            switch(__state) {
                
                case SelectionState.Select: // 선택완료 
                
                // 트윈 후, 나머지 퇴장처리 
                this.transform.DOKill();
                this.transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(()=>{ 
                    
                    Debug.Log("Select State! Set Others to Out");
                    
                    SetOtherSelectionState(this, SelectionState.Out); 
                    // 파티클
                    particleSelect.gameObject.SetActive(true);
                    particleSelect.Play();
                });
                
                imageBar.DOKill();
                imageBar.DOFillAmount(0, 0.3f).SetDelay(1).OnComplete(()=>{ 
                    SetState(SelectionState.Out); 
                    
                    ChooseSelection(); // 선택완료처리
                    
                });;
                
                break;
                
                case SelectionState.Fill:
                imageSelection.rectTransform.DOSizeDelta(focusSizeDelta, 0.3f); // 사이즈 커진다. 
                imageAura.DOFade(1, 0.3f); // 아우라 알파값!
                
                imageBar.DOKill();
                imageBar.DOFillAmount(1, 1f).SetEase(Ease.Linear).OnComplete(() => {
                    SetState(SelectionState.Select);
                });
                
                isFilling = true;
                
                break;
                
                case SelectionState.Idle:
                
                SetCanvasGroupAlphaTween(1);
                
                // 게이지 원상태로 돌려놓기.
                SetFillMountZero();
                
                this.transform.DOScale(Vector3.one, 0.2f); // 
                imageSelection.rectTransform.DOSizeDelta(originSizeDelta, 0.1f);
                imageAura.DOFade(0, 0.2f); // 아우라 알파값!
                
                isFilling = false;
                
                break;
                
                case SelectionState.Unselect: // 선택받지 못함 
                
                // 게이지 원상태로 돌려놓기.
                SetFillMountZero();
                
                this.transform.DOScale(Vector3.one * 0.9f, 0.2f); // 크기 조정하고 
                SetCanvasGroupAlphaTween(0.8f); // 살짝 투명하게 
                imageAura.DOFade(0, 0.2f); // 아우라 알파값!
                
                break;
                
                case SelectionState.Appear: 
                this.gameObject.SetActive(true);
                canvasGroup.DOFade(1, 0.3f);
                this.transform.DOLocalMoveY(targetPosY, 0.4f).OnComplete(()=> { SetState(SelectionState.Idle); });
                break;
                
                case SelectionState.Out: // 퇴장처리 
                canvasGroup.DOKill();
                canvasGroup.DOFade(0, 0.4f).OnComplete(()=> { 
                    SetState(SelectionState.None); 
                });
                
                imageAura.DOKill();
                imageAura.DOFade(0, 0.3f);
                // this.transform.DOLocalMoveY(appearPosY, 0.4f).OnComplete(()=> { SetState(SelectionState.None); });
                break;
                
                case SelectionState.None:
                this.gameObject.SetActive(false);
                
                // 최대한 원상복구 해보자 
                canvasGroup.alpha = 1;
                imageAura.color = new Color(1,1,1,1);
                imageAura.gameObject.SetActive(false);
                
                // ViewGame 메소드 호출 
                ViewGame.main.RemoveListAppearSelection(this);
                
                break;
        

            }
        } // ? end of SetState
        
        void SetLockStatus() {
            
            // 조건 컬럼에 값이 없으면 그냥 끝!
            if (string.IsNullOrEmpty(requisite))
                return;
                
            // imageSelection.sprite = GameSpriteHolder.main.spriteSelectionNormal
                
            // 잠금 여부 
            isLock = !ScriptExpressionParser.main.ParseScriptExpression(requisite);
            
            // 잠금여부에 따른 스프라이트 변경
            if(isLock) {
                imageSelection.sprite = GameSpriteHolder.main.spriteSelectionLock;
            }
            else {
                imageSelection.sprite = GameSpriteHolder.main.spriteSelectionUnlock;
            }
            
        }
        
        
        /// <summary>
        /// 선택 완료.
        /// </summary>
        void ChooseSelection() {
            Debug.Log(string.Format(">>> {0} <<<", selectionText));
            
            // 
            
            // 로그 만들어주기. 
            ViewGame.main.CreateSelectionLog(selectionText);

            // 이동할 사건ID와 선택지 버튼 index를 함께 넘겨준다. 
            ViewGame.main.ChooseSelection(targetSceneID, selectionIndex);

            // 선택지 선택 후 서버 통신 진행 (선택지 경로 저장 )
            NetworkLoader.main.UpdateUserSelectionProgress(targetSceneID, selectionText);

            // 선택지 기록 쌓기
            NetworkLoader.main.UpdateUserSelectionCurrent(targetSceneID, scriptRow.selection_group, scriptRow.selection_no);
        }
        
        /// <summary>
        /// 캔버스 그룹 알파 조정 
        /// </summary>
        /// <param name="__v"></param>
        void SetCanvasGroupAlphaTween(float __v) {
            canvasGroup.DOKill();
            canvasGroup.DOFade(__v, 0.2f);
        }
        
        
        void SetFillMountZero() {
            // 게이지 원상태로 돌려놓기.
            imageBar.DOKill();
            imageBar.DOFillAmount(0, 0.1f);
        }
    }
}