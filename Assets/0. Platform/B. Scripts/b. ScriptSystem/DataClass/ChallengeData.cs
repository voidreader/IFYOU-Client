using System;
using LitJson;


namespace PIERStory {
    
    
    [Serializable]
    public class ChallengeData
    {
        public JsonData jsonData;
        
        public int chapterNumber = 0;
        public int detailNo = 9;
        public string freeCurrency = string.Empty;
        public int freeQuantity = 0;
        public bool isFreeReceived = false;
        
        public string premiumCurrency = string.Empty;
        public int premiumQuantity = 0;
        
        public bool isPremiumReceived = false;
        
            // "detail_no": 453,
            // "free_currency": "gem",
            // "free_quantity": 5,
            // "free_reward_date": "",
            // "premium_currency": "gem",
            // "premium_quantity": 25,
            // "premium_reward_date": ""        
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="__data"></param>
        /// <param name="__chapterNumber"></param>
        public ChallengeData(JsonData __data, string __chapterNumber) {
            jsonData = __data;
            
            int.TryParse(__chapterNumber, out chapterNumber);
            
            freeQuantity = SystemManager.GetJsonNodeInt(jsonData, "free_quantity");
            premiumQuantity = SystemManager.GetJsonNodeInt(jsonData, "premium_quantity");
            
            freeCurrency = SystemManager.GetJsonNodeString(jsonData, "free_currency");
            premiumCurrency = SystemManager.GetJsonNodeString(jsonData, "premium_currency");
            
            // 미션 수신 여부 
            isFreeReceived = !string.IsNullOrEmpty(SystemManager.GetJsonNodeString(jsonData, "free_reward_date"));
            isPremiumReceived = !string.IsNullOrEmpty(SystemManager.GetJsonNodeString(jsonData, "premium_reward_date"));
        }
    }
}