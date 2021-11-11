using UnityEngine;
using BestHTTP;

namespace PIERStory {
    [System.Serializable]
    public class EpisodeThumbnail  {
        public string image_url = string.Empty;
        public string image_key = string.Empty;
        public Sprite sprite = null;
        public Texture2D texture = new Texture2D(0, 0);

        // 단순하게 불러오기 액션을 취했는지. 
        public bool isLoaded = false;

        // 불러오기 후에 실제로 이미지를 성공적으로 불어왔는지. 
        public bool isSuccess = false; 

        /// <summary>
        /// 썸네일 정보 생성하자. 
        /// </summary>
        /// <param name="__url"></param>
        /// <param name="__key"></param>
        public EpisodeThumbnail(string __url, string __key)
        {

            isLoaded = false;
            isSuccess = false;

            if (string.IsNullOrEmpty(__url) || string.IsNullOrEmpty(__key))
                return;

            image_url = __url;
            image_key = __key;

            if(ES3.FileExists(image_key))
            {
                texture = ES3.LoadImage(image_key);
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                isLoaded = true;
                isSuccess = true;
            }
            else
            {
                // 다운로드
                new HTTPRequest(new System.Uri(image_url), OnDownload).Send();
            }
        }

        void OnDownload(HTTPRequest req, HTTPResponse res)
        {
            if(req.State != HTTPRequestStates.Finished)
            {
                isLoaded = true;
                isSuccess = false;
                Debug.LogError("Download Failed : " + image_url);
                return;
            }

            texture.LoadImage(res.Data);
            ES3.SaveImage(texture, image_key);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            isLoaded = true;
            isSuccess = true;
        }
    }
}