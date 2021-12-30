using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

namespace PIERStory {
    public class GenreToggle : MonoBehaviour
    {
        [SerializeField] Image buttonImage;
        [SerializeField] TextMeshProUGUI textGenre;
        
        [SerializeField] string originName = string.Empty; // 검색을 위한 원래 이름
        [SerializeField] string localizedName = string.Empty;
        
        [SerializeField] bool isFirstToggle = false; // 관심작품용도 
        
        /// <summary>
        /// 장르 정보 설정 
        /// </summary>
        /// <param name="__j"></param>
        public void SetGenre(JsonData __j) {
            
            this.gameObject.SetActive(true);
            
            originName = __j["origin_name"].ToString();
            localizedName = __j["genre_name"].ToString();
            textGenre.text = localizedName;
            
            SetOff();
        }
        
        public void SetOff(){
            buttonImage.sprite = LobbyManager.main.spriteGenreOff;
            textGenre.color = LobbyManager.main.colorGenreOff;   
        }
        
        public void SetOn() {
            buttonImage.sprite = LobbyManager.main.spriteGenreOn;
            textGenre.color = LobbyManager.main.colorGenreOn;   
        }
        
        public void OnClickGenre() {
            
        }
           
    }
}