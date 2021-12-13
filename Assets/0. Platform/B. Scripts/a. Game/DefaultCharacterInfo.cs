using UnityEngine;

using TMPro;

namespace PIERStory
{
    public class DefaultCharacterInfo : MonoBehaviour
    {
        Transform dummyCharacter;
        TextMeshProUGUI characterInfo = null;

        public void SetCharacterInfo(Transform t, string talkerName, string talker_expression)
        {
            characterInfo = GetComponentInChildren<TextMeshProUGUI>();
            dummyCharacter = t;
            characterInfo.text = string.Format("{0} : {1}", talkerName, talker_expression);
        }

        void Update()
        {
            // 더미캐릭터 사용중이고 활성화 중일때, UI 라벨이 더미캐릭터를 쫓아다니게 한다
            if (gameObject.activeSelf)
                transform.position = new Vector3(dummyCharacter.position.x, transform.position.y, 0f);
        }
    }
}
