using UnityEngine;

namespace PIERStory
{
    public class GameSpriteCtrl : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite sprite = null;
        public string spriteName = string.Empty;

        public float gameScale = 10;
        public float offset_x = 0;
        public float offset_y = 0;

        /// <summary>
        /// 스프라인트 단순 초기화
        /// </summary>
        public void InitSprite(Sprite __sp, string __name, float __scale = 1)
        {
            // ! 이렇게 안쓰면 sprite size가 이전 것을 갖고 있는다. 
            // ! 유니티 오류 같아..
            spriteRenderer.sprite = null;

            sprite = __sp;
            spriteRenderer.sprite = sprite;

            spriteName = __name;
            gameScale = __scale;

            transform.localScale = new Vector3(gameScale, gameScale, 1);
            gameObject.SetActive(false);

        }
    }
}

