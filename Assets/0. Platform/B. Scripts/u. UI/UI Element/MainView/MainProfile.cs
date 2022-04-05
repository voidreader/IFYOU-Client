using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory
{
    public class MainProfile : MonoBehaviour
    {
        public Image gradeBackground;
        public Image badgebackAura;
        public Image gradeBadge;
        public GameObject badgeGlitter;

        public Sprite spriteDefaultAura;
        public Sprite spriteBestAura;

        public TextMeshProUGUI gradeTitle;      // 등급 명칭

        public Image expGauge;
        public TextMeshProUGUI expText;

        public Image downgradeBadge;
        public TextMeshProUGUI downgradeTitle;
        public Image nextGradeBadge;
        public TextMeshProUGUI nextGradeTitle;


    }
}