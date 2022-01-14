using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace PIERStory
{
    public class MessengerUserInfo : MonoBehaviour
    {
        public Image profileImage;
        public TextMeshProUGUI nickName;
        public Transform bubblesParent;

        public void SetMessengerForm(Sprite profileSprite, string name)
        {
            profileImage.sprite = profileSprite;
            nickName.text = StoryManager.main.GetNametagName(name);
        }
    }
}