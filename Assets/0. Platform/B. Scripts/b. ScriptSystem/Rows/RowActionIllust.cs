using UnityEngine;
using DG.Tweening;
using System;

using LitJson;

namespace PIERStory
{
    public class RowActionIllust : IRowAction
    {
        ScriptRow scriptRow;
        Action callback = delegate { };
        GameSpriteCtrl gameSprite;
        ScriptLiveMount liveImage;

        public RowActionIllust(ScriptRow __row)
        {
            scriptRow = __row;
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            JsonData data = UserManager.main.GetIllustData(scriptRow.script_data);

            // 일러스트 해금이라면 팝업 출현
            /* 21.11.22 아직 미구현이기 때문에 주석
            if (data != null && NetworkLoader.main.UpdateUserIllust(data[0].ToString(), data[1].ToString()))
            {
                GameManager.main.ShowAchieveIllust(data[2].ToString());
                //AppsFlyerSDK.AppsFlyer.sendEvent("ILLUST_ACQUIRE_" + data[0].ToString(), null);
            }
            */

            // 스킵했으면 렌더 안하고 넘어가
            if (__isInstant && GameManager.main.RenderingPass())
            {
                __actionCallback();
                return;
            }

            callback = __actionCallback;

            // 대상 일러스트가 라이브 일러스트인지 먼저 체크를 하자. 
            liveImage = GameManager.main.SetGameLiveIllust(scriptRow.script_data);

            // 화면 이미지들 제거해주고. 
            GameManager.main.HideImageResources();

            // 캐릭터도 제거해주고
            GameManager.main.HideCharacters();
            ViewGame.main.FadeOutTimeFlow();

            if (liveImage != null)
            {
                // 라이브 일러스트 보여주세요!
                liveImage.PlayCubismAnimation();
                callback();
            }
            else
            {
                // 라이브 일러스트가 아니었다면 그냥 일반 일러스트 로직으로 돌아온다. 
                // 게임매니저에게 이미지 요청 
                gameSprite = GameManager.main.SetGameIllust(scriptRow.script_data);

                if (gameSprite == null)
                {
                    // null 이면 default로 게임매니저에서 자동 세팅된다. 
                    // 바로 완료 콜백 호출
                    __actionCallback();
                    return;
                }

                gameSprite.spriteRenderer.color = Color.black;
                gameSprite.gameObject.SetActive(true);
                gameSprite.spriteRenderer.DOColor(Color.white, 2f).OnComplete(OnTween);
            }
        }

        public void EndAction() { }

        void OnTween()
        {
            if (scriptRow.autoplay_row < 1)
                callback();
        }
    }
}