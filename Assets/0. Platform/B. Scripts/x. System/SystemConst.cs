using UnityEngine;

namespace PIERStory {
    public static class SystemConst {
        
        public const long timerTick = 621355968000000000; // C#과 javascript 타임 Tick 차이  UTC 기준
        
        public const string KEY_LOCAL_VER = "localVer"; // 로컬라이징 텍스트 버전 KEY (PlayerPrefs)
        public const string KEY_LOCAL_DATA ="localData"; // 로컬라이징 텍스트 데이터 
        public const string KEY_LANG ="currentLang"; // 언어코드 
        
        public const string KEY_NETWORK_DOWNLOAD = "accessData";
        public const string KEY_PLATFORM_LOADING ="platformLoading"; // PlayerPrefs.. 
        
        
        public const string IMAGE_KEY = "image_key";
        public const string IMAGE_URL = "image_url";
        
        
        
        
        public static long ConvertServerTimeTick(long __serverTick) {
            return (__serverTick * 10000) + timerTick;
        } 
        
        /// <summary>
        /// 할인 가격 구하기 
        /// </summary>
        /// <param name="__originPrice"></param>
        /// <param name="__discount"></param>
        /// <returns></returns>
        public static int GetSalePrice(int __originPrice, float __discount) {
            
            float salePrice = (float)__originPrice - (float)__originPrice * __discount;
                
            return Mathf.RoundToInt(salePrice);
        }
    }
}