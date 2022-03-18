using UnityEngine.UI;

using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewAbility : CommonView
    {
        public CharacterAbilityElement[] characterAbilityElements;
        public ScrollRect scroll;


        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACKGROUND, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_PROPERTY_GROUP, false, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_SHOW_BACK_BUTTON, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME_EXIST, true, string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, SystemManager.GetLocalizedText("5162"), string.Empty);
            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_ATTENDANCE, false, string.Empty);

            scroll.horizontalNormalizedPosition = 0f;

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