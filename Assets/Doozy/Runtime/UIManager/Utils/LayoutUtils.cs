using UnityEngine;
using UnityEngine.UI;
namespace Doozy.Runtime.UIManager.Utils
{
    public static class LayoutUtils
    {
        /// <summary> Returns TRUE if the target RectTransform's parent is a LayoutGroup </summary>
        /// <param name="target"> Target RectTransform </param>
        public static bool InLayoutGroup(this RectTransform target)
        {
            Transform parent = target.parent;
            LayoutGroup layoutGroup = parent != null ? parent.GetComponent<LayoutGroup>() : null;
            bool inLayoutGroup = layoutGroup != null;
            // if (inLayoutGroup) LayoutRebuilder.MarkLayoutForRebuild(layoutGroup.GetComponent<RectTransform>());
            return inLayoutGroup;
        }
        
         
    }
}
