

namespace PIERStory
{
    public class ViewAbility : CommonView
    {
        public CharacterAbilityElement[] characterAbilityElements;


        public override void OnStartView()
        {
            base.OnStartView();

            int j = 0;

            foreach (string key in UserManager.main.DictStoryAbility.Keys)
            {
                for (int i = 0; i < UserManager.main.DictStoryAbility[key].Count; i++)
                {
                    // 능력치값 세팅. 서버 정렬로 첫번쨰는 main 능력치가 오고 있음
                    if (i == 0)
                        characterAbilityElements[j].InitMainAbility(UserManager.main.DictStoryAbility[key][i]);
                    else if (i == 1)
                        characterAbilityElements[j].InitFirstSubAbility(UserManager.main.DictStoryAbility[key][i]);
                    else
                        characterAbilityElements[j].InitSecondSubAbility(UserManager.main.DictStoryAbility[key][i]);
                }
                j++;
            }
        }


        public override void OnHideView()
        {
            base.OnHideView();

            foreach (CharacterAbilityElement element in characterAbilityElements)
                element.gameObject.SetActive(false);
        }
    }
}