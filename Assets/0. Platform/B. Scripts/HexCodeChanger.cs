using UnityEngine;

namespace PIERStory
{
    public static class HexCodeChanger
    {
        /// <summary>
        /// hex�ڵ� ����(6 or 8�ڸ�) # �����ؼ� �Է�
        /// </summary>
        /// <param name="__hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string __hex)
        {
            if (__hex.Length != 6 && __hex.Length != 8)
                return Color.black;

            string hexCode = __hex.Length == 6 ? "#" + __hex + "FF" : "#" + __hex;
            hexCode = hexCode.ToUpper();

            Color c;

            ColorUtility.TryParseHtmlString(hexCode, out c);
            return c;
        }

        public static Color ColorConvert(string hexCode)
        {
            Color c;

            if (ColorUtility.TryParseHtmlString(hexCode, out c))
                return c;
            else
                return Color.black;
        }
    }
}

