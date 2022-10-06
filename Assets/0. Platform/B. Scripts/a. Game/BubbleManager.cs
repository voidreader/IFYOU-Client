using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace PIERStory
{
    public class BubbleManager : MonoBehaviour
    {
        // 말풍선 타입 2종 
        const string HALF_LINE = "half";
        const string BODY_LINE = "body";
        
        public static BubbleManager main = null;

        public int normalFontSize = 28; // 대화 폰트 사이즈 
        public int BigFontSize = 36;    // 큰 폰트 사이즈

        public int defaultTalkPos = 1;      // default 대화, 속삭임 포지션 (1부터 9)
        public int defaultFeelingPos = 4;   // default 속마음, 외침 포지션 


        [Space][Header("말풍선 크기 판단용 가짜 말풍선들")]
        public List<GameBubbleCtrl> FakeBubbles;

        // 서버에서 말풍선 세트로 받아온 이미지들.
        public Dictionary<string, Sprite> DictBubbleSprites = new Dictionary<string, Sprite>();
        public List<Sprite> partnerBubbleSprites = new List<Sprite>();
        
        
        
        JsonData bubbleMaster = null;
        public string bubbleType = BODY_LINE; // 기본은 body 타입 
        public bool isTagColorAffect = false; // 네임태그 색상이 말풍선에 영향을 미침 
        public string tagAlignType = "center";
        public int tagTextareaLeft = 0;
        public int tagTextareaRight = 20;
        public int tagTextareaTop = -2;
        public int tagTextareaBottom = -2;
        
        public float bubbleInitFactor = 0;

        private void Awake()
        {
            main = this;
        }
        
        void Start() {
            
            // 말풍선 마스터 처리 추가 2022.07
            bubbleMaster = StoryManager.main.currentBubbleMasterJson;
            
            normalFontSize = SystemManager.GetJsonNodeInt(bubbleMaster, "normal_font_size");
            BigFontSize = SystemManager.GetJsonNodeInt(bubbleMaster, "big_font_size");
            
            // 아랍어 폰트 사이즈 조정.. 2022.06.22
            if(SystemManager.main.currentAppLanguageCode == CommonConst.COL_AR) {
                normalFontSize -= 4;
                BigFontSize -= 4;
            }
            
            // 말풍선 마스터 정보 모으기 
            isTagColorAffect = SystemManager.GetJsonNodeBool(bubbleMaster, "tag_color_affect");
            tagAlignType = SystemManager.GetJsonNodeString(bubbleMaster, "tag_align_type");
            tagTextareaLeft = SystemManager.GetJsonNodeInt(bubbleMaster, "tag_textarea_left");
            tagTextareaRight = SystemManager.GetJsonNodeInt(bubbleMaster, "tag_textarea_right");
            tagTextareaTop = SystemManager.GetJsonNodeInt(bubbleMaster, "tag_textarea_top");
            tagTextareaBottom = SystemManager.GetJsonNodeInt(bubbleMaster, "tag_textarea_bottom");
            
            bubbleType = SystemManager.GetJsonNodeString(bubbleMaster, "bubble_type");
            
            // 타입에 따라서 말풍선 초기 크기 지정 
            if(bubbleType == "half")
                bubbleInitFactor = 0.85f;
            else 
                bubbleInitFactor = 0f;
        }
        
        /// <summary>
        /// 말풍선 타입이 하프인지 체크 
        /// </summary>
        /// <returns></returns>
        public bool IsHalfBubbleSprite() {
            return bubbleType == HALF_LINE;
        }


        /// <summary>
        /// 말풍선 스프라이트 주세요!
        /// </summary>
        /// <param name="__id"></param>
        /// <returns></returns>
        public Sprite GetBubbleSprite(string __id)
        {
            if (!DictBubbleSprites.ContainsKey(__id))
                return null;

            return DictBubbleSprites[__id];

        }

        public Sprite GetForPhoneSprite(int size)
        {
            if (size < 1)
                return partnerBubbleSprites[0];

            return partnerBubbleSprites[size - 1];
        }

        /// <summary>
        /// 말풍선 스프라이트 딕셔너리 구성 
        /// </summary>
        /// <param name="__id"></param>
        /// <param name="__sp"></param>
        public void AddBubbleSprite(string __id, Sprite __sp)
        {
            if (DictBubbleSprites.ContainsKey(__id))
                return;

            DictBubbleSprites.Add(__id, __sp);

        }

        public void ClearBubbleSpriteDictionary()
        {
            DictBubbleSprites.Clear();
        }
        
        
        
        /// <summary>
        /// 언어별 폰트 변경때문에 로비에서는 시작시점에 감춰준다. 
        /// </summary>
        public void ShowFakeBubbles(bool __flag) {
            for(int i=0; i<FakeBubbles.Count;i++) {
                FakeBubbles[i].gameObject.SetActive(__flag);
            }
        }

        public void SetFakeBubbles(ScriptRow __row)
        {
            // Debug.Log("SetFakeBubbles");
            
            // 크기 1부터 4까지 할당한다.
            for (int i = 0; i < FakeBubbles.Count; i++)
            {
                FakeBubbles[i].SetFakeBubble(__row, i + 1);
            }
        }
        
        /// <summary>
        /// 꾸미기 말풍선 사이즈 체크를 위한 가짜 말풍선 설정하기 
        /// </summary>
        /// <param name="__text"></param>
        public void SetLobbyFakeBubbles(string __text) {
            
            Debug.Log(string.Format("SetLobbyFakeBubbles [{0}]", __text));
            
            for(int i=0; i<FakeBubbles.Count;i++) {
                FakeBubbles[i].SetLobbyFakeBubble(__text, i+1);
            }
        }
        

        /// <summary>
        /// 현재 가짜 말풍선 중에서 적절한 크기 받아오기 
        /// </summary>
        /// <returns></returns>
        public int GetCurrentAdjustmentSize()
        {
            for (int i = 0; i < FakeBubbles.Count; i++)
            {
                if (!FakeBubbles[i].textContents.isTextTruncated)
                    return i + 1;
            }

            return 4;
        }
        
        /// <summary>
        /// 네임태그의 정렬 방법 가져오기 
        /// </summary>
        public TMPro.HorizontalAlignmentOptions GetTagAlign() {
            
            
            if(string.IsNullOrEmpty(tagAlignType)) 
                return TMPro.HorizontalAlignmentOptions.Center;
            
            if(tagAlignType == "center")
                return TMPro.HorizontalAlignmentOptions.Center;
            else if(tagAlignType == "left")
                return TMPro.HorizontalAlignmentOptions.Left;
            else if(tagAlignType == "right")
                return TMPro.HorizontalAlignmentOptions.Right;
            
                
            
                
            return TMPro.HorizontalAlignmentOptions.Center;
        }


        /// <summary>
        /// 주어진 포지션에서 +-20 을 해서 돌려준다. 
        /// </summary>
        /// <param name="__pos"></param>
        /// <returns></returns>
        public static Vector2 GetRandomPos(Vector2 __pos)
        {
            return new Vector2(__pos.x + Random.Range(-10, 11), __pos.y + Random.Range(-10, 11));
        }


        /// <summary>
        /// 인덱스!
        /// </summary>
        /// <returns></returns>
        public static int GetDefaultBubbleTalkPosIndex()
        {
            if (main.defaultTalkPos > 6)
                main.defaultTalkPos = 4;

            return main.defaultTalkPos;
        }

        public static int GetDefaultBubbleCenterPosIndex()
        {
            if (main.defaultFeelingPos > 6)
                main.defaultFeelingPos = 4;

            return main.defaultFeelingPos;
        }

        public static void IncreaseBubbleTalkPosIndex()
        {
            main.defaultTalkPos++;
        }

        public static void IncreaseBubbleCenterPosIndex()
        {
            main.defaultFeelingPos++;
        }
        
    }
}


