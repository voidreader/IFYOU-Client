using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory
{
    public class MainProfile : MonoBehaviour
    {
        public static Action OnRefreshIFYOUAchievement = null;
        public static Action OnSaveVerticalNormalize = null;

        public ScrollRect scroll;

        [Header("계정 등급 뱃지 관련")]
        public Image gradeBackground;
        public Image badgebackAura;
        public Image gradeBadge;
        public GameObject badgeGlitter;

        [Space]
        public Sprite spriteDefaultAura;
        public Sprite spriteBestAura;

        [Space]
        public Sprite spriteBronzeBackground;
        public Sprite spriteSilverBackground;
        public Sprite spriteGoldBackground;
        public Sprite spritePlatinumBackground;
        public Sprite spriteIFYOUBackground;

        [Space]
        public TextMeshProUGUI gradeTitle;      // 등급 명칭

        [Header("계정 경험치 관련")]
        public Image expGauge;
        public RectTransform expMinimumBar;
        public TextMeshProUGUI expText;

        public Image downgradeBadge;
        public TextMeshProUGUI downgradeTitle;
        public Image nextGradeBadge;
        public TextMeshProUGUI nextGradeTitle;

        [Space(15)][Header("혜택 제공 텍스트 관련")]
        public TextMeshProUGUI seasonEndText;
        public TextMeshProUGUI benefitDetailText;

        Color bronzeText = new Color32(179, 93, 60, 255);
        Color silverText = new Color32(97, 97, 97, 255);
        Color goldText = new Color32(161, 48, 46, 255);
        Color platinumText = new Color32(0, 109, 190, 255);
        Color ifyouText = new Color32(255, 0, 128, 255);


        [Space(15)][Header("초심자 업적")]
        public RectTransform newbieAchievements;
        public GameObject newbieAchievementPrefab;
        public Transform newbieAchievementContents;


        [Space(15)][Header("이프유 업적")]
        public RectTransform IFYOUAchievements;
        public GameObject IFYOUAchievementPrefab;
        public Transform IFYOUAchievementContents;

        List<GameObject> achievementElements = new List<GameObject>();
        public static float postVerticalNormalize = -1f;

        private void Awake()
        {
            OnRefreshIFYOUAchievement = RefreshAchievementList;
            OnSaveVerticalNormalize = SetVerticalNormalize;
        }

        /// <summary>
        /// 프로필 화면 들어왔을 때 실행
        /// </summary>
        public void EnterProfile()
        {
            switch (UserManager.main.grade)
            {
                case 1:
                    gradeBackground.sprite = spriteBronzeBackground;
                    gradeTitle.color = bronzeText;
                    break;
                case 2:
                    gradeBackground.sprite = spriteSilverBackground;
                    gradeTitle.color = silverText;
                    break;
                case 3:
                    gradeBackground.sprite = spriteGoldBackground;
                    gradeTitle.color = goldText;
                    break;
                case 4:
                    gradeBackground.sprite = spritePlatinumBackground;
                    gradeTitle.color = platinumText;
                    break;
            }

            SetBadgeSprite(gradeBadge, gradeTitle, UserManager.main.grade);
            SetBadgeSprite(downgradeBadge, downgradeTitle, UserManager.main.grade - 1);
            SetBadgeSprite(nextGradeBadge, nextGradeTitle, UserManager.main.nextGrade + 1);

            badgeGlitter.SetActive(UserManager.main.grade > 3);
            downgradeBadge.gameObject.SetActive(UserManager.main.gradeExperience < UserManager.main.keepPoint);
            downgradeTitle.gameObject.SetActive(UserManager.main.gradeExperience < UserManager.main.keepPoint);
            expMinimumBar.gameObject.SetActive(UserManager.main.gradeExperience < UserManager.main.keepPoint);


            if(UserManager.main.grade == 1)
                benefitDetailText.text = string.Format(SystemManager.GetLocalizedText("6296"));
            else
            {
                benefitDetailText.text = string.Format(SystemManager.GetLocalizedText("6269"), UserManager.main.additionalStarDegree, UserManager.main.additionalStarUse, UserManager.main.additionalStarLimitCount, UserManager.main.waitingSaleDegree);

                if (UserManager.main.canPreview)
                    benefitDetailText.text += "\n" + SystemManager.GetLocalizedText("6270");
            }

            // 등급 방어 포인트 바 표기
            float posX = ((float)UserManager.main.keepPoint / (float)UserManager.main.upgradeGoalPoint) * expGauge.rectTransform.sizeDelta.x;
            expMinimumBar.anchoredPosition = new Vector2(posX, expMinimumBar.anchoredPosition.y);

            expGauge.fillAmount = (float)UserManager.main.gradeExperience / (float)UserManager.main.upgradeGoalPoint;
            expText.text = string.Format("({0}/{1})", UserManager.main.gradeExperience, UserManager.main.upgradeGoalPoint);

            seasonEndText.text = string.Format(SystemManager.GetLocalizedText("6293"), UserManager.main.remainDay);

            if (gameObject.activeSelf)
                StartCoroutine(LayoutRebuild());
        }

        IEnumerator LayoutRebuild()
        {
            yield return null;

            newbieAchievements.sizeDelta = new Vector2(newbieAchievements.sizeDelta.x,
                newbieAchievements.GetChild(0).GetComponent<RectTransform>().sizeDelta.y + newbieAchievementContents.GetComponent<RectTransform>().sizeDelta.y);
            IFYOUAchievements.sizeDelta = new Vector2(IFYOUAchievements.sizeDelta.x,
                IFYOUAchievements.GetChild(0).GetComponent<RectTransform>().sizeDelta.y + IFYOUAchievementContents.GetComponent<RectTransform>().sizeDelta.y);

            newbieAchievements.gameObject.SetActive(false);
            IFYOUAchievements.gameObject.SetActive(false);
            yield return null;
            newbieAchievements.gameObject.SetActive(newbieAchievementContents.childCount > 0);
            IFYOUAchievements.gameObject.SetActive(IFYOUAchievementContents.childCount > 0);

            yield return null;

            if (postVerticalNormalize < 0f)
                scroll.verticalNormalizedPosition = 1f;
            else
            {
                scroll.verticalNormalizedPosition = postVerticalNormalize;
                postVerticalNormalize = -1f;
            }
        }


        public void OnClickOpenGradeBenefit()
        {
            PopupBase p = PopupManager.main.GetPopup(LobbyConst.POPUP_GRADE_BENEFIT_INFO);

            if(p == null)
            {
                Debug.LogError("혜택 안내 팝업 존재 안함");
                return;
            }

            PopupManager.main.ShowPopup(p, false);
        }

        void RefreshAchievementList()
        {
            foreach (GameObject g in achievementElements)
                Destroy(g);

            achievementElements.Clear();

            IFYOUAchievementElement achievementElement;

            for (int i = 0; i < UserManager.main.listAchievement.Count; i++)
            {
                // 타입이 empty인 경우 초심자 업적임
                if (string.IsNullOrEmpty(UserManager.main.listAchievement[i].achievementType))
                {
                    achievementElement = Instantiate(newbieAchievementPrefab, newbieAchievementContents).GetComponent<IFYOUAchievementElement>();
                    achievementElement.InitNewbieAcheivement(UserManager.main.listAchievement[i]);
                    achievementElements.Add(achievementElement.gameObject);
                }
                else
                {
                    achievementElement = Instantiate(IFYOUAchievementPrefab, IFYOUAchievementContents).GetComponent<IFYOUAchievementElement>();
                    achievementElement.InitIFYouAchievement(UserManager.main.listAchievement[i]);
                    achievementElements.Add(achievementElement.gameObject);
                }
            }

            ViewMain.OnRefreshProfileNewSign?.Invoke();

            EnterProfile();
        }

        void SetBadgeSprite(Image __img, TextMeshProUGUI __text, int __grade)
        {
            __img.gameObject.SetActive(true);
            __text.gameObject.SetActive(true);

            switch (__grade)
            {
                case 1:
                    __img.sprite = LobbyManager.main.spriteBronzeBadge;
                    __text.text = SystemManager.GetLocalizedText("5191");
                    break;
                case 2:
                    __img.sprite = LobbyManager.main.spriteSilverBadge;
                    __text.text = SystemManager.GetLocalizedText("5192");
                    break;
                case 3:
                    __img.sprite = LobbyManager.main.spriteGoldBadge;
                    __text.text = SystemManager.GetLocalizedText("5193");
                    break;
                case 4:
                    __img.sprite = LobbyManager.main.spritePlatinumBadge;
                    __text.text = SystemManager.GetLocalizedText("5194");
                    break;
                case 5:
                    __img.sprite = LobbyManager.main.spriteIFYOUBadge;
                    __text.text = SystemManager.GetLocalizedText("5195");
                    break;
                default:
                    __img.gameObject.SetActive(false);
                    __text.gameObject.SetActive(false);
                    break;
            }
        }

        void SetVerticalNormalize()
        {
            postVerticalNormalize = scroll.verticalNormalizedPosition;
        }
    }
}