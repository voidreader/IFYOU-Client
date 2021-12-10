using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class OnetimePlayHistoryElement : MonoBehaviour
    {
        public TextMeshProUGUI storyTitle;
        public TextMeshProUGUI date;
        public GameObject useTag;
        public GameObject expirationTag;
        public TextMeshProUGUI ticketAmount;

        public void InitHistoryData(bool useHistory)
        {

            // 사용내역이 아니므로 사용,만료 태그를 비활성화한다
            if(!useHistory)
            {
                useTag.SetActive(false);
                expirationTag.SetActive(false);

                ticketAmount.text = "+1장";
            }
            else
            {
                ticketAmount.text = "-1장";
            }
        }
    }
}
