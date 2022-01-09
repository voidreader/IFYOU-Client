using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PIERStory {
    public class GameSpriteHolder : MonoBehaviour
    {
        public static GameSpriteHolder main = null;
        
        public Sprite spriteSelectionNormal; // 인게임 선택지 버튼 일반 
        public Sprite spriteSelectionLock; // 인게임 선택지 버튼 락
        public Sprite spriteSelectionUnlock; // 인게임 선택지 버튼 언락 
        
        public Sprite spriteInappOriginIcon; // 메일함에서 사용하는 아이콘
        
        void Awake() {
            main = this;
        }
    }
}