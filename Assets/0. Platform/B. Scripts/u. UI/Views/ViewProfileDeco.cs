using System;
using UnityEngine;

using Doozy.Runtime.Signals;

namespace PIERStory
{
    public class ViewProfileDeco : CommonView
    {
        public static Action OnDisableAllOptionals = null;

        public Transform decoObjects;

        public override void OnStartView()
        {
            base.OnStartView();

            Signal.Send(LobbyConst.STREAM_TOP, LobbyConst.TOP_SIGNAL_VIEW_NAME, "꾸미기모드", string.Empty);

            OnDisableAllOptionals = OnClickAllDisable;
        }

        public override void OnHideView()
        {
            base.OnHideView();
        }

        /// <summary>
        /// 화면 위의 Optionals 모두 비활성화
        /// </summary>
        public void OnClickAllDisable()
        {
            for(int i=0;i<decoObjects.childCount;i++)
                decoObjects.GetChild(i).GetComponent<DecoElement>().DisableOptionals();
        }
    }
}