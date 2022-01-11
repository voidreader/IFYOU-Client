using System;
using UnityEngine;

using LitJson;
using BestHTTP;

namespace PIERStory
{
    [Serializable]

    public class ScriptImageMount
    {
        Action OnMountCompleted = delegate { };

        static readonly string COL_RESIZE = "is_resized"; // 이미지 크기 조정되었는지 체크 컬럼
        static readonly string COL_EMOTICON_OWNER = "emoticon_owner";

        public Texture2D texture = null;
        public Sprite sprite = null;

        public string template = string.Empty;
        public string imageName = string.Empty; // 이미지 이름 
        public string speaker = string.Empty;    // 화자 

        public string imageUrl = string.Empty;
        public string imageKey = string.Empty;

        JsonData resourceData = null;

        public bool isMounted = false;  // 성공적으로 불러왔는지!
        public int useCount = 0;        // 하나의 에피소드에서 몇번을 사용하는지 체크용도. (image만 일단 먼저 적용 2021.06.04);

        // 크기와 위치정보 
        public float gameScale = 10;
        public float offsetX = 0;
        public float offsetY = 0;

        public bool isResized = false; // 이미지 크기 조정 되었음! 기본값 false. 미니컷에서 사용

        public ScriptImageMount(string __type, JsonData __j, Action __cb)
        {
            OnMountCompleted = __cb;
            resourceData = __j;

            template = __type;

            imageName = SystemManager.GetJsonNodeString(resourceData, CommonConst.COL_IMAGE_NAME);
            imageUrl = SystemManager.GetJsonNodeString(resourceData, CommonConst.COL_IMAGE_URL);
            imageKey = SystemManager.GetJsonNodeString(resourceData, CommonConst.COL_IMAGE_KEY);

            // 이모티콘의 경우 speaker 정보 필요함
            if (template != GameConst.TEMPLATE_BACKGROUND
                && template != GameConst.TEMPLATE_ILLUST
                && template != GameConst.TEMPLATE_IMAGE
                && resourceData.ContainsKey(COL_EMOTICON_OWNER))
            {
                speaker = SystemManager.GetJsonNodeString(resourceData, COL_EMOTICON_OWNER);
            }

            // 추가 정보
            if(resourceData.ContainsKey(CommonConst.COL_GAME_SCALE))
                gameScale = float.Parse(SystemManager.GetJsonNodeString(resourceData, CommonConst.COL_GAME_SCALE));
            if (resourceData.ContainsKey(CommonConst.COL_OFFSET_X))
                offsetX = float.Parse(SystemManager.GetJsonNodeString(resourceData, CommonConst.COL_OFFSET_X));
            if (resourceData.ContainsKey(CommonConst.COL_OFFSET_Y))
                offsetY = float.Parse(SystemManager.GetJsonNodeString(resourceData, CommonConst.COL_OFFSET_Y));


            switch (template)
            {
                case GameConst.TEMPLATE_IMAGE:

                    // isResized 컬럼을 통해 미니컷 이미지의 크기를 0.65 스케일로 줄일지, 그냥 원본 크기를 사용할지를 처리합니다. 
                    if (SystemManager.GetJsonNodeString(resourceData, COL_RESIZE).Equals("1"))
                        isResized = true;

                    DownloadImage(); // ! 미니컷은 다운로드만 처리해보자 2021.07.23
                    return;

                case GameConst.TEMPLATE_BACKGROUND:
                case GameConst.TEMPLATE_MOVEIN:
                    DownloadImage(); // ! 배경도 다운로드만 처리해놓는다. 2021.07.23
                    return;
            }

            // 실제 이미지 불러오기 처리 (배경, 이모티콘)
            LoadImage();
        }

        void DownloadImage()
        {
            // 로컬 체크 
            if (ES3.FileExists(imageKey))
            {
                SendSuccessMessage();
                return;
            }

            // ! 다운로드 처리. 다운로드 완료 후에도 texture와 sprite를 생성해두지 않는다. 
            var req = new HTTPRequest(new Uri(imageUrl), OnDownloadOnly);
            req.Send();
        }

        /// <summary>
        /// 다운로드만 해두는 처리!
        /// </summary>
        void OnDownloadOnly(HTTPRequest req, HTTPResponse res)
        {

            // 인게임 리소스 다운로드는 CheckInGameDownloadValidation 사용 
            // CheckInGameDownloadValidation에서 3번 시도 후 실패하면 팝업 띄운다. 
            if (!NetworkLoader.CheckInGameDownloadValidation(req, res))
                return;

            // Allow Missing Resource에 의해서 true로 넘어오는 경우도 있다.
            if (!res.IsSuccess)
            {
                Debug.LogError("Download Failed : " + req.Uri.ToString());
                SendFailMessage();
                return;
            }

            // ! 로컬에 저장만 해둔다. 
            ES3.SaveRaw(res.Data, imageKey);
            SendSuccessMessage(); // 완료
        }

        /// <summary>
        /// 서버 기준정보 기반으로 이미지 정보 불러오기 
        /// 이미지 불러서 Texture 및 Sprite까지 생성 
        /// </summary>
        public void LoadImage()
        {
            // 파일 로컬에 존재하는 경우에 로컬에서 읽어들이기.
            if (ES3.FileExists(imageKey))
            {
                // Debug.Log(string.Format("{0} file exists", image_key));
                texture = ES3.LoadImage(imageKey);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                SendSuccessMessage();
                return;
            }

            // 파일이 없으면 network로 불러온다.
            var req = new HTTPRequest(new Uri(imageUrl), OnImageDownloaded);
            req.Send();
        }

        void OnImageDownloaded(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
                return;

            if (!res.IsSuccess)
            {
                Debug.LogError("Download Failed : " + req.Uri.ToString());
                SendFailMessage();
                return;
            }
            
            texture = new Texture2D(0, 0);
            texture.LoadImage(res.Data);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            ES3.SaveImage(texture, imageKey); // 로컬에 세이브 합니다.
            SendSuccessMessage(); // 완료
        }

        void SendFailMessage()
        {
            isMounted = false;
            OnMountCompleted();
        }

        void SendSuccessMessage()
        {
            isMounted = true;
            OnMountCompleted();
        }

        /// <summary>
        /// 미리 생성하지 않고, 필요할때 Sprite 및 Texture 생성 
        /// </summary>
        public bool CreateRealtimeSprite()
        {
            try
            {
                // 둘다 생성되어있으면 Create 할 필요 없음 
                if (texture != null && sprite != null)
                    return true;

                if (texture == null)
                    texture = ES3.LoadImage(imageKey);

                if (sprite == null)
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            }
            catch (Exception e)
            {
                Debug.Log(e.StackTrace);
                return false;
            }

            return true;
        }



        /// <summary>
        /// 이미지의 에피소드 내에서 사용횟수 정하기 
        /// </summary>
        /// <param name="__count"></param>
        public void SetImageUseCount(int __count)
        {
            useCount = __count;
        }

        /// <summary>
        /// 이미지 사용하면서 사용횟수 차감하기.
        /// </summary>
        /// <param name="__count"></param>
        public void DecreaseUseCount()
        {
            useCount -= 1;
        }

        /// <summary>
        /// 이미지 사용 종료 신고!
        /// </summary>
        public void EndImage()
        {

            // 이미지, 배경만 해당된다.
            // 21.09.15 이모티콘 할당도 풀어주기로 했다. 그래서 대화 관련된거 나왔을 때 다 풀어줘야 함
            
            // 다 썼다고 판단되면 놔주자.. 
            if (useCount <= 0)
            {
                Debug.Log(string.Format("><>< Bye [{0}]", imageName));

                // 게임매니저 Dictionary에서 제거 요청
                switch (template)
                {
                    // IMAGE 사용하지 않도록 변경 2022.01.11
                    case GameConst.TEMPLATE_IMAGE:
                        return;
                    case GameConst.TEMPLATE_BACKGROUND:
                    case GameConst.TEMPLATE_MOVEIN:
                        //GameManager.main.RemoveBackgroundFromDicionary(imageName);
                        break;
                    case GameConst.TEMPLATE_TALK:
                    case GameConst.TEMPLATE_SPEECH:
                    case GameConst.TEMPLATE_WHISPER:
                    case GameConst.TEMPLATE_YELL:
                    case GameConst.TEMPLATE_MONOLOGUE:
                    case GameConst.TEMPLATE_FEELING:
                        //GameManager.main.RemoveEmoticonFromDictionary(imageName);
                        break;

                    default:
                        return;
                }

                // 이 오브젝트 파괴
                Sprite.Destroy(sprite);
                Texture2D.Destroy(texture);

                sprite = null;
                texture = null;
            }
        }
    }
}
