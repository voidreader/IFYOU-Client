using UnityEngine;
using UnityEngine.UI;

using TMPro;
using LitJson;
using BestHTTP;


namespace PIERStory
{
    public class IFYOUAchievementElement : MonoBehaviour
    {
        [Header("공통")]
        AchievementData achievementData;
        public ImageRequireDownload achieveThumbnail;
        public TextMeshProUGUI expText;

        public TextMeshProUGUI achieveExplain;

        public Image achievementGauge;
        public TextMeshProUGUI degreeOfAchievementText;

        public GameObject getButton;

        [Space(15)][Header("IFyou 업적")]
        [Header("레벨태그")]
        public Image levelTagBackground;
        public TextMeshProUGUI achievementLevelText;
        [Header("아이콘 태그")]
        public GameObject iconBadgeTag;
        public Image tagAchievementIcon;

        public void InitNewbieAcheivement(AchievementData __achievementData)
        {
            CommonInitialize(__achievementData);
        }


        public void InitIFYouAchievement(AchievementData __achievementData)
        {
            CommonInitialize(__achievementData);

            levelTagBackground.gameObject.SetActive(achievementData.achievementType == "level");
            iconBadgeTag.gameObject.SetActive(achievementData.achievementType == "repeat");

            SetAchievementLevel();
        }


        /// <summary>
        /// 공통 초기화
        /// </summary>
        void CommonInitialize(AchievementData __achievementData)
        {
            achievementData = __achievementData;
            achieveThumbnail.SetDownloadURL(achievementData.achievementIconUrl, achievementData.achievementIconKey, true);
            expText.text = string.Format("+{0}", achievementData.experience);

            SystemManager.SetText(achieveExplain, achievementData.achievementSummary);

            achievementGauge.fillAmount = achievementData.achievementDegree;
            degreeOfAchievementText.color = Color.white;
            degreeOfAchievementText.text = string.Format("({0}/{1})", achievementData.currentPoint, achievementData.achievementPoint);

            getButton.SetActive(achievementData.achievementDegree >= 1f);
        }

        void SetAchievementLevel()
        {
            achievementLevelText.text = string.Format("{0} LV", achievementData.currentLevel);

            if (achievementData.currentLevel >= 1 && achievementData.currentLevel < 30)
                levelTagBackground.sprite = LobbyManager.main.spriteLevelTag1;
            else if (achievementData.currentLevel >= 30 && achievementData.currentLevel < 50)
                levelTagBackground.sprite = LobbyManager.main.spriteLevelTag2;
            else if (achievementData.currentLevel >= 50 && achievementData.currentLevel < 100)
                levelTagBackground.sprite = LobbyManager.main.spriteLevelTag3;
            else if (achievementData.currentLevel >= 100)
                levelTagBackground.sprite = LobbyManager.main.spriteLevelTag4;
        }

        public void OnClickAchieveClaim()
        {
            JsonData sending = new JsonData();
            sending[CommonConst.FUNC] = "updateUserAchievement";
            sending[CommonConst.COL_USERKEY] = UserManager.main.userKey;
            sending["achievement_id"] = achievementData.achievementId;

            NetworkLoader.main.SendPost(CallbackUpdateUserAchievement, sending, true);
        }

        void CallbackUpdateUserAchievement(HTTPRequest req, HTTPResponse res)
        {
            if (!NetworkLoader.CheckResponseValidation(req, res))
            {
                Debug.LogError("CallbackUpdateUserAchievement");

                JsonData errordata = JsonMapper.ToObject(res.DataAsText);

                if (SystemManager.GetJsonNodeInt(errordata, "code") == 80117)
                    ViewMain.OnReturnLobby?.Invoke();

                // 이미 받은 경우 화면 갱신을 요청한다
                else if (SystemManager.GetJsonNodeInt(errordata, "code") == 80019)
                    UserManager.main.RequestUserGradeInfo(UserManager.main.CallbackUserGreadeInfo);

                return;
            }

            JsonData result = JsonMapper.ToObject(res.DataAsText);

            MainProfile.OnSaveVerticalNormalize?.Invoke();

            PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_GRADE_EXP);

            if(p == null)
            {
                Debug.LogError("등급 경험치 획득 팝업 없음!");
                return;
            }

            Sprite s = null;

            switch(UserManager.main.nextGrade + 1)
            {
                case 1:
                    s = LobbyManager.main.spriteBronzeBadge;
                    break;
                case 2:
                    s = LobbyManager.main.spriteSilverBadge;
                    break;
                case 3:
                    s = LobbyManager.main.spriteGoldBadge;
                    break;
                case 4:
                    s = LobbyManager.main.spritePlatinumBadge;
                    break;
                case 5:
                    s = LobbyManager.main.spriteIFYOUBadge;
                    break;
            }

            p.Data.SetImagesSprites(s);
            p.Data.SetLabelsTexts(string.Format("+{0}", achievementData.experience), string.Format("/{0}", UserManager.main.upgradeGoalPoint));
            p.Data.contentValue = achievementData.experience;
            p.Data.contentJson = result["list"]; // 전달 주고 
            PopupManager.main.ShowPopup(p, false);

            UserManager.main.SetSeasonCheck(result["list"]);
        }
    }
}