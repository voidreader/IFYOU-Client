using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using Coffee.UIExtensions;

namespace PIERStory
{
    /// <summary>
    /// 선택지 버튼 스크립트입니다!
    /// </summary>
    public class GameSelectionCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        ScriptRow scriptRow;

        public static List<GameSelectionCtrl> ListStacks = new List<GameSelectionCtrl>(); // 현재 선택지에 사용되는 친구들 
        static bool isOneOfSelectionPointerDown = false; // 선택지 중 하나라도 누르고 있는 경우 true로 변환 

        [SerializeField] Animator anim; // 애니메이터 

        [Space]
        [SerializeField] int selectionIndex = 0; // 선택지 순서 
        [SerializeField] Image buttonImage; // 버튼 본인 
        [SerializeField] TextMeshProUGUI textSelection; // 
        [SerializeField] Image auraImage; // 뒤 후광 이미지 

        [SerializeField] SelectionAnimationReceiver animEventReceiver; // 애니메이션 이벤트 Receiver. 

        [SerializeField] Image lockIconImage; // 자물쇠 이미지 

        [SerializeField] UIParticle particleSelect; // 선택지 선택시 발생 파티클 


        [Header("선택지 잠금")]
        [SerializeField] bool isLock = false; // 잠금 여부 
        

        [Header("선택된 버튼")]
        [SerializeField] bool isButtonSelected = false; // 버튼 선택됨!
        public bool isReleasing = false; // 누르고 있다가 뗀 경우 돌아가기 위한 시간이 필요해. 

        [Space]
        [SerializeField] string targetSceneID = string.Empty; // 이동할 상황 ID 

        // 텍스트 값
        string selectionScript = string.Empty;
        [SerializeField] string requisite = string.Empty; // 조건 

        int targetPosY = 0; // 최종적으로 도달할 위치 (Y)
        int appearPosY = 0; // 등장 위치 

        // 위치 잡기. 연출을 위해서 Vertical Layout을 사용하지 않는다.
        const int originPosY = 250;
        const int offsetPosY = -120;
        const float offsetDelayTime = 0.2f;

        void Start()
        {
            animEventReceiver.SetSelectionBase(this);
        }

        /// <summary>
        /// 변수 초기화 
        /// </summary>
        void InitSelection()
        {
            selectionIndex = -1;
            isButtonSelected = false;
            isLock = false;
            isReleasing = false;


            // 컬러 초기화 
            auraImage.color = CommonConst.COLOR_IMAGE_TRANSPARENT;
            textSelection.color = CommonConst.COLOR_GRAY_TRANSPARENT;
            buttonImage.color = CommonConst.COLOR_IMAGE_TRANSPARENT;

            auraImage.gameObject.SetActive(true);

            // 기본 이미지 설정
            buttonImage.sprite = GameManager.main.spriteSelectionNormalBase;
            lockIconImage.sprite = GameManager.main.spriteSelectionUnlockIcon;
            lockIconImage.gameObject.SetActive(false);

            this.transform.localScale = Vector3.one;

            // 리스트에 본인 추가
            ListStacks.Add(this);
            isOneOfSelectionPointerDown = false;

            particleSelect.gameObject.SetActive(false);
        }


        /// <summary>
        /// 잠금 상태에 대한 처리 
        /// </summary>
        void SetLockStatus()
        {

            // 조건 컬럼에 값이 없으면 그냥 끝!
            if (string.IsNullOrEmpty(requisite))
                return;

            // 조건 컬럼에 값이 있을때만 ! 
            lockIconImage.gameObject.SetActive(true); // 조건이 있는 경우는 잠금 아이콘 활성화
            // 도와주세요 ExpressionParser!
            isLock = !ScriptExpressionParser.main.ParseScriptExpression(requisite);

            // 초기화때 lock이 false 이기 때문에, true 일때만 처리해주면 된다. 
            if (isLock)
            {
                buttonImage.sprite = GameManager.main.spriteSelectionLockedBase;
                lockIconImage.sprite = GameManager.main.spriteSelectionLockIcon;

                auraImage.gameObject.SetActive(false); // 휘광 꺼버리자 
            }
            else
            {
                buttonImage.sprite = GameManager.main.spriteSelectionUnlockedBase;
                lockIconImage.sprite = GameManager.main.spriteSelectionUnlockIcon;
                auraImage.gameObject.SetActive(true); // 휘광 꺼버리자 
            }

            // 크기 조정 
            if (lockIconImage.gameObject.activeSelf)
                lockIconImage.SetNativeSize();

        }


        /// <summary>
        /// 선택지 버튼 세팅하기 
        /// </summary>
        /// <param name="__row"></param>
        public void SetSelection(ScriptRow __row, int __index)
        {
            // 초기화 
            InitSelection();

            selectionIndex = __index;
            scriptRow = __row;
            selectionScript = scriptRow.script_data;
            requisite = scriptRow.requisite;

            // 잠금 여부 설정 
            SetLockStatus();

            textSelection.text = selectionScript; // 텍스트 세팅
            targetSceneID = scriptRow.target_scene_id;


            if (string.IsNullOrEmpty(targetSceneID))
                textSelection.text = textSelection.text + "(이동정보없음)";


            // 위치잡기 추가 2021.07.12
            targetPosY = originPosY + (__index * offsetPosY);
            appearPosY = targetPosY - 100;


            // 시작 위치 지정하고.
            transform.localPosition = new Vector3(0, targetPosY, 0);

            // 애니메이션 시작 트리거 
            SetAnimationState("Base");
            gameObject.SetActive(true);

            StartCoroutine(RoutineAppearSelection(offsetDelayTime * __index));
        }

        IEnumerator RoutineAppearSelection(float __delayTime)
        {
            // 순차적으로 등장시키고 싶다!
            yield return new WaitForSeconds(__delayTime);
            SetAnimationState("Appear");
        }


        /// <summary>
        /// 애니메이션 트리거 
        /// </summary>
        /// <param name="__trigger"></param>
        public void SetAnimationState(string __trigger)
        {
            if (!this.gameObject.activeSelf)
                return;

            anim.SetTrigger(__trigger);
        }


        /// <summary>
        /// 선택 완료! (게이지 완료 후 처리)
        /// </summary>
        public void ChooseSelection()
        {
            Debug.Log(string.Format(">>> {0} <<<", selectionScript));
            isButtonSelected = true; // 선택됨! 

            // 로그 만들어주기. 
            ViewGame.main.CreateSelectionLog(selectionScript);

            // 이동할 사건ID와 선택지 버튼 index를 함께 넘겨준다. 
            ViewGame.main.ChooseSelection(targetSceneID, selectionIndex);

            // 선택지 선택 후 서버 통신 진행 (선택지 경로 저장 )
            NetworkLoader.main.UpdateUserSelectionProgress(targetSceneID, selectionScript);

            // 선택지 기록 쌓기
            NetworkLoader.main.UpdateUserSelectionCurrent(targetSceneID, scriptRow.selection_group, scriptRow.selection_no);

            // 다른 친구들은 Out 으로 처리 
            SetOtherStackState("Out");

            // 선택받은 상태로 !
            SetAnimationState("Select");

            // 파티클
            particleSelect.gameObject.SetActive(true);
            particleSelect.Play();

        }


        /// <summary>
        /// 종료!
        /// </summary>
        public void HideSelection()
        {
            this.gameObject.SetActive(false);
            buttonImage.transform.localPosition = Vector3.zero;

            ListStacks.Remove(this);
        }


        /// <summary>
        /// 본인 제외 다른 선택지들에 대한 상태 제어 
        /// </summary>
        void SetOtherStackState(string __state)
        {
            for (int i = 0; i < ListStacks.Count; i++)
            {
                if (ListStacks[i] == this)
                    continue;

                ListStacks[i].SetAnimationState(__state);
            }
        }


        #region 개체 포인터 다운 & 업 처리 
        /* 빠른속도로 터치를 했을 때 문제가 있다.... */

        public void OnPointerDown(PointerEventData eventData)
        {
            // 잠금상태의 선택지는 고를 수 없음
            if (isLock)
                return;

            if (CheckReleasing())
                return;

            // 멀티 터치 방지
            if (isOneOfSelectionPointerDown)
                return;

            // 하나라도 선택이 완료된 경우 안되도록 
            if (CheckSelectionSelected())
                return;

            isOneOfSelectionPointerDown = true;


            Debug.Log(string.Format("OnPointerDown [{0}]", selectionScript));


            // 현재 누르고 있는 개체 상태 
            SetAnimationState("Fill");

            SetOtherStackState("Unselect");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isOneOfSelectionPointerDown)
                return;

            // 하나라도 선택이 완료된 경우 안되도록 
            if (CheckSelectionSelected())
                return;

            isOneOfSelectionPointerDown = false;
            isReleasing = true;

            Debug.Log(string.Format("OnPointerUp [{0}]", selectionScript));

            SetAnimationState("Idle");
            SetOtherStackState("Idle");

            StartCoroutine(RoutineRelease());
        }

        IEnumerator RoutineRelease()
        {
            yield return new WaitForSeconds(0.5f);
            isReleasing = false;
        }

        /// <summary>
        /// 하나라도 선택이 되어있는지 체크 
        /// </summary>
        /// <returns></returns>
        public static bool CheckSelectionSelected()
        {
            for (int i = 0; i < ListStacks.Count; i++)
            {
                if (ListStacks[i].isButtonSelected)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 반복 터치시 발생하는 오류를 막기 위해 사용한다.
        /// </summary>
        /// <returns></returns>
        public static bool CheckReleasing()
        {
            for (int i = 0; i < ListStacks.Count; i++)
            {
                if (ListStacks[i].isReleasing)
                    return true;
            }

            return false;
        }

        #endregion
    }
}