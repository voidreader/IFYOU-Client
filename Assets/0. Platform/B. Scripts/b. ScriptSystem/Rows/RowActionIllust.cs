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
        
        string template = string.Empty;

        public RowActionIllust(ScriptRow __row, string __type)
        {
            scriptRow = __row;
            template = __type; // 라이브 일러스트와 일러스트가 구분되어 들어온다.
        }

        public void DoAction(Action __actionCallback, bool __isInstant = false)
        {
            JsonData data = null;
            string illustType = string.Empty;
            string id = string.Empty;
            
            // 라이브 일러스트와 일러스트로 구분해야된다. (스크립트 불러올때 유료, 무료에 따라 템플릿을 변경시켜준다.)
            // * 기존 로직 때문에 일러스트의 경우는 이미지를 검색 후 없으면 Live2D까지 체크한다.
            if(template == "illust") {
                data = StoryManager.main.GetImageIllustData(scriptRow.script_data);
                illustType = template;
                
                if(data == null) { // 일러스트 없으면 라이브 일러스트 데이터 찾아본다.
                
                    data = StoryManager.main.GetLiveIllustJsonByName(scriptRow.script_data);
                    
                    if(data == null) {
                        SystemManager.ShowMessageAlert(string.Format("일러스트 {0}가 없습니다", scriptRow.script_data), false);
                        __actionCallback();
                        return;
                    }
                    
                    illustType = "live_illust"; // 라이브 일러스트로 변경한다. 
                    id = data[0]["live_illust_id"].ToString();
                }
                else {
                    // 일러스트 있음
                    id = data["illust_id"].ToString();
                }
                
                
                
            }
            else {
                // 라이브 일러스트 
                data = StoryManager.main.GetLiveIllustJsonByName(scriptRow.script_data);
                illustType = template;
                id = data[0]["live_illust_id"].ToString();
            }
            

            // 일러스트 해금이라면 팝업 출현
            // id와 타입을 전달한다. 
            if (data !=null && NetworkLoader.main.UpdateUserIllust(id, illustType))
            {
                // public_name을 전달해줘야한다. 
                GameManager.main.ShowAchieveIllust(UserManager.main.GetGalleryImagePublicName(id, illustType));
            }

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
                Debug.Log("No Live2D Data in RowActionIllust");
                
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