using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

namespace PIERStory {
    
    
    [Serializable]
    public class PassTimeDealData
    {
        JsonData originJSON = null; 
        public string debugJSON = string.Empty;
        
        public string projectID = string.Empty;
        public int timedealID = 0;
        public int discountINT = 0;
        public long expireTick = 0; // 마감 시간 tick 
        
        public PassTimeDealData(JsonData __j) {
            originJSON = __j;
            debugJSON = JsonMapper.ToStringUnicode(originJSON);
            
            InitData();    
            
        }
        
        public PassTimeDealData() {
            originJSON = null;
            timedealID = 0;
        }
        
        /// <summary>
        /// 직렬화, 커스텀 클래스는 null을 지원하지 않기 때문에 만들었음. 
        /// </summary>
        /// <value></value>        
        public bool isValidData {
            get {
                return originJSON != null && timedealID > 0;
            }
        }
        
        
        
        void InitData() {
            projectID = SystemManager.GetJsonNodeString(originJSON, "project_id");
            
            timedealID = SystemManager.GetJsonNodeInt(originJSON, "timedeal_id");
            discountINT = SystemManager.GetJsonNodeInt(originJSON, "discount");
            
            expireTick = long.Parse(SystemManager.GetJsonNodeString(originJSON, "end_date_tick"));
        }
        
    }
}