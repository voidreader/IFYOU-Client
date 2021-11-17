﻿using System;
using UnityEngine;

using LitJson;
using BestHTTP;

namespace PIERStory
{
    [Serializable]
    public class ScriptBubbleMount
    {
        Action OnMountCompleted = delegate { };

        static readonly string COL_SLICE = "is_slice";
        static readonly string COL_BORDER_LEFT = "border_left";
        static readonly string COL_BORDER_RIGHT = "border_right";
        static readonly string COL_BORDER_TOP = "border_top";
        static readonly string COL_BORDER_BOTTOM = "border_bottom";

        public Texture2D texture = null;
        public Sprite sprite = null;

        public string imageUrl = string.Empty;
        public string imageKey = string.Empty;

        public bool isMounted = false;          // 성공적으로 불러왔는지

        JsonData spriteOriginJson = null;

        public string spriteId = string.Empty;
        bool is_slice = false;
        float border_left = 0;
        float border_right = 0;
        float border_top = 0;
        float border_bottom = 0;

        public ScriptBubbleMount(string __id, string __url, string __key, Action __cb)
        {
            spriteId = __id;
            imageUrl = __url;
            imageKey = __key;
            OnMountCompleted = __cb;

            // 스프라이트 기준정보를 가져온다.
            spriteOriginJson = GetSpriteOrigin(spriteId);

            // 기준정보가 없으면 slice 처리 하지 않는다. 
            if (spriteOriginJson == null)
                is_slice = false;
            else
            {
                if (SystemManager.GetJsonNodeString(spriteOriginJson, COL_SLICE).Equals("1"))
                    is_slice = true;
            }


            // border 값 가져오기 
            if (is_slice)
            {
                float.TryParse(SystemManager.GetJsonNodeString(spriteOriginJson, COL_BORDER_LEFT), out border_left);
                float.TryParse(SystemManager.GetJsonNodeString(spriteOriginJson, COL_BORDER_RIGHT), out border_right);
                float.TryParse(SystemManager.GetJsonNodeString(spriteOriginJson, COL_BORDER_TOP), out border_top);
                float.TryParse(SystemManager.GetJsonNodeString(spriteOriginJson, COL_BORDER_BOTTOM), out border_bottom);
            }

            // 이미지 불러온다
            LoadImage();
        }

        /// <summary>
        /// 이미지 로드
        /// </summary>
        public void LoadImage()
        {
            // 받아놓은 파일이 있을때. 
            if (ES3.FileExists(imageKey))
            {
                texture = ES3.LoadImage(imageKey);
                CreateSprite();
                SendSuccessMessage();
                return;
            }

            var req = new HTTPRequest(new Uri(imageUrl), OnImageDownloaded);
            req.Send();
        }

        void OnImageDownloaded(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckInGameDownloadValidation(req, res))
                return;

            // 다운로드 실패했을때는..? 
            if (!res.IsSuccess)
            {
                Debug.LogError("Download Failed : " + req.Uri.ToString());
                SendFailMessage();
                return;
            }

            texture = new Texture2D(0, 0);
            texture.LoadImage(res.Data);

            CreateSprite();

            ES3.SaveImage(texture, imageKey); // 로컬에 세이브 합니다.
            SendSuccessMessage(); // 완료
        }

        void CreateSprite()
        {
            if (is_slice)
            {
                // slice 방식 
                sprite = Sprite.Create(texture
                    , new Rect(0, 0, texture.width, texture.height)
                    , new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.Tight
                    , new Vector4(border_left, border_bottom, border_right, border_top));
            }
            else
            {
                // 일반 방식
                sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }

            sprite.texture.wrapMode = TextureWrapMode.Clamp;
            sprite.texture.filterMode = FilterMode.Trilinear;
            sprite.texture.Apply();
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

        static JsonData GetSpriteOrigin(string __id)
        {
            JsonData spriteJSON = UserManager.main.GetNodeBubbleSprite();
            for (int i = 0; i < spriteJSON.Count; i++)
            {
                if (SystemManager.GetJsonNodeString(spriteJSON[i], GameConst.COL_BUBBLE_SPRITE_ID).Equals(__id))
                    return spriteJSON[i];
            }

            return null;
        }
    }
}


