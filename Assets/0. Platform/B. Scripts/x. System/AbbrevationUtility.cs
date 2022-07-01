using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

 public static class AbbrevationUtility
 {
     private static readonly SortedDictionary<int, string> abbrevations = new SortedDictionary<int, string>
     {
         {1000,"K"},
         {1000000, "M" },
         {1000000000, "B" }
     };
 
    /// <summary>
    /// 소수점 포함하지 않음 
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
     public static string AbbreviateNumber(float number)
     {
         for (int i = abbrevations.Count - 1; i >= 0; i--)
         {
             KeyValuePair<int, string> pair = abbrevations.ElementAt(i);
             if (Mathf.Abs(number) >= pair.Key)
             {
                 int roundedNumber = Mathf.FloorToInt(number / pair.Key);
                 return roundedNumber.ToString() + pair.Value;
             }
         }
         return number.ToString();
     }
 
    /// <summary>
    /// 소수점 2자리 까지 
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>    
    public static string intToSimple(int val) {
        
        if(val > 1000000000)
            return string.Format("{0:0.00}B", val * 0.000000001f);
        else if(val > 1000000)
            return string.Format("{0:0.00}M", val * 0.000001f);
        else if(val > 1000)
            return string.Format("{0:0.00}K", val * 0.001f);
        else
            return val.ToString();
    }

    /// <summary>
    /// 소수점 1자리 까지 
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string FormatNumberFirstDecimalPlace(int val)
    {

        if (val > 1000000000)
            return string.Format("{0:0.0}B", val * 0.000000001f);
        else if (val > 1000000)
            return string.Format("{0:0.0}M", val * 0.000001f);
        else if (val > 1000)
            return string.Format("{0:0.0}K", val * 0.001f);
        else
            return val.ToString();
    }
}
 