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
        
        void Awake() {
            main = this;
        }
    }
}