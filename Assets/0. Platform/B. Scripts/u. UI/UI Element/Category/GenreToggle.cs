using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LitJson;

namespace PIERStory {
    public class GenreToggle : MonoBehaviour
    {
        public string locatedPlace = string.Empty;
        
        [SerializeField] Image buttonImage;
        [SerializeField] TextMeshProUGUI textGenre;
        
        [SerializeField] string originName = string.Empty; // 검색을 위한 원래 이름
        [SerializeField] string localizedName = string.Empty;
        
        [SerializeField] bool isFirstToggle = false; // 관심작품용도 
        
        public GameObject cover; // 커버 
        
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

            if (LobbyManager.main == null)
                return;

            // buttonImage.sprite = LobbyManager.main.spriteGenreOff;
            // textGenre.color = LobbyManager.main.colorGenreOff;   
            if(cover != null) {
                cover.SetActive(false);
            }
        }
        
        public void SetOn() {

            if (LobbyManager.main == null)
                return;

            // buttonImage.sprite = LobbyManager.main.spriteGenreOn;
            // textGenre.color = LobbyManager.main.colorGenreOn;   
            
            if(cover != null) {
                cover.SetActive(true);
            }
            
            OnClickGenre();
        }
        
        public void OnClickGenre() {
            
            if(locatedPlace == "Lobby")
                IFYouLobby.OnCategoryList?.Invoke(localizedName);
            else if(locatedPlace == "Library") 
                MainLibrary.OnCategoryList?.Invoke(localizedName);
        }
        
        void OnEnable() {
            
            if(SystemManager.main == null)
                return;
            
            if(!isFirstToggle)
                return;
           
            if(originName == "Main") {
                localizedName = SystemManager.GetLocalizedText("5131");
                
            }
            else if(originName == "All"){
                localizedName = SystemManager.GetLocalizedText("5137");
            }
            
            textGenre.text = localizedName;
        }
           
    }
}