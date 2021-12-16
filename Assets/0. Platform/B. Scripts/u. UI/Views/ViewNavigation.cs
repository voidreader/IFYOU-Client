using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PIERStory {

    public class ViewNavigation : CommonView
    {
        [SerializeField] List<Image> listNavigationIcons;
        
        [SerializeField] List<Sprite> listInactiveIcons; // 비활성 아이콘 스프라이트 
        [SerializeField] List<Sprite> listActiveIcons; // 활성 아이콘 스프라이트 
        
        public Dictionary<string, Sprite> DictSprite;
        
        public override void OnView()
        {
            base.OnView();
        }
        
        
        /// <summary>
        /// 네비게이션 첫번째 초기화 (Main) 으로 설정됩니다. 
        /// </summary>
        public void InitNavigation() {
            
            Debug.Log("InitNavigation");
            
            ResetIcons();   
            SetIconActive(0);
        }
        
        /// <summary>
        /// 네비게이션 버튼 활성화 
        /// </summary>
        /// <param name="__index"></param>
        public void ActivateNavigationButton(int __index) {
            ResetIcons();
            SetIconActive(__index);
        }
        
        
        /// <summary>
        /// 모든 아이콘 비활성화 상태로 변경 
        /// </summary>
        void ResetIcons() {
            for(int i=0; i < listNavigationIcons.Count;i++) {
                listNavigationIcons[i].sprite = listInactiveIcons[i];
                listNavigationIcons[i].SetNativeSize();
            }
        }
        
        /// <summary>
        ///  아이콘 활성화 처리 (인덱스)
        /// </summary>
        /// <param name="__index"></param>
        void SetIconActive(int __index) {
            listNavigationIcons[__index].sprite = listActiveIcons[__index];
            listNavigationIcons[__index].SetNativeSize();
        }
    }
}