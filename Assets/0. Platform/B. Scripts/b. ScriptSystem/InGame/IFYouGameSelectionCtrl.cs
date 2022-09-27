using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;
using BestHTTP;
using DG.Tweening;



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
        public static bool isChooseCompleted = false; // 선택 완료 (다른거 누를 수 없도록 막는다)
        public bool isPurchaseSelection = false;     // 구매해야 하는 선택지인가요?
        public bool hasSelectionHint = false;           // 선택지 힌트가 연결되어 있나요?
        public bool isPurchasedHint
        {
            get
            {
                if (!hasSelectionHint)
                    return true;

                return hasSelectionHint && UserManager.main.IsPurchaseSelectionHint(StoryManager.main.CurrentEpisodeID, scriptRow.selection_group, scriptRow.selection_no);
            }
        }

        #region const & readonly
        static readonly Vector2 originSizeDelta = new Vector2(550, 90); // 기본 크기 
        static readonly Vector2 focusSizeDelta = new Vector2(590, 96); // 포커스 크기 
        const int originPosY = -260; // 기본 첫번재 선택지 높이 250
        const int offsetPosY = 120; // offset  -120
        #endregion


        ScriptRow scriptRow; // 기반 스크립트 로우 
        [SerializeField] SelectionState currentState; // 선택지 애니메이션 상태

        [SerializeField] int selectionIndex = 0; // 선택지 순서 


        [SerializeField] CanvasGroup canvasGroup;   // 캔버스 그룹 
        [SerializeField] Image imageSelection;      // 선택지 버튼 이미지 
        [SerializeField] Image imageBar;            // 선택지 채워짐 바
        [SerializeField] Image imageAura;           // 아우라 이미지 
        public GameObject illustIcon;
        [SerializeField] TextMeshProUGUI textSelection; //  텍스트
        public Image lockIcon;
        public GameObject selectionPrice;
        public TextMeshProUGUI priceText;
        public TextMeshProUGUI salePriceText; // 세일 가격 
        public ImageRequireDownload freepassBadge;
        
        [SerializeField] int originPrice = 0;
        [SerializeField] int saleSelectionPrice = 0;

        [SerializeField] bool isLock = false;       // 잠금 여부 
        [SerializeField] bool isFilling = false;    // 채워짐 
        bool selectionPurchaseStart = false;        // 선택지 구매 시작
        

        string selectionText = string.Empty; // 선택지 문구 
        [SerializeField] string requisite = string.Empty; // 조건 
        [SerializeField] string targetSceneID = string.Empty; // 이동할 사건 ID 
        [SerializeField] float fillAmount = 0; // 게이지 값


        int targetPosY = 0; // 최종적으로 도달할 위치 (Y)
        int appearPosY = 0; // 등장 위치 

        [Space][Header("선택지 힌트")]
        public GameObject selectionHint;
        public GameObject coinBox;
        public TextMeshProUGUI hintPrice;
        
        
        [Space][Header("할인 관련")]
        public Image imageOff;
        public TextMeshProUGUI textOff;
        public GameObject offLine;
        
        public Sprite spriteIFyouCircle;
        public Sprite spriteOnedayCircle;
        
    

        #region static methods



        /// <summary>
        /// 다른 선택지들의 상태를 변경하기 
        /// </summary>
        /// <param name="__selected"></param>
        /// <param name="__state"></param>
        public static void SetOtherSelectionState(IFYouGameSelectionCtrl __selected, SelectionState __state) {

            Debug.Log(string.Format("<color=yellow>SetOtherSelectionState [{0}]/[{1}]</color>", __selected.selectionText, __state.ToString()));

            for (int i = 0; i < ListStacks.Count; i++) {

                // 선택지 리스트를 가지고 있는 경우 활성화 조절
                if (ListStacks[i].hasSelectionHint)
                    ListStacks[i].selectionHint.SetActive(__state == SelectionState.Idle);

                if (ListStacks[i] == __selected) {
                    Debug.Log("SetOtherSelectionState pass itself");
                    continue;
                }

                ListStacks[i].SetState(__state);
            }
        }

        #endregion


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
            originPrice = scriptRow.selectionPrice;

            if (selectionText.Contains("\\"))
                selectionText = selectionText.Replace("\\", "");

            SystemManager.SetText(textSelection, selectionText);

            // 일러스트(이미지, CG) 획득이 있는지
            if (GameManager.main.CheckNextSceneHasIllust(targetSceneID))
                illustIcon.SetActive(true);
            else
                illustIcon.SetActive(false);

            freepassBadge.gameObject.SetActive(false);

            // 과금 선택지 관련 세팅
            // 과금 선택지가 아님
            if (scriptRow.selectionPrice < 0)
            {
                selectionPrice.SetActive(false);
            }
            else
            {
                // 여기부터 과금 선택지
                // 프리패스 보유 여부
                if (UserManager.main.HasProjectFreepass())
                {
                    Debug.Log("## PremiumPass Setting in IFYouGameSelectionCtrl");
                    
                    selectionPrice.SetActive(false);
                    freepassBadge.gameObject.SetActive(true);
                    
                    // * 2022.05.25 올패스를 위한 체크 추가
                    if(UserManager.main.HasProjectPremiumPassOnly(StoryManager.main.CurrentProjectID)) {
                        // 프리미엄 패스 있으면 프리미엄 패스 이미지 사용하고 
                        freepassBadge.SetDownloadURL(StoryManager.main.freepassBadgeURL, StoryManager.main.freepassBadgeKey);    
                    }
                    else {
                        // 없으면 올패스 이미지를 사용한다.
                        freepassBadge.GetComponent<Image>().sprite = SystemManager.main.spriteAllPassIcon;
                    }

                }
                else
                {
                    selectionPrice.SetActive(true);

                    // 선택지를 구매한 적이 있다면 가격을 표시하지 않음
                    if (UserManager.main.IsPurchaseSelection(StoryManager.main.CurrentEpisodeID, scriptRow.selection_group, scriptRow.selection_no))
                        priceText.text = string.Empty;
                    else
                    {
                        isPurchaseSelection = true;

                        saleSelectionPrice = scriptRow.selectionPrice;
                        priceText.text = string.Format("{0}", saleSelectionPrice); // 원 가격 설정 
                        

                        if (UserManager.main.ifyouPassDay > 0) {
                            
                            // 스프라이트, 텍스트 처리 
                            imageOff.sprite = spriteIFyouCircle;
                            textOff.text = BillingManager.main.ifyouPassChoiceSale.ToString() + "\n<size=16>OFF</size>";
                            
                            // 할인 가격 설정 
                            saleSelectionPrice = (int)(scriptRow.selectionPrice * (1f - BillingManager.main.ifyouPassChoiceSaleFloat));
                        }

                        // 원데이 패스의 할인율이 더 크다 
                        if(StoryManager.main.CurrentProject.IsValidOnedayPass()) {
                            
                            imageOff.sprite = spriteOnedayCircle;
                            textOff.text = BillingManager.main.onedayPassChoiceSale.ToString() + "\n<size=16>OFF</size>";
                            
                            
                            saleSelectionPrice = (int)(scriptRow.selectionPrice * (1f - BillingManager.main.onedayPassChoiceSaleFloat));
                        }
                        
                        
                        if(UserManager.main.ifyouPassDay > 0 || StoryManager.main.CurrentProject.IsValidOnedayPass()) {
                            imageOff.gameObject.SetActive(true);
                            offLine.SetActive(true);
                            priceText.color = new Color(priceText.color.r, priceText.color.g, priceText.color.b, 0.7f);
                            
                            offLine.SetActive(true);
                            salePriceText.gameObject.SetActive(true);
                            
                            salePriceText.text = string.Format("{0}", saleSelectionPrice);;
                        }                        

                        
                    }
                }
            }

            // 잠금 여부 설정 
            SetLockStatus();

            // targetScene 설정 
            // ! 없으면 안됨. 
            if (string.IsNullOrEmpty(targetSceneID))
                SystemManager.ShowMessageAlert("이동해야 하는 사건ID 정보 없음");

            // 선택지 힌트 설정
            SetSelectionHintState();


            // 위치 잡기
            InitPosition();


            // 상태 지정 
            SetState(SelectionState.Appear);

        }

        /// <summary>
        ///  초기화 
        /// </summary>
        void InitSelection()
        {
            isOneOfSelectionPointerDown = false;

            this.transform.localScale = Vector3.one; // 스케일 

            imageSelection.sprite = GameManager.main.spriteSelectionNormalBase; // 버튼 스프라이트

            // 크기SizeDelta 초기화 
            imageSelection.rectTransform.sizeDelta = originSizeDelta;
            imageAura.color = new Color(1, 1, 1, 0); // 투명하게. 
            imageAura.gameObject.SetActive(true);
            
            imageOff.gameObject.SetActive(false);
            offLine.SetActive(false);
            salePriceText.gameObject.SetActive(false); 
            priceText.color = new Color(priceText.color.r, priceText.color.g, priceText.color.b, 1);

            // 변수들 초기화 
            isLock = false;
            isFilling = false;
            fillAmount = 0;
            requisite = string.Empty;

            canvasGroup.alpha = 0; // 알파값 초기화 
            imageBar.fillAmount = fillAmount;


            ListStacks.Add(this);
        }


        void SetLockStatus()
        {
            // 조건 컬럼에 값이 없으면 그냥 끝!
            if (string.IsNullOrEmpty(requisite))
            {
                lockIcon.gameObject.SetActive(false);
                return;
            }

            // imageSelection.sprite = GameSpriteHolder.main.spriteSelectionNormal

            // 잠금 여부 
            isLock = !ScriptExpressionParser.main.ParseScriptExpression(requisite);
            lockIcon.gameObject.SetActive(true);

            // 잠금여부에 따른 스프라이트 변경
            if (isLock)
            {
                imageSelection.sprite = GameManager.main.spriteSelectionLockedBase;
                lockIcon.sprite = GameManager.main.spriteSelectionLockIcon;
                // textSelection.text = string.Empty; // 잠긴 선택지에서는 텍스트 보이지 않음 
            }
            else
            {
                imageSelection.sprite = GameManager.main.spriteSelectionUnlockedBase;
                lockIcon.sprite = GameManager.main.spriteSelectionUnlockIcon;
            }
        }


        void SetSelectionHintState()
        {
            hasSelectionHint = SelectionHintPrice() > 0;
            selectionHint.SetActive(hasSelectionHint);

            // 선택지 힌트가 없으면 함수 종료!
            if (!hasSelectionHint)
                return;

            // 22.07.18 프리미엄 패스 유저의 경우 선택지 힌트를 무료로 제공함
            // 22.07.19 원데이 패스 유저도 선택지 힌트를 무료로 제공함
            hintPrice.text = string.Format("{0}", SelectionHintPrice());
            coinBox.SetActive(!UserManager.main.IsPurchaseSelectionHint(StoryManager.main.CurrentEpisodeID, scriptRow.selection_group, scriptRow.selection_no) && !UserManager.main.HasProjectFreepass() && !StoryManager.main.CurrentProject.IsValidOnedayPass());
        }


        /// <summary>
        /// 위치잡기 
        /// </summary>
        void InitPosition() {

            // 위치잡기 
            targetPosY = originPosY + (selectionIndex * offsetPosY); // 타겟 위치 
            //appearPosY = targetPosY - 100; // 등장 위치 
            appearPosY = targetPosY + 100; // 등장 위치 

            // 시작 위치 지정하고.
            transform.localPosition = new Vector2(0, appearPosY);
        }
        

        #region 포인터 다운, 업 

        public void OnPointerDown(PointerEventData eventData) {
            if (isChooseCompleted)
                return;

            if (this.currentState == SelectionState.None || this.currentState == SelectionState.Appear)
                return;

            // 잠금 상태의 선택지는 선택불가
            if (isLock)
                return;

            // 멀티터치 방지
            if (isOneOfSelectionPointerDown)
                return;

            // 두개 이상 터치 막는다. 
            if (Input.touchCount > 1)
                return;


            Debug.Log("OnPointerDown : " + this.gameObject);
            isOneOfSelectionPointerDown = true;

            SetState(SelectionState.Fill);

            // 다른 선택지들 unselect 상태로 변경하기 
            SetOtherSelectionState(this, SelectionState.Unselect);
        }

        public void OnPointerUp(PointerEventData eventData) {
            if (this.currentState == SelectionState.None
                || this.currentState == SelectionState.Select
                || this.currentState == SelectionState.Appear
                || this.currentState == SelectionState.Out)
                return;

            if (isChooseCompleted)
                return;


            // 멀티 터치 방지
            if (!isOneOfSelectionPointerDown)
                return;

            // 두개 이상 터치 막는다. 
            if (Input.touchCount > 1)
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

            switch (__state) {

                case SelectionState.Select: // 선택완료 

                    // 돈을 내야하는데, 구매한적도 없고, 심지어 슈퍼유저도 아니면
                    if (scriptRow.selectionPrice > 0 && !UserManager.main.IsPurchaseSelection(StoryManager.main.CurrentEpisodeID, scriptRow.selection_group, scriptRow.selection_no) && !UserManager.main.CheckAdminUser())
                        SelectionProcess();
                    else
                        SelectionSelected();

                    break;

                case SelectionState.Fill:
                    // imageSelection.rectTransform.DOSizeDelta(focusSizeDelta, 0.3f); // 사이즈 커진다. 
                    imageAura.DOFade(1, 0.3f); // 아우라 알파값!

                    imageBar.DOKill();
                    imageBar.DOFillAmount(1, 0.6f).SetEase(Ease.Linear).OnComplete(() => {
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
                    this.transform.DOLocalMoveY(targetPosY, 0.4f).OnComplete(() => { SetState(SelectionState.Idle); });
                    break;

                case SelectionState.Out: // 퇴장처리 
                    canvasGroup.DOKill();
                    ViewGame.main.selectionInfo.DOFade(0f, 0.4f);
                    canvasGroup.DOFade(0, 0.4f).OnComplete(() => {
                        SetState(SelectionState.None);
                        ViewGame.main.selectionInfoText.text = string.Empty;
                    });

                    imageAura.DOKill();
                    imageAura.DOFade(0, 0.3f);
                    // this.transform.DOLocalMoveY(appearPosY, 0.4f).OnComplete(()=> { SetState(SelectionState.None); });
                    break;

                case SelectionState.None:
                    this.gameObject.SetActive(false);

                    // 최대한 원상복구 해보자 
                    canvasGroup.alpha = 1;
                    imageAura.color = new Color(1, 1, 1, 1);
                    imageAura.gameObject.SetActive(false);

                    // ViewGame 메소드 호출 
                    ViewGame.main.RemoveListAppearSelection(this);

                    break;


            }
        } // ? end of SetState

        
        /// <summary>
        /// 선택 완료.
        /// </summary>
        void ChooseSelection() {
            Debug.Log(string.Format(">>> {0} <<<", selectionText));

            isChooseCompleted = true; // 완료처리 

            // 로그 만들어주기. 
            ViewGame.main.CreateSelectionLog(selectionText);

            // 이동할 사건ID와 선택지 버튼 index를 함께 넘겨준다. 
            ViewGame.main.ChooseSelection(targetSceneID, selectionIndex);

            // 선택지 선택 후 서버 통신 진행 (선택지 경로 저장 )
            NetworkLoader.main.UpdateUserSelectionProgress(targetSceneID, selectionText);

            // 선택지 기록 쌓기
            NetworkLoader.main.UpdateUserSelectionCurrent(targetSceneID, scriptRow.selection_group, scriptRow.selection_no);

            // 선택지 튜토리얼 완료 처리 
            if (!UserManager.main.isSelectionTutorialClear) {
                UserManager.main.RequestSelectionTutorialClear();
            }

            if (ViewGame.main.commonTop.isVisible)
                ViewGame.main.commonTop.Hide();
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

        /// <summary>
        /// 선택지 게이지 다 채워진 것에 대한 처리
        /// </summary>
        void SelectionProcess()
        {
            // 프리패스도 아닌데 돈도 없어!
            if (!UserManager.main.HasProjectFreepass() && !UserManager.main.CheckGemProperty(saleSelectionPrice))
            {
                SystemManager.ShowLackOfCurrencyPopup(true, "6321", saleSelectionPrice);

                SetState(SelectionState.Idle);
                SetOtherSelectionState(this, SelectionState.Idle);
                return;
            }


            // 프리패스 이용자이면 가격을 0원 처리 한다
            if (UserManager.main.HasProjectFreepass())
                saleSelectionPrice = 0;

            // 이중 처리 방지
            if (selectionPurchaseStart)
                return;

            selectionPurchaseStart = true;
            UserManager.main.PurchaseSelection(scriptRow.selection_group, scriptRow.selection_no, saleSelectionPrice, CallbackPurchaseSelection);
        }

        /// <summary>
        /// 선택지 선택 완료됨
        /// </summary>
        void SelectionSelected()
        {
            // 트윈 후, 나머지 퇴장처리 
            transform.DOKill();
            transform.DOScale(1.1f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {

                Debug.Log("Select State! Set Others to Out");

                SetOtherSelectionState(this, SelectionState.Out);
                // 파티클
                //particleSelect.gameObject.SetActive(true);
                //particleSelect.Play();
            });

            imageBar.DOKill();
            imageBar.DOFillAmount(1, 0.3f).SetDelay(1).OnComplete(() =>
            {
                SetState(SelectionState.Out);
                ChooseSelection(); // 선택완료처리
            });
        }


        void CallbackPurchaseSelection(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackPurchaseSelection");
                selectionPurchaseStart = false;
                return;
            }

            LitJson.JsonData result = LitJson.JsonMapper.ToObject(res.DataAsText);

            // 노드 갱신해주고
            UserManager.main.SetPurchaseSelection(result);
            UserManager.main.SetBankInfo(result);

            AdManager.main.isPaidSelection = true; // 유료 선택지 선택됨

            // 22.04.06 과금 선택지 5회 업적 클리어에 대한 조건문 추가하기
            // NetworkLoader.main.RequestIFYOUAchievement(5);

            // NetworkLoader.main.RequestIFYOUAchievement(20);

            selectionPurchaseStart = false;

            // 선택지 선택완료 처리
            SelectionSelected();
        }


        /// <summary>
        /// 선택지 힌트 클릭
        /// </summary>
        public void OnClickSelectionHint()
        {
            // 힌트를 구매한적 있는지 체크하고, 구매한 적 있으면 바로 팝업 띄워주고
            // 22.07.18 프리미엄 패스 유저는 힌트가 무료임
            // 22.07.19 원데이 패스 유저도 힌트가 무료임
            if (UserManager.main.HasProjectFreepass() || StoryManager.main.CurrentProject.IsValidOnedayPass() || UserManager.main.IsPurchaseSelectionHint(StoryManager.main.CurrentEpisodeID, scriptRow.selection_group, scriptRow.selection_no))
            {
                ShowSelectionHintPopup();
                return;
            }

            // 구매 기록이 없다면 통신 완료 후, 선택지 힌트 팝업을 띄워주는데, 그 전에 코인 갯수를 체크해서 부족하면 상점
            if(!UserManager.main.CheckCoinProperty(SelectionHintPrice()))
            {
                SystemManager.ShowLackOfCurrencyPopup(false, "6324", SelectionHintPrice());
                return;
            }

            UserManager.main.RequestSelectionHint(scriptRow.selection_group, scriptRow.selection_no, CallbackPurchaseSelectionHint);
            selectionHint.GetComponent<Button>().interactable = false;
        }


        void ShowSelectionHintPopup()
        {
            StoryManager.main.selectedEndingHintList.Clear();

            // 선택된 엔딩힌트 리스트 쌓기
            foreach (EndingHintData hintData in StoryManager.main.endingHintList)
            {
                for (int i = 0; i < hintData.unlockScenes.Length; i++)
                {
                    if (hintData.unlockScenes[i] == targetSceneID)
                    {
                        StoryManager.main.selectedEndingHintList.Add(hintData);
                        break;
                    }
                }
            }

            PopupBase p = PopupManager.main.GetPopup(GameConst.POPUP_SELECTION_HINT);

            if (p == null)
            {
                Debug.LogError("선택지 힌트 팝업이 없음!");
                return;
            }

            p.Data.SetLabelsTexts(string.Format("‘ {0}’", scriptRow.script_data));
            PopupManager.main.ShowPopup(p, false);
        }

        int SelectionHintPrice()
        {
            int totalPrice = 0;

            foreach (EndingHintData hintData in StoryManager.main.endingHintList)
            {
                for (int i = 0; i < hintData.unlockScenes.Length; i++)
                {
                    if (hintData.unlockScenes[i] == targetSceneID)
                    {
                        totalPrice += hintData.price;
                        break;
                    }
                }
            }

            return totalPrice;
        }


        void CallbackPurchaseSelectionHint(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("Failed CallbackPurchaseSelectionHint");
                selectionHint.GetComponent<Button>().interactable = true;
                return;
            }

            LitJson.JsonData result = LitJson.JsonMapper.ToObject(res.DataAsText);

            // 선택지 힌트 구매목록 갱신
            UserManager.main.SetSelectionHint(result);
            UserManager.main.SetBankInfo(result);

            coinBox.SetActive(!UserManager.main.IsPurchaseSelectionHint(StoryManager.main.CurrentEpisodeID, scriptRow.selection_group, scriptRow.selection_no));

            ShowSelectionHintPopup();
            selectionHint.GetComponent<Button>().interactable = true;
        }
    }
}