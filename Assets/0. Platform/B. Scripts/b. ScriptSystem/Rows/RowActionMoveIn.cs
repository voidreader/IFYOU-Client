using UnityEngine;
using DG.Tweening;
using System;

namespace PIERStory
{

    public class RowActionMoveIn : IRowAction
    {
        ScriptRow scriptRow;
        string scriptData = string.Empty;
        Action callback = delegate { };

        bool isValidParameter = false;
        string place = string.Empty;        // 진입할 배경이름 
        string label = string.Empty;        // 진입시 표기할 문구
        string[] paramArray;                // 파라매터

        // 배경 트윈용 변수 
        string directionFrom = string.Empty; // 진입방향 
        float arrivalPoint = 0f;             // 도착 좌표 

        float moveDistance = 0;
        float originPosX = 0;               // 최초 X좌표 
        float destPosX = 0;                 // 도착 X좌표 
        float tweenTime = 0;
        GameSpriteCtrl currentBG = null;    // 현재 배경개체

        /// <summary>
        /// 생성자 입니다
        /// </summary>
        public RowActionMoveIn(ScriptRow __row)
        {
            scriptRow = __row;
            scriptData = __row.script_data;

            // 파라매터 처리
            // 데이터 입력안된 경우. 
            if (string.IsNullOrEmpty(scriptData))
            {
                Debug.Log("<color=yellow>RowActionMoveIn scriptData Empty </color>");
                isValidParameter = false; // 유효하지 않은 입력이에요!
                scriptData = string.Empty;
                return;
            }

            if (!scriptData.Contains(GameConst.SPLIT_SCREEN_EFFECT)) // 장소 이외에 추가로 파라매터가 없는 경우 기본값 처리 
            {
                place = scriptData;
            }
            else // 장소 이외에 추가로 파라매터가 있는 경우 
            {
                place = scriptData.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[0]; // 배경 지정. 

                string options = scriptData.Split(GameConst.SPLIT_SCREEN_EFFECT[0])[1]; // 파라매터 스트링 추출 

                // 띄어쓰기 때문에 라벨은 이렇게 얻어낸다. 
                if (options.Contains("라벨"))
                {
                    label = options.Substring(options.LastIndexOf('=') + 1);
                    label = label.Replace("\\", "\n");
                }


                // 공백 입력되는 경우가 있어서 공백 제거하고 다시 paramArray 사용한다
                options = options.Replace(" ", "");
                paramArray = options.Split(GameConst.SPLIT_SCREEN_EFFECT_V[0]);

                // 진입과 도착좌표 구하기
                ScriptRow.GetParam<string>(paramArray, "진입", ref directionFrom);
                ScriptRow.GetParam<float>(paramArray, "도착좌표", ref arrivalPoint);

                // 기본값 처리, float의 default는 0. 
                if (string.IsNullOrEmpty(directionFrom) || (!directionFrom.Equals(GameConst.POS_LEFT) && !directionFrom.Equals(GameConst.POS_RIGHT)))
                    directionFrom = GameConst.POS_LEFT;

                //혹시 모르니까 범위 제한 처리 
                arrivalPoint = Mathf.Clamp(arrivalPoint, -1f, 1f);
            }

            // 여기까지 왔으면 통과!
            isValidParameter = true;

        } // end 생성자 

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            callback = __actionCallback;
            GameManager.main.isWaitingScreenTouch = false; // 따로 터치 기다릴 필요 없음 

            // 배경이 있는지 체크해서 없으면 false로 처리해줍니다. 
            if (!GameManager.main.DictBackgroundMounts.ContainsKey(place))
            {
                Debug.Log(string.Format("<color=yellow>RowActionMoveIn place is not valid [{0}] </color>", place));
                isValidParameter = false;
            }

            // 2021.10.12 효과음들이 길어서 배경이 전환 될 때, 효과음을 멈춘다
            if (GameManager.main.SoundGroup[2].GetIsPlaying)
                GameManager.main.SoundGroup[2].StopAudioClip();


            // 이탈과는 다르게 script_data가 올바르지 않다면 진행하지 못함. 
            if (!isValidParameter)
            {
                // 디폴트 배경이라도 설정하게 해주고 끝낸다.
                GameManager.main.SetGameBackground("DEFAULT"); // 아무이름이나 쓰면 알아서 default로 처리해준다;;; 

                GameManager.ShowMissingComponent("장소진입", string.Format("잘못된 입력 : {0}", scriptData));
                callback?.Invoke();
                return;
            }

            // 유효성 검사를 끝내고 여기서부터 실제 로직을 진행
            currentBG = GameManager.main.SetGameBackground(place); // 배경 할당.

            // 거리에 따라 트윈 시간 설정 
            tweenTime = GameManager.main.CalcMoveBGAnimTime(ref moveDistance);

            // Action에서 사용할 여러 변수들을 처리합니다. 
            // 시작 위치 및 도착 위치 설정 
            if (directionFrom.Equals(GameConst.POS_LEFT)) // 왼쪽에서 진입
                originPosX = moveDistance;
            else
                originPosX = moveDistance * -1;

            // 도착위치 
            destPosX = -originPosX * (1 + arrivalPoint);

            if (!string.IsNullOrEmpty(scriptRow.controlAlternativeName))
                currentBG.transform.localScale = new Vector3(-currentBG.gameScale, currentBG.gameScale, 1f);

            #region 즉시 실행 처리하기 
            if (__isInstant)
            {
                currentBG.transform.position = new Vector3(originPosX + destPosX, 0, 0); // 바로 도착좌표로 이동시키고 끝!
                currentBG.gameObject.SetActive(true);
                currentBG.spriteRenderer.color = Color.white;

                callback?.Invoke();
                return;
            }

            #endregion


            Sequence moveIn = DOTween.Sequence();

            currentBG.gameObject.SetActive(true);
            // 배경의 시작 position 설정 
            currentBG.transform.position = new Vector3(originPosX, 0, 0);
            currentBG.spriteRenderer.color = Color.black; // 암전상태에서 시작한다. 

            // 페이드인 처리 
            moveIn.Append(currentBG.spriteRenderer.DOColor(Color.white, 1.2f));

            ViewGame.main.FadeOutTimeFlow();

            // 페이드인 후 앵글 이동. 0이면 이동할 값이 없기 때문에 추가해주지 않는다.
            if (!destPosX.Equals(0f))
                moveIn.Append(currentBG.transform.DOMoveX(destPosX, tweenTime).SetRelative());

            // label이 empty값이 아니면
            if (!string.IsNullOrEmpty(label))
                ViewGame.main.PlaceNameAnim(moveIn, label);

            moveIn.OnComplete(MoveComplete);
        }

        void MoveComplete()
        {
            ViewGame.main.placeTextBG.gameObject.SetActive(false);
            callback?.Invoke();
        }

        public void EndAction() { }
    }
}