// Copyright (c) 2015 - 2021 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Collections.Generic;
// ReSharper disable All
namespace Doozy.Runtime.UIManager.Containers
{
    public partial class UIView
    {
        public static IEnumerable<UIView> GetViews(UIViewId.Common id) => GetViews(nameof(UIViewId.Common), id.ToString());
        public static void Show(UIViewId.Common id, bool instant = false) => Show(nameof(UIViewId.Common), id.ToString(), instant);
        public static void Hide(UIViewId.Common id, bool instant = false) => Hide(nameof(UIViewId.Common), id.ToString(), instant);

        public static IEnumerable<UIView> GetViews(UIViewId.Game id) => GetViews(nameof(UIViewId.Game), id.ToString());
        public static void Show(UIViewId.Game id, bool instant = false) => Show(nameof(UIViewId.Game), id.ToString(), instant);
        public static void Hide(UIViewId.Game id, bool instant = false) => Hide(nameof(UIViewId.Game), id.ToString(), instant);

        public static IEnumerable<UIView> GetViews(UIViewId.IFYOU id) => GetViews(nameof(UIViewId.IFYOU), id.ToString());
        public static void Show(UIViewId.IFYOU id, bool instant = false) => Show(nameof(UIViewId.IFYOU), id.ToString(), instant);
        public static void Hide(UIViewId.IFYOU id, bool instant = false) => Hide(nameof(UIViewId.IFYOU), id.ToString(), instant);

        public static IEnumerable<UIView> GetViews(UIViewId.Popup id) => GetViews(nameof(UIViewId.Popup), id.ToString());
        public static void Show(UIViewId.Popup id, bool instant = false) => Show(nameof(UIViewId.Popup), id.ToString(), instant);
        public static void Hide(UIViewId.Popup id, bool instant = false) => Hide(nameof(UIViewId.Popup), id.ToString(), instant);
    }
}

namespace Doozy.Runtime.UIManager
{
    public partial class UIViewId
    {
        public enum Common
        {
            CurrencyTop,
            EpisodeStart,
            Mail,
            Top
        }

        public enum Game
        {
            EpisodeEnd,
            InGame,
            InGameLoading,
            InGameMenu,
            InGameOption
        }

        public enum IFYOU
        {
            Beginning,
            Category,
            DataManager,
            Gallery,
            IfyouPlay,
            IllustDetail,
            Lobby,
            Main,
            Mission,
            Navigation,
            Notice,
            NoticeDetail,
            OneTimeHistory,
            Profile,
            Shop,
            SoundDetail,
            StoryDetail,
            Title
        }

        public enum Popup
        {
            AlertMessage,
            AlertSimple,
            ConfirmType1,
            ConfirmType2,
            Coupon,
            PrivacyPolicy
        }    
    }
}